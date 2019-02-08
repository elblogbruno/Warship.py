# first of all import the socket library
import socket

# next create a socket object
s = socket.socket()
print ("Socket successfully created")

# reserve a port on your computer in our
# case it is 12345 but it can be anything
port = 8000
s.bind(('', port))
print ("socket binded to %s" %(port))

# put the socket into listening mode
s.listen(5)
print ("socket is listening")
# a forever loop until we interrupt it or
# an error occurs
c, addr = s.accept()
print ('Got connection from', addr)
word = 'Thank you for connecting'
   # send a thank you message to the client.
my_str_as_bytes = str.encode(word)
c.send(my_str_as_bytes)
data = c.recv(1024).decode()
while data != "close":
   # Establish connection with client.

   data = c.recv(1024).decode()
   print(data)
   # Close the connection with the client
c.close()
