from fastapi import Request, APIRouter
from typing import List

import server.server.api.models.schemas as schemas
import server.server.core.database as database

router = APIRouter()


@router.get("/all", response_model=List[schemas.Car])
def get_likes(request: Request, page_size: int = 10, page: int = 1):
    return ""


@router.get("/car/{car_id}")
def like(request: Request, car_id: int):
    return ""


@router.delete("/car/{car_id}")
def delete_like(request: Request, car_id: int):
    return ""
