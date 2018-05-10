import json
import sys
import subprocess
import socket
import threading


class processRequest:
    __processed = False
    __stringJson = None
    __jsonObject = None
    __exception = ""
    __sock = None
    def __init__(self,message,socket):
        self.__stringJson = message
        self.__sock = socket
        self.deseralizeJson()
        if(self.__jsonObject != None):
            self.goThroughRequests()
    def requestProgressedSucessfully(self):
        return self.__processed
    def getException(self):
        return self.__exception
    def deseralizeJson(self):
        try:
            data = json.loads(self.__stringJson)
            if 'request' not in data or 'data' not in data or 'type' not in data:
                raise ValueError("Missing keys from the json")
            self.__jsonObject = data
        except Exception, e:
            self.__exception = e.message
            print(e.message)
    def sendResponse(self,type,request,data):
        response = {}
        response["type"] = type
        response["request"] = request
        response["data"] = data
        if(type == "response"):
            self.__sock.send(json.dumps(response))
    def goThroughRequests(self):
        if self.__jsonObject["type"] == "request":
            if self.__jsonObject["request"] == "connection":            
                self.sendResponse("response","","Connection Established")
                self.__processed = True
            else:
                self.__exception = "The request does not exist"
        else:
            self.__exception = "Could not processs this type of request"

class client(threading.Thread):
    __connection = None
    def __init__(self,socket,address,connection):
        threading.Thread.__init__(self)
        self.__connection = connection
        self.sock = socket
        self.add=address
        self.start()
    def run(self):
        while 1:
            data = self.sock.recv(1024)
            if(data == ''):
                self.disconnect()
                break
            else:
                print('client sent: ', data)
                request = processRequest(data,self.sock)
                if (not request.requestProgressedSucessfully()):
                    self.sock.send('Messsage recived, could not process request: ', request.getException())
    def disconnect(self):
        print "client disconnected"
        self.__connection.disconnect(self)



class connections:
    __host = None
    __port = 0
    __serversocket = None;
    __clientsMap = {}
    __clientsMapLock = None
    def __init__(self,host,port):
        self.__host = host
        self.__port = port
        self.__clientsMapLock = threading.Lock()
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
            self.connect(clientsocket,address)
           
    def connect(self,clientsocket,address):
        print("connecting to  %s:%d" % (address[0],address[1]))
        self.__clientsMapLock.acquire()
        try:
            self.__clientsMap[address[1]] = client(clientsocket,address,self)
        finally:
            self.__clientsMapLock.release()
    def disconnect(self,client):
        self.__clientsMapLock.acquire()
        try:
            del self.__clientsMap[client.add[1]]
        finally:
            self.__clientsMapLock.release()


def start():
    if len(sys.argv) >= 2:
        host = sys.argv[1]
        port = int(sys.argv[2])
        c = connections(host,port)
        c.listen();
    else:
        print("This script requires a host address and port")
start()