from dotenv import load_dotenv
import os

load_dotenv()

DB_USER = os.environ.get("DB_USER")
DB_PASS = os.environ.get("DB_PASS")
DB_HOST = os.environ.get("DB_HOST")
DB_PORT = os.environ.get("DB_PORT")
DB_NAME = os.environ.get("DB_NAME")
JWT_KEY = os.environ.get("JWT_KEY")
PASSWORD_KEY = os.environ.get("PASSWORD_KEY")
RECAPTCHA_SECRET_KEY = os.environ.get("RECAPTCHA_SECRET_KEY")

ECHO_DATABASE_LOG = True
