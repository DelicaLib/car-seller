from datetime import datetime
from decimal import Decimal

from sqlalchemy import create_engine, Column, Integer, Boolean, String, ForeignKey, Date, Float, Text, Sequence
from sqlalchemy.dialects.postgresql import VARCHAR, CHAR, MONEY
from sqlalchemy.orm import DeclarativeBase

from server.server.core.config import DB_HOST, DB_PASS, DB_NAME, DB_PORT, DB_USER, ECHO_DATABASE_LOG

engine = create_engine(f"postgresql://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}", echo=ECHO_DATABASE_LOG)


class Base(DeclarativeBase):
    pass


class User_(Base):
    __tablename__ = "user_"

    id: int = Column("id", Integer, Sequence("user__id_seq", start=1, increment=1), primary_key=True)
    name: str = Column("name", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    surname: str = Column("surname", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    hashed_password: str = Column("hashed_password", String(1024).with_variant(VARCHAR(1024), "postgresql"),
                                  nullable=False)
    email: str = Column("email", String(100).with_variant(VARCHAR(100), "postgresql"), nullable=False, unique=True)
    phone_number: str = Column("phone_number", String(10).with_variant(CHAR(10), "postgresql"), nullable=False,
                               unique=True)

    def __repr__(self) -> str:
        return f"id: {self.id}   name:{self.name}"


class Car_(Base):
    __tablename__ = "car_"

    id: int = Column("id", Integer, Sequence("car__id_seq", start=1, increment=1), primary_key=True)
    brand: str = Column("brand", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    model: str = Column("model", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    city: str = Column("city", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    mileage: int = Column("mileage", Integer, nullable=False)
    transmission: str = Column("transmission", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    engine: str = Column("engine", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    body: str = Column("body", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    release_year: int = Column("release_year", Integer, nullable=False)
    drive: str = Column("drive", String(50).with_variant(VARCHAR(50), "postgresql"), nullable=False)
    cost: Decimal = Column("cost", Float().with_variant(MONEY, "postgresql"), nullable=False)
    volume: int = Column("volume", Integer, nullable=False)
    description: str = Column("description", Text, nullable=True)
    date_publish: datetime = Column("date_publish", Date, nullable=False)
    photos: str = Column("photos", String(1024).with_variant(VARCHAR(1024), "postgresql"), nullable=False)
    id_owner: int = Column("id_owner", Integer, ForeignKey("user_.id"))

    def __repr__(self) -> str:
        return f"id: {self.id}   brand:{self.brand}"


class Like_(Base):
    __tablename__ = "like_"

    id: int = Column("id", Integer, Sequence("like__id_seq", start=1, increment=1), primary_key=True)
    id_user: int = Column("id_user", Integer, ForeignKey("user_.id"), nullable=False)
    id_car: int = Column("id_car", Integer, ForeignKey("car_.id"), nullable=False)

    def __repr__(self) -> str:
        return f"id_car: {self.id_car}   id_user:{self.id_user}"


class Response_(Base):
    __tablename__ = "response_"

    id: int = Column("id", Integer, Sequence("response__id_seq", start=1, increment=1), primary_key=True)
    id_user: int = Column("id_user", Integer, ForeignKey("user_.id"), nullable=False)
    id_car: int = Column("id_car", Integer, ForeignKey("car_.id"), nullable=False)
    id_owner: int = Column("id_owner", Integer, ForeignKey("user_.id"), nullable=False)
    is_claim: bool = Column("is_claim", Boolean, nullable=False, default=False)
    message: str = Column("message", Text, nullable=True)

    def __repr__(self) -> str:
        return f"id_car: {self.id_car}   id_user:{self.id_user}, message: {self.message}"
