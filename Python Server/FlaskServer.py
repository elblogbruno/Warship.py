import logging
import threading
import os
from time import sleep
import asyncio
from threading import Thread

import atexit
import re
from concurrent.futures import ProcessPoolExecutor, ThreadPoolExecutor
from flask import Flask, request, redirect, url_for

from werkzeug.utils import secure_filename
from TelegramBot import *

import threading


import json
ALLOWED_EXTENSIONS = set(['txt', 'pdf', 'png', 'jpg', 'jpeg', 'gif'])

app = Flask(__name__)

def main():
    print("Starting Flask Server")
    app.run(port=5001,debug = True) 

@app.route('/sendimage', methods = ['GET', 'POST'])
def upload_file():
    file = request.files['image']
    # if user does not select file, browser also
    # submit a empty part without filename
    if file.filename == '':
        print('No selected file')
        return redirect(request.url)
    elif file.filename != '':
        filename = secure_filename(file.filename)+".jpg"
        print(filename)
        file.save(filename)
        bot.send_image(filename)
    return filename
@app.route("/sendmessage")
def sendmessage():
    q = request.args.get('text')
    bot.send_text(q)
    return q
@app.route("/askForPhoto")
def askForPhoto():
    bot.getNewUserNameAndProfile()
    q = "photo"
    return q
@app.route("/sendaudio")
def sendaudio():
    q = request.args.get('text')
    bot.send_audio(q)
    return q
@app.route("/getmessage")
def getmessage():
    # file1 = open("message.txt","r") 
    # q = file1.read() 
    with open('message.txt') as json_file:
        q = json.load(json_file)
    print("Getting message: "+ str(q))
    return q

@app.route('/')
def handle_request():
    return "Flask Server & Android are Working Successfully"

if __name__ == "__main__":
    main()