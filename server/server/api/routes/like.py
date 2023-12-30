from fastapi import Request, APIRouter
from typing import List

import server.server.api.models.schemas as schemas

router = APIRouter()


@router.get("/all", response_model=List[schemas.Car])
def get_likes(request: Request, page_size: int = 10, page: int = 1):
    return ""


@router.get("/")
def like(request: Request, like_id: schemas.Id):
    return ""


@router.get("/car/{car_id}")
def like(request: Request, car_id: int):
    return ""
