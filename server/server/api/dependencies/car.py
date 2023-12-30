from fastapi import UploadFile
from typing import List
from os import mkdir as os_mkdir
from shutil import copyfileobj
from re import split as re_split
from datetime import datetime


async def is_correct_files(files: List[UploadFile]):
    if len(files) == 0:
        return dict(correct=False,
                    msg="Минимальное количество файлов - 1")
    if len(files) > 10:
        return dict(correct=False,
                    msg="Максимальное количество файлов - 10")
    for file in files:
        file.file.seek(0, 2)
        file_size = file.file.tell()
        await file.seek(0)
        if file_size > 4 * 1024 * 1024:
            return dict(correct=False,
                        msg="Файл слишком большой")
        content_type = file.content_type
        if content_type not in ["image/jpeg", "image/png"]:
            return dict(correct=False,
                        msg="Неверный тип файла")
    return dict(correct=True,
                msg="Ок")


async def upload_files(files: List[UploadFile]):
    file_paths = []
    id_owner = 1
    folder_name = f"{id_owner}-{'+'.join(re_split('[ :]', str(datetime.now())))}"
    os_mkdir(f"static/{folder_name}")
    start_path = f"static/{folder_name}/"
    for file in files:
        path = start_path + f'{file.filename}'
        file_paths.append(path)
        with open(path, "wb+") as buffer:
            copyfileobj(file.file, buffer)
    return file_paths
