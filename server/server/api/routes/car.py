from fastapi import Request, status, UploadFile, APIRouter
from fastapi.responses import JSONResponse
from typing import List
import json
from pydantic import ValidationError

import server.server.core.database as database
import server.server.api.models.schemas as schemas
from server.server.api.dependencies.car import is_correct_files, upload_files as do_upload_files
from server.server.api.dependencies.user import decode_jwt

router = APIRouter()


@router.post("/upload_images")
async def upload_images(request: Request, files: List[UploadFile]):
    check_result = is_correct_files(files)
    if not check_result:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=check_result["msg"])
    file_paths = do_upload_files(files)
    return file_paths


@router.post("/new", response_model=schemas.Car)
async def new_car(request: Request, new_car_data: schemas.New_car):
    id_owner = decode_jwt(json.loads(request.cookies["jwt_token"]))
    if id_owner is None:
        return JSONResponse(status_code=status.HTTP_401_UNAUTHORIZED, content="Вы не авторизованы")

    return database.add_car(new_car_data, id_owner, new_car_data.photos)


@router.get("/catalog/{car_id}", response_model=schemas.Car)
async def get_car(request: Request, car_id: int):
    car = database.get_car(car_id)
    if car is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Объявление не найдено")
    return car


@router.delete("/", response_model=schemas.Car)
async def delete_car(request: Request, car_id: schemas.Id):
    car = database.delete_car(car_id.id)
    if car is None:
        return JSONResponse(status_code=status.HTTP_404_NOT_FOUND, content="Объявление не найдено")
    return car


@router.get("/available_filters", response_model=schemas.Filters)
async def available_filters(city: str | None = None, brand: str | None = None,
                            model: str | None = None, body: schemas.Body | None = None,
                            transmission: schemas.Transmission | None = None, min_volume: int = 0,
                            engine: schemas.Engine | None = None, drive: schemas.Drive | None = None,
                            min_release_year: int = 0, max_release_year: int = 3000,
                            min_cost: int = 0, max_cost: int = 99999999999999,
                            min_mileage: int = 0, max_mileage: int = 99999999999999,
                            max_volume: int = 99999999999999):
    try:
        cur_filter = schemas.Filter(min_volume=min_volume, max_volume=max_volume,
                                    min_cost=min_cost, max_cost=max_cost,
                                    min_mileage=min_mileage, max_mileage=max_mileage,
                                    min_release_year=min_release_year, max_release_year=max_release_year,
                                    city=city, brand=brand,
                                    model=model, body=body,
                                    transmission=transmission, engine=engine,
                                    drive=drive)
    except ValidationError as e:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=str(e).split("\n")[-2])
    found_filters = database.get_available_filters(cur_filter=cur_filter)

    return found_filters


@router.get("/catalog", response_model=List[schemas.Car])
async def catalog(city: str | None = None, brand: str | None = None,
                  model: str | None = None, body: schemas.Body | None = None,
                  transmission: schemas.Transmission | None = None, min_volume: int = 0,
                  engine: schemas.Engine | None = None, drive: schemas.Drive | None = None,
                  min_release_year: int = 0, max_release_year: int = 3000,
                  min_cost: int = 0, max_cost: int = 99999999999999,
                  min_mileage: int = 0, max_mileage: int = 99999999999999,
                  max_volume: int = 99999999999999, page_size: int = 10,
                  page: int = 1):
    try:
        cur_filter = schemas.Filter(min_volume=min_volume, max_volume=max_volume,
                                    min_cost=min_cost, max_cost=max_cost,
                                    min_mileage=min_mileage, max_mileage=max_mileage,
                                    min_release_year=min_release_year, max_release_year=max_release_year,
                                    city=city, brand=brand,
                                    model=model, body=body,
                                    transmission=transmission, engine=engine,
                                    drive=drive)
    except ValidationError as e:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content=str(e).split("\n")[-2])
    if page_size < 1 or page < 1:
        return JSONResponse(status_code=status.HTTP_400_BAD_REQUEST, content="Неверный размер страницы или страница")
    result = database.get_catalog(cur_filter, page, page_size)
    if len(result) == 0:
        return database.get_catalog(cur_filter, 1, page_size)
    return result
