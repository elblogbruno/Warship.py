import BotDemo as bot
import os
from flask import Flask, request, redirect, url_for
from werkzeug.utils import secure_filename
import GameHelper
import threading
ALLOWED_EXTENSIONS = set(['txt', 'pdf', 'png', 'jpg', 'jpeg', 'gif'])

app = Flask(__name__)


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

@app.route("/getmessage")
def getmessage():
    q = bot.get_user_text()
    return q

@app.route('/', methods=['GET', 'POST'])
def handle_request():
    return "Flask Server & Android are Working Successfully"

if __name__ == '__main__':
    bot.setBotToken("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk")
    app.run(port=5001, debug=True)