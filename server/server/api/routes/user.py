from fastapi import APIRouter, Request, status, Response
from fastapi.responses import JSONResponse
import json

from server.server.core.config import RECAPTCHA_SECRET_KEY
from server.server.api.models import schemas
from server.server.api.dependencies import user
from server.server.core.database import (get_user as database_get_user,
                                         new_user as database_new_user,
                                         login_user as database_login_user,
                                         verify_user as database_verify_user,
                                         change_user_data as database_change_user_data,
                                         is_claim_response as database_is_claim_response)

router = APIRouter()


@router.post("/login", response_model=schemas.User_JWT_Token)
async def login(request: Request, response: Response, login_data: schemas.Login_data,
                recaptcha: schemas.RecaptchaVerification):
    remote_ip = request.client.host
    recaptcha_is_valid = user.verify_recaptcha(recaptcha.recaptcha_response, RECAPTCHA_SECRET_KEY, remote_ip)
    if not recaptcha_is_valid:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content="Проверка reCAPTCHA не пройдена")

    found_user = database_login_user(login_data)
    if found_user is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Неверный логин и/или пароль")
    user_schema = schemas.User(id=found_user.id,
                               name=found_user.name,
                               surname=found_user.surname,
                               email=found_user.email,
                               phone_number=found_user.phone_number)
    jwt_token = user.generate_token(user_schema)
    response.set_cookie("jwt_token", json.dumps(jwt_token))
    return schemas.User_JWT_Token(**user_schema.__dict__, jwt_token=jwt_token)


@router.post("/new", response_model=schemas.User)
async def new_user(request: Request, recaptcha: schemas.RecaptchaVerification, new_user_data: schemas.New_user):
    remote_ip = request.client.host
    recaptcha_is_valid = user.verify_recaptcha(recaptcha.recaptcha_response, RECAPTCHA_SECRET_KEY, remote_ip)
    if not recaptcha_is_valid:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content="Проверка reCAPTCHA не пройдена")

    found_user = database_get_user(user_email=new_user_data.email, user_phone_number=new_user_data.phone_number)
    if found_user is not None:
        return JSONResponse(status_code=status.HTTP_403_FORBIDDEN,
                            content="Пользователь с таким телефоном и/или почтой уже зарегистрирован")
    added_user = database_new_user(new_user_data)
    return added_user


@router.get("/{user_id}", response_model=schemas.User)
async def get_user(request: Request, user_id: int):
    found_user = database_get_user(user_id=user_id)
    if found_user is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Пользователь не найден")

    authorize_check_result = None
    if "jwt_token" in request.cookies:
        authorize_check_result = user.decode_jwt(json.loads(request.cookies["jwt_token"]))

    found_user = user.confidential_data_user(authorize_check_result, found_user)
    return found_user


@router.get("/me/", response_model=schemas.User)
async def get_user_me(request: Request):
    id_user = None
    if "jwt_token" in request.cookies:
        id_user = user.decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_user is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")
    found_user = database_get_user(user_id=id_user)
    if found_user is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Пользователь не найден")
    return found_user


@router.post("/logout")
async def logout():
    response = JSONResponse(status_code=status.HTTP_200_OK, content="Вы вышли из аккаунта")
    response.delete_cookie("jwt_token")
    return response


@router.post("/edit", response_model=schemas.User)
async def change_user_data(request: Request, recaptcha: schemas.RecaptchaVerification,
                           user_new_data: schemas.Change_user_data):
    remote_ip = request.client.host
    recaptcha_is_valid = user.verify_recaptcha(recaptcha.recaptcha_response, RECAPTCHA_SECRET_KEY, remote_ip)
    if not recaptcha_is_valid:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content="Проверка reCAPTCHA не пройдена")

    authorized_id = None
    if "jwt_token" in request.cookies:
        authorized_id = user.decode_jwt(json.loads(request.cookies["jwt_token"]))
    if authorized_id is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")

    found_user = database_verify_user(user_new_data, authorized_id)
    if found_user is None:
        return JSONResponse(status_code=status.HTTP_403_FORBIDDEN,
                            content="Неправильный пароль")
    change_result = database_change_user_data(found_user, user_new_data)
    if not change_result["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST,
                            content=change_result["msg"])
    return change_result["user_data"]
