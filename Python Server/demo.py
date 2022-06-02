from flask import Flask
import threading

data = 'foo'
app = Flask(__name__)

@app.route("/")
def main():
    return data

def flaskThread():
    app.run()

if __name__ == "__main__":
    threading.Thread(target=app.run).start()