from fastapi import APIRouter, Request, status, Response
from fastapi.responses import JSONResponse
import json

from server.server.core.config import RECAPTCHA_SECRET_KEY
from server.server.api.models import schemas
from server.server.api.dependencies import user
from server.server.core.database import (get_user as database_get_user,
                                         new_user as database_new_user,
                                         login_user as database_login_user)

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
                               email=found_user.surname,
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
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST,
                            content="Пользователь с таким телефоном и/или почтой уже зарегистрирован")
    added_user = database_new_user(new_user_data)
    return added_user


@router.get("/{user_id}", response_model=schemas.User)
async def get_user(request: Request, user_id: int):
    found_user = database_get_user(user_id=user_id)
    if found_user is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Пользователь не найден")
    authorize_check_result = user.decode_jwt(json.loads(request.cookies["jwt_token"]))
    print(authorize_check_result)
    if authorize_check_result is None or authorize_check_result != found_user.id:
        user.delete_confidential_data(found_user)
    return found_user
