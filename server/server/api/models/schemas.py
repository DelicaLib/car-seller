from pydantic import BaseModel, Field, field_validator
from enum import Enum
from decimal import Decimal
from datetime import datetime
from typing import List, Optional
import bcrypt
import re
from email_validator import validate_email, EmailNotValidError

from server.server.api.models.models import Car_


class Transmission(Enum):
    mechanic = "механическая"
    automate = "автоматическая"

    def __str__(self):
        return str(self.value)


class Engine(Enum):
    gas = "газовый"
    electric = "электрический"
    diesel = "дизельный"
    gasoline = "бензиновый"

    def __str__(self):
        return str(self.value)


class Drive(Enum):
    back = "задний"
    front = "передний"
    full = "полный"

    def __str__(self):
        return str(self.value)


class Body(Enum):
    sedan = "седан"
    sedan_two_door = "двухдверный седан"
    universal = "универсал"
    hatchback = "хэтчбек"
    coupe = "купе"
    limousine = "лимузин"
    minibus = "микроавтобус"
    minivan = "минивэн"
    hardtop = "хардтоп"
    tawncar = "таункар"
    combi = "комби"
    liftback = "лифтбэк"
    fastback = "фастбэк"
    cabriolet = "кабриолет"
    roadster = "родстер"
    phaeton = "фаэтон"
    lando = "ландо"
    brogam = "брогам"
    targa = "тарга"
    spider = "спайдер"
    shootingbrake = "шутингбрейк"
    pickup = "пикап"
    ut = "ют"
    van = "фургон"
    offroad = "внедорожник"

    def __str__(self):
        return str(self.value)


class Id(BaseModel):
    id: int = Field(ge=1)


class Login_data(BaseModel):
    email: str = Field(max_length=100)
    password: str

    def verify_password(self, hashed_password: str):
        try:
            return bcrypt.checkpw(password=self.password.encode(), hashed_password=hashed_password.encode())
        except ValueError:
            return None


class New_user(BaseModel):
    name: str = Field(max_length=50)
    surname: str = Field(max_length=50)
    password: str
    email: str
    phone_number: str = Field(min_length=10, max_length=10)

    @field_validator("password")
    def password_validate(cls, password: str):
        if len(password) < 8:
            raise ValueError('Пароль должен состоять как минимум из 8 символов')
        regex_lowercase = re.compile(r'[a-z]')
        regex_uppercase = re.compile(r'[A-Z]')
        regex_digit = re.compile(r'[0-9]')
        regex_special = re.compile(r'[\W_]')

        if not regex_lowercase.search(password):
            raise ValueError('Пароль должен содержать строчные латинские буквы')
        if not regex_uppercase.search(password):
            raise ValueError('Пароль должен содержать заглавные латинские буквы')
        if not regex_digit.search(password):
            raise ValueError('Пароль должен содержать цифры')
        if not regex_special.search(password):
            raise ValueError('Пароль должен содержать специальные символы')
        hashed_password = bcrypt.hashpw(password.encode(), bcrypt.gensalt()).decode()
        if len(hashed_password) > 1024:
            raise ValueError('Пароль слишком длинный')
        return hashed_password

    @field_validator("email")
    def email_validate(cls, email):
        try:
            validate_email(email)
        except EmailNotValidError:
            raise ValueError("Неверный адрес электронной почты")
        if len(email) > 100:
            raise ValueError("Слишком длинный адрес электронной почты")
        return email


class Change_user_data(Login_data):
    email: Optional[str]
    phone_number: Optional[str] = Field(min_length=10, max_length=10)

    @field_validator("email")
    def email_validate(cls, email):
        if email is None:
            return email
        try:
            validate_email(email)
        except EmailNotValidError:
            raise ValueError("Неверный адрес электронной почты")
        if len(email) > 100:
            raise ValueError("Слишком длинный адрес электронной почты")
        return email


class User(Id):
    name: str
    surname: str
    email: Optional[str]
    phone_number: Optional[str] = Field(min_length=10, max_length=10)


class User_JWT_Token(User):
    jwt_token: dict


class New_car(BaseModel):
    brand: str = Field(max_length=50)
    model: str = Field(max_length=50)
    city: str = Field(max_length=50)
    mileage: int = Field(ge=0)
    transmission: Transmission
    engine: Engine
    body: Body
    release_year: int = Field(ge=0)
    drive: Drive
    cost: Decimal
    volume: int = Field(ge=0)
    description: str
    photos: List[str]


