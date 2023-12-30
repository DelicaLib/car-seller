import requests
import jwt

from server.server.api.models import schemas
from server.server.core.config import JWT_KEY


def verify_recaptcha(recaptcha_response: str, secret_key: str, remote_ip: str) -> bool:
    payload = {
        'response': recaptcha_response,
        'secret': secret_key,
        'remoteip': remote_ip
    }

    response = requests.post('https://www.google.com/recaptcha/api/siteverify', data=payload)
    result = response.json()
    if result['success']:
        return True
    else:
        return False


def delete_confidential_data(user: schemas.User):
    user.email = None
    user.phone_number = None


def generate_token(user: schemas.User) -> dict:
    return {
        "access_token": jwt.encode(
            {"full_name": f"{user.surname} {user.name}",
             "id": user.id},
            JWT_KEY
        )
    }


def decode_jwt(jwt_token: dict):
    try:
        decoded_token = jwt.decode(jwt_token["access_token"], JWT_KEY, algorithms=['HS256'])
        return decoded_token.get("id")
    except jwt.ExpiredSignatureError:
        return None


