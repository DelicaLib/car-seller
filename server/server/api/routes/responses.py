from fastapi import Request, APIRouter
import json

import server.server.api.models.schemas as schemas
from server.server.api.dependencies.user import decode_jwt

router = APIRouter()


@router.get("/all", response_model=schemas.Response)
def get_responses(request: Request, page_size: int = 10, page: int = 1):
    return ""


@router.post("/claim")
def claim_response(request: Request, response_id: schemas.Id):
    return ""

@router.post("/new")
def new_response(request: Request, message: schemas.New_Response_data):
    user_id = decode_jwt(json.loads(request.cookies["jwt_token"]))
