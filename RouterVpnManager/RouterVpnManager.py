import sys
import subprocess
import socket
from threading import *


class client(Thread):
    def __init__(self,socket,address):
        Thread.__init__(self)
        self.sock = socket
        self.add=address
        print('Connection recived from client: ')
        self.start()
    def run(self):
        while 1:
            print('client sent: ', self.sock.recv(1024).decode())
            self.sock.send(b'Messsage recived')


class connections:
    __host = None
    __port = 0
    __serversocket = None;
    __clientsMap = {}
    def __init__(self,host,port):
        self.__host = host
        self.__port = port
        self.bind()
    def bind(self):
        print 'Port Binded'
        self.__serversocket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        self.__serversocket.bind((self.__host,self.__port))
    def listen(self):
        print'server started and listening'
        self.__serversocket.listen(5)
        while 1:
            clientsocket, address = self.__serversocket.accept()
            client(clientsocket,address)
            self.__clientMap[address[1]]



if len(sys.argv) >= 2:
    host = sys.argv[1]
    port = int(sys.argv[2])
    c = connections(host,port)
    c.listen();
    

else:
    print("This script requires a host address and port")

