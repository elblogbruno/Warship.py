from mqtt.socket_unity_communication import SocketUnity
from time import sleep
from random_word import RandomWords
r = RandomWords()

def test():
    socket = SocketUnity(None,None,None,None)
    socket.start_listening()
    sleep(5)
    #socket.publish_on_mqtt(w)
    while True:
        socket.publish_on_mqtt(r.get_random_word())
        sleep(1)

if __name__ == '__main__':
    test()