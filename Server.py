import socket

serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
host = socket.gethostname()
port = 8000
last_data = " "
print (host)
print (port)
serversocket.bind((host, port))

serversocket.listen(5)
print ('server started and listening')
while True:
    (clientsocket, address) = serversocket.accept()
    print ("connection found!")
    data = clientsocket.recv(1024).decode()
    if(data):
        if(data == last_data):
            print("Nothing changed")
        else:
            last_data = data
            print (data)
    #r=data
    #clientsocket.send(r.encode())
