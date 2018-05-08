import sys
import subprocess
import socket
from threading import *


class clientconnection(Thread):
    def __init__(self,socket,address):
        Thread.__init__(self)
        self.sock = socket
        self.add=address
        self.start()
    def run(self):
        while 1:
            print('client sent: ', self.sock.recv(1024).decode())
            self.sock.send(b'Messsage recived')




if len(sys.argv) >= 2:
    host = sys.argv[1]
    port = int(sys.argv[2])
    serversocket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
    serversocket.bind((host,port))
    
    serversocket.listen(5)
    print('server started and listening')
    while 1:
        clientsocket, address = serversocket.accept()
        clientconnection(clientsocket,address)

else:
    print("This script requires a host address and port")

