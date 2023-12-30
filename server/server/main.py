from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles

from server.server.api.routes.car import router as car_router
from server.server.api.routes.user import router as user_router
from server.server.api.routes.like import router as like_router
from server.server.api.routes.responses import router as responses_router

app = FastAPI(docs_url="/")
app.mount("/static", StaticFiles(directory="server/server/static"), name="static")
app.include_router(car_router, prefix="/car")
app.include_router(user_router, prefix="/user")
app.include_router(like_router, prefix="/like")
app.include_router(responses_router, prefix="/response")


@app.get("/ping")
def ping():
    return "OK"
