from sqlalchemy.orm import Session
from sqlalchemy import or_, and_, between
from datetime import datetime
from decimal import Decimal
from typing import List, Optional
import shutil

from server.server.api.models import models
import server.server.api.models.schemas as schemas

session = Session(models.engine)


def make_car_scheme(car: models.Car_):
    return schemas.Car(
        id=car.id,
        brand=car.brand,
        model=car.model,
        city=car.city,
        mileage=car.mileage,
        transmission=car.transmission,
        engine=car.engine,
        body=car.body,
        release_year=car.release_year,
        drive=car.drive,
        cost=Decimal("".join(car.cost.split('?')[0].split("\xa0")).replace(",", ".")),
        volume=car.volume,
        description=car.description,
        date_publish=car.date_publish,
        photos=car.photos.split(";"))


def get_available_filter_for(column, params: dict) -> list:
    result = []
    for i in session.query(column).filter(and_(
            models.Car_.city == params["city"],
            models.Car_.brand == params["brand"],
            models.Car_.model == params["model"],
            models.Car_.body == params["body"],
            models.Car_.transmission == params["transmission"],
            models.Car_.engine == params["engine"],
            models.Car_.drive == params["drive"],
            between(models.Car_.volume, params["min_volume"], params["max_volume"]),
            between(models.Car_.cost, params["min_cost"], params["max_cost"]),
            between(models.Car_.release_year, params["min_release_year"], params["max_release_year"]),
            between(models.Car_.mileage, params["min_mileage"], params["max_mileage"])
    )).distinct():
        result.append(i[0])
    return result


def get_available_filters(cur_filter: schemas.Filter) -> schemas.Filters:
    params = cur_filter.get_dict()

    tmp = params["city"]
    params["city"] = models.Car_.city
    city = get_available_filter_for(models.Car_.city, params)
    params["city"] = tmp

    tmp = [params["brand"], params["model"]]
    params["brand"] = models.Car_.brand
    params["model"] = models.Car_.model
    brand = get_available_filter_for(models.Car_.brand, params)
    params["brand"] = tmp[0]
    params["model"] = tmp[1]
    model = None
    if cur_filter.brand is not None:
        tmp = params["model"]
        params["model"] = models.Car_.model
        model = get_available_filter_for(models.Car_.model, params)
        params["model"] = tmp

    tmp = params["body"]
    params["body"] = models.Car_.body
    body = get_available_filter_for(models.Car_.body, params)
    params["body"] = tmp

    tmp = params["transmission"]
    params["transmission"] = models.Car_.transmission
    transmission = get_available_filter_for(models.Car_.transmission, params)
    params["transmission"] = tmp

    tmp = params["engine"]
    params["engine"] = models.Car_.engine
    engine = get_available_filter_for(models.Car_.engine, params)
    params["engine"] = tmp

    tmp = params["drive"]
    params["drive"] = models.Car_.drive
    drive = get_available_filter_for(models.Car_.drive, params)
    params["drive"] = tmp

    return schemas.Filters(city=city,
                           brand=brand,
                           model=model,
                           body=body,
                           transmission=transmission,
                           engine=engine,
                           drive=drive)


def add_car(new_ad_data: schemas.New_car, id_owner: int, photos: List[str]) -> schemas.Car:
    new_ad = models.Car_(
        brand=new_ad_data.brand,
        model=new_ad_data.model,
        city=new_ad_data.city,
        mileage=new_ad_data.mileage,
        transmission=new_ad_data.transmission.value,
        engine=new_ad_data.engine.value,
        body=new_ad_data.body.value,
        release_year=new_ad_data.release_year,
        drive=new_ad_data.drive.value,
        cost=new_ad_data.cost,
        volume=new_ad_data.volume,
        description=new_ad_data.description,
        date_publish=datetime.now(),
        id_owner=id_owner,
        photos=";".join(photos)
    )
    session.add(new_ad)
    session.commit()

    return make_car_scheme(new_ad)


def get_car_object(car_id: int) -> Optional[models.Car_]:
    car = session.query(models.Car_).filter(and_(models.Car_.id == car_id))
    if car.count() == 0:
        return None
    return car.one()


def get_car(car_id: int):
    car = get_car_object(car_id)
    if car is None:
        return None
    return make_car_scheme(car)


def delete_car(car: models.Car_):
    if car is None:
        return None
    delete_all_car_likes(car.id)
    car_scheme = make_car_scheme(car)
    session.delete(car)
    shutil.rmtree("/".join(car_scheme.photos[0].split("/")[:4]))
    session.commit()
    return car_scheme


