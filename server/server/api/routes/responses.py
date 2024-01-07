from fastapi import Request, APIRouter, status
from fastapi.responses import JSONResponse
import json

import server.server.api.models.schemas as schemas
from server.server.api.dependencies.user import decode_jwt
from server.server.core.database import (new_response as database_new_response,
                                         get_all_responses as database_get_all_responses,
                                         get_all_responses_owner as database_get_all_responses_owner,
                                         claim_response as database_claim_response)

router = APIRouter()


@router.get("/all", response_model=schemas.Responses_info)
def responses(request: Request, page_size: int = 10, page: int = 1):
    if "jwt_token" not in request.cookies:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы должны авторизоваться")
    user_id = decode_jwt(json.loads(request.cookies["jwt_token"]))
    get_result_user = database_get_all_responses(user_id, page_size, page)
    if not get_result_user["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=get_result_user["msg"])
    get_result_owner = database_get_all_responses_owner(user_id, page_size, page)
    if not get_result_owner["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=get_result_owner["msg"])
    return schemas.Responses_info(responses_owner=get_result_owner["cars"],
                                  responses_user=get_result_user["cars"])


@router.post("/claim")
def claim_response(request: Request, response_id: schemas.Id):
    if "jwt_token" not in request.cookies:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы должны авторизоваться")
    user_id = decode_jwt(json.loads(request.cookies["jwt_token"]))

    claim_result = database_claim_response(response_id.id, user_id)
    if not claim_result["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=claim_result["msg"])
    return JSONResponse(status_code=status.HTTP_200_OK, content="Ok")


@router.post("/new", response_model=schemas.Response)
def new_response(request: Request, message: schemas.New_Response_data):
    if "jwt_token" not in request.cookies:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы должны авторизоваться")
    user_id = decode_jwt(json.loads(request.cookies["jwt_token"]))
    create_result = database_new_response(message.car_id, user_id, message.message)
    if not create_result["successful"]:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=create_result["msg"])
    return create_result["response"]
