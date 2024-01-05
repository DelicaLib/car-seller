from fastapi import Request, APIRouter, status
from fastapi.responses import JSONResponse
import json
from typing import List

import server.server.api.models.schemas as schemas
import server.server.core.database as database
from server.server.api.dependencies.user import decode_jwt


router = APIRouter()


@router.get("/all", response_model=List[schemas.Car])
def get_likes(request: Request, page_size: int = 10, page: int = 1):
    id_user = None
    if "jwt_token" in request.cookies:
        id_user = decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_user is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")
    found_likes = database.get_user_likes(id_user, page_size, page)
    return found_likes


@router.get("/car/{car_id}")
def check_like(request: Request, car_id: int):
    id_user = None
    if "jwt_token" in request.cookies:
        id_user = decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_user is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")
    return database.has_like(id_user, car_id)


@router.post("/car/{car_id}")
def new_like(request: Request, car_id: int):
    id_user = None
    if "jwt_token" in request.cookies:
        id_user = decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_user is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")
    set_result = database.set_like(car_id, id_user)
    if not set_result["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=set_result["msg"])

    return JSONResponse(status_code=status.HTTP_200_OK, content="Ok")


@router.delete("/car/{car_id}")
def delete_like(request: Request, car_id: int):
    id_user = None
    if "jwt_token" in request.cookies:
        id_user = decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_user is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")
    delete_result = database.delete_like(id_user, car_id)
    if not delete_result["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=delete_result["msg"])
    return JSONResponse(status_code=status.HTTP_200_OK, content="Ok")