def get_catalog(car_filter: schemas.Filter, page: int, page_size: int) -> List[schemas.Car]:
    params = car_filter.get_dict()
    result = []
    offset = page_size * (page - 1)
    for i in session.query(models.Car_).filter(
            models.Car_.city == params["city"],
            models.Car_.brand == params["brand"],
            models.Car_.model == params["model"],
            models.Car_.body == params["body"],
            models.Car_.transmission == params["transmission"],
            models.Car_.engine == params["engine"],
            models.Car_.drive == params["drive"],
            between(models.Car_.volume, params["min_volume"], params["max_volume"]),
            between(models.Car_.cost, params["min_cost"], params["max_cost"]),
            between(models.Car_.release_year, params["min_release_year"], params["max_release_year"]),
            between(models.Car_.mileage, params["min_mileage"], params["max_mileage"])
    ).limit(page_size).offset(offset):
        result.append(make_car_scheme(i))
    return result


def get_user(user_id: int = -1, user_email: str = "", user_phone_number: str = ""):
    found_user = session.query(models.User_).filter(or_(models.User_.id == user_id,
                                                        models.User_.email == user_email,
                                                        models.User_.phone_number == user_phone_number))
    if found_user.count() == 0:
        return None
    found_user = found_user.one()
    return schemas.User(id=found_user.id,
                        name=found_user.name,
                        surname=found_user.surname,
                        email=found_user.email,
                        phone_number=found_user.phone_number)


def new_user(new_user_data: schemas.New_user) -> schemas.User:
    added_user = models.User_(name=new_user_data.name,
                              surname=new_user_data.surname,
                              hashed_password=new_user_data.password,
                              email=new_user_data.email,
                              phone_number=new_user_data.phone_number)
    session.add(added_user)
    session.commit()
    return schemas.User(id=added_user.id,
                        name=added_user.name,
                        surname=added_user.surname,
                        email=added_user.email,
                        phone_number=added_user.phone_number)


def login_user(login_data: schemas.Login_data) -> Optional[models.User_]:
    found_user = session.query(models.User_).filter(and_(models.User_.email == login_data.email))
    if found_user.count() == 0:
        return None
    found_user = found_user.one()
    if not login_data.verify_password(found_user.hashed_password):
        return None
    return found_user


def verify_user(verify_data: schemas.Change_user_data, user_id: int) -> Optional[models.User_]:
    found_user = session.query(models.User_).filter(and_(models.User_.id == user_id))
    if found_user.count() == 0:
        return None
    found_user = found_user.one()
    if not verify_data.verify_password(found_user.hashed_password):
        return None
    return found_user


def change_user_data(user: models.User_, new_data: schemas.Change_user_data):
    if new_data.email is not None:
        if session.query(models.User_).filter(and_(models.User_.email == new_data.email)).count() > 0:
            return {"successful": False, "msg": "Почта занята"}
        user.email = new_data.email
    if new_data.phone_number is not None:
        if session.query(models.User_).filter(and_(models.User_.phone_number == new_data.phone_number)).count() > 0:
            return {"successful": False, "msg": "Номер телефона занят"}
        user.phone_number = new_data.phone_number

    session.commit()
    return {"successful": True, "user_data": schemas.User(id=user.id,
                                                          name=user.name,
                                                          surname=user.surname,
                                                          email=user.email,
                                                          phone_number=user.phone_number)}


def set_like(car_id: int, user_id: int):
    if has_like(user_id, car_id):
        return {"successful": False, "msg": "Такой лайк уже есть"}
    if session.query(models.Car_).filter(and_(models.Car_.id == car_id)).count() == 0:
        return {"successful": False, "msg": "Такого авто нет"}
    if session.query(models.User_).filter(and_(models.User_ .id == user_id)).count() == 0:
        return {"successful": False, "msg": "Такого пользователя нет"}

    new_like = models.Like_(id_user=user_id,
                            id_car=car_id)
    session.add(new_like)
    session.commit()
    return {"successful": True}


def get_user_likes(user_id: int, page_size: int, page: int) -> List[schemas.Car]:
    result = []
    offset = page_size * (page - 1)
    for i in session.query(models.Like_).filter(and_(models.Like_.id_user == user_id)).\
            limit(page_size).offset(offset):
        found_car = get_car(i.id_car)
        if found_car is not None:
            result.append(get_car(i.id_car))
    return result


def has_like(user_id: int, car_id: int):
    return get_like(user_id, car_id).count() > 0


def get_like(user_id: int, car_id: int):
    return session.query(models.Like_).filter(and_(models.Like_.id_user == user_id, models.Like_.id_car == car_id))


def delete_all_car_likes(car_id: int):
    for i in session.query(models.Like_).filter(and_(models.Like_.id_car == car_id)):
        session.delete(i)
    session.commit()


def delete_like(user_id: int, car_id: int):
    cur_like = get_like(user_id, car_id)
    if cur_like.count() == 0:
        return {"successful": False, "msg": "Такого лайка нет"}
    session.delete(cur_like.one())
    session.commit()
    return {"successful": True}


def delete_all_car_responses(car_id: int):
    for i in session.query(models.Response_).filter(and_(models.Response_.id_car == car_id)):
        session.delete(i)
    session.commit()
