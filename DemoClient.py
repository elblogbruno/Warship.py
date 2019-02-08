import socket               # Import socket module

s = socket.socket()         # Create a socket object
host = socket.gethostname() # Get local machine name
port = 8000                # Reserve a port for your service.

s.connect((host, port))
data = s.recv(1024).decode()
while data != "close":
   # Establish connection with client.
   data = s.recv(1024).decode()
   print(data)
   # Close the connection with the client
s.close()