class Car(New_car, Id):
    date_publish: datetime
    id_owner: int


class Get_car_response(BaseModel):
    car: Car
    owner: User


class Response_info(Id):
    car: Car
    message: Optional[str]
    is_claim: bool


class Response_owner_info(Response_info):
    user: User


class Responses_info(BaseModel):
    responses_owner: List[Response_owner_info]
    responses_user: List[Response_info]


class New_Response_data(BaseModel):
    car_id: int
    message: Optional[str]


class Response(Id):
    user: User
    car: Car
    message: Optional[str]


class Filter_max_min(BaseModel):
    max_volume: int = 99999999999999
    min_volume: int = 0

    @field_validator('min_volume')
    def check_min_volume(cls, v, values):
        if 'max_volume' in values.data and v >= values.data['max_volume']:
            raise ValueError('min_volume должен быть меньше, чем max_volume')
        if v > 99999999999999:
            raise ValueError('min_volume должен быть меньше, чем 99999999999999')
        if v < 0:
            raise ValueError('min_volume должен быть больше, чем 0')
        return v

    max_cost: int = 99999999999999
    min_cost: int = 0

    @field_validator('min_cost')
    def check_min_cost(cls, v, values):
        if 'max_cost' in values.data and v >= values.data['max_cost']:
            raise ValueError('min_cost должен быть меньше, чем max_cost')
        if v > 99999999999999:
            raise ValueError('min_cost должен быть меньше, чем 99999999999999')
        if v < 0:
            raise ValueError('min_cost должен быть больше, чем 0')
        return v

    max_mileage: int = 99999999999999
    min_mileage: int = 0

    @field_validator('min_mileage')
    def check_min_mileage(cls, v, values):
        if 'max_mileage' in values.data and v >= values.data['max_mileage']:
            raise ValueError('min_mileage должен быть меньше, чем max_mileage')
        if v > 99999999999999:
            raise ValueError('min_mileage должен быть меньше, чем 99999999999999')
        if v < 0:
            raise ValueError('min_mileage должен быть больше, чем 0')
        return v

    max_release_year: int = 3000
    min_release_year: int = 0

    @field_validator('min_release_year')
    def check_min_release_year(cls, v, values):
        if 'max_release_year' in values.data and v >= values.data['max_release_year']:
            raise ValueError('min_release_year должен быть меньше, чем max_release_year')
        if v > 3000:
            raise ValueError('min_release_year должен быть меньше, чем 3000')
        if v < 0:
            raise ValueError('min_release_year должен быть больше, чем 0')
        return v


class Filters(BaseModel):
    city: Optional[List[str]]
    brand: Optional[List[str]]
    model: Optional[List[str]]
    body: Optional[List[Body]]
    transmission: Optional[List[Transmission]]
    engine: Optional[List[Engine]]
    drive: Optional[List[Drive]]


class Filter(Filter_max_min):
    city: Optional[str]
    brand: Optional[str]
    model: Optional[str]
    body: Optional[Body]
    transmission: Optional[Transmission]
    engine: Optional[Engine]
    drive: Optional[Drive]

    def get_dict(self):
        params = {
            "city": Car_.city,
            "brand": Car_.brand,
            "model": Car_.model,
            "body": Car_.body,
            "transmission": Car_.transmission,
            "engine": Car_.engine,
            "drive": Car_.drive,
            "min_cost": str(self.min_cost),
            "max_cost": str(self.max_cost),
            "min_volume": self.min_volume,
            "max_volume": self.max_volume,
            "min_mileage": self.min_mileage,
            "max_mileage": self.max_mileage,
            "min_release_year": self.min_release_year,
            "max_release_year": self.max_release_year
        }
        if self.city is not None:
            params["city"] = self.city
        if self.brand is not None:
            params["brand"] = self.brand
        if self.model is not None:
            params["model"] = self.model
        if self.body is not None:
            params["body"] = str(self.body)
        if self.transmission is not None:
            params["transmission"] = str(self.transmission)
        if self.engine is not None:
            params["engine"] = str(self.engine)
        if self.drive is not None:
            params["drive"] = str(self.drive)
        return params


class RecaptchaVerification(BaseModel):
    recaptcha_response: str
