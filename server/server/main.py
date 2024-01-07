from fastapi import FastAPI, Request
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from fastapi.responses import HTMLResponse

from server.server.api.routes.car import router as car_router
from server.server.api.routes.user import router as user_router
from server.server.api.routes.like import router as like_router
from server.server.api.routes.responses import router as responses_router

templates = Jinja2Templates(directory="server/server/templates")

app = FastAPI(docs_url="/")
app.mount("/static", StaticFiles(directory="server/server/static"), name="static")
app.include_router(car_router, prefix="/car")
app.include_router(user_router, prefix="/user")
app.include_router(like_router, prefix="/like")
app.include_router(responses_router, prefix="/response")


@app.get("/ping")
def ping():
    return "OK"


@app.get("/recaptcha_form")
def recaptcha_form(request: Request):
    return templates.TemplateResponse(name="recaptcha.html", context={"request": request})
