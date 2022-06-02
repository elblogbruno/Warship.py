from threading import Thread

from flask import Flask, request, redirect

from werkzeug.utils import secure_filename

ALLOWED_EXTENSIONS = set(['txt', 'pdf', 'png', 'jpg', 'jpeg', 'gif'])

app = Flask(__name__)
bot = None


class GameServer:
    def __init__(self, tel_bot):
        global bot
        bot = tel_bot

    def start_listening(self):
        thread = Thread(target=self.main)
        thread.daemon = True
        thread.start()

    def is_connected(self):
        return True

    def main(self):
        print("Starting Flask Server")
        app.use_reloader = False
        app.run(port=5001)


@app.route('/send_image', methods=['GET', 'POST'])
def upload_file():
    file = request.files['image']
    # if user does not select file, browser also
    # submit a empty part without filename
    if file.filename == '':
        print('No selected file')
        return redirect(request.url)
    elif file.filename != '':
        filename = secure_filename(file.filename) + ".jpg"
        print(filename)
        file.save(filename)
        bot.send_image(filename)
    return filename


@app.route("/send_message")
def send_message():
    q = request.args.get('text')
    bot.filter_text(q)
    return q


@app.route("/send_audio")
def send_audio():
    q = request.args.get('text')
    bot.send_audio(q)
    return q


@app.route("/get_bot_status")
def get_bot_status():
    return str(bot.get_status())


@app.route('/')
def handle_request():
    return "Flask Server & Android are Working Successfully"
