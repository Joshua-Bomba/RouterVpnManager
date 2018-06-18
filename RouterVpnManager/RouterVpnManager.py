#this python script will only run in linux

#remote debugging tools

#pip install ptvsd
import ptvsd
ptvsd.enable_attach('RouterVpnManager')

import os
import json
import sys
import signal
import subprocess
import socket
import threading
import time


#This class will process any output from a subprocessHandler
class subprocessOutputHandler(threading.Thread):
    __PRINTSUBPROCESS = True
    __handler = None
    __outputCallback = None
    __output = True
    def __init__(self,outputCallback = None):
        threading.Thread.__init__(self)
        self.__outputCallback = outputCallback
    def addHandler(self,handler):
        self.__handler = handler
    def stop(self):
        self.__output = False
    def run(self):
        try:
            while (self.__PRINTSUBPROCESS or self.__outputCallback is not None) and self.__handler is not None and self.__output:
                line = self.__handler.stdout.readline()
                if line != '':
                    self.output(line)
                else:
                    break
        except Exception,e: 
            print str(e)
    def output(self,str):
        if self.__PRINTSUBPROCESS:
            sys.stdout.write(str)
        if self.__outputCallback is not None:
            self.__outputCallback(self,str)

#this handler class is used to control a single instance of a subprocess
class subprocessHandler:
    __lock = None
    __command = None
    __handler = None
    __processRequest = None
    __running = False
    __output = None
    def __init__(self,command,processRequest = None,outputCallback = None):
        self.__lock = threading.Lock()
        self.__command = command
        self.__processRequest = processRequest
        self.__output = subprocessOutputHandler(outputCallback)
        self.execute()
    def execute(self):
        self.__running = True
        try:
            self.__handler = subprocess.Popen(self.__command,stdout=subprocess.PIPE,shell=True,preexec_fn=os.setsid)
            self.__output.addHandler(self.__handler)
            self.__output.start()
        except:
            print "an issue has occured"
    def kill(self):
        self.__lock.acquire()
        try:
            if self.__running and self.__handler is not None:
                os.killpg(os.getpgid(self.__handler.pid),signal.SIGTERM)
                self.stop()
        finally:
            self.__lock.release()
    def stop(self):
        self.__running = False
        if self.__processRequest is not None:
            self.__processRequest.unexpectedDisconnect()
        if self.__output is not None:
            self.__output.stop()
    def isRunning(self):
        try:
            self.__lock.acquire()
            return self.__running
        finally:
            self.__lock.release()
    def checkStatus(self):
        self.__lock.acquire()
        try:
            if self.__running:
                retcode = self.__handler.poll()
                if(retcode is not None):
                    self.stop()
                    return False
            else:
                return False
        finally:
            self.__lock.release()

# this class is incharge on managing subprocess and creating new ones
class subprocessManager(threading.Thread):
    __processLock = None
    __process = []
    __stopProcessing = False
    def __init__(self):
        threading.Thread.__init__(self)
        self.__processLock = threading.Lock()
        self.start()
    def startProcess(self,command,processRequest):
        handler = None
        self.__processLock.acquire()
        try:
            handler = subprocessHandler(command,processRequest)
            self.__process.append(handler)
        finally:
            self.__processLock.release()
            return handler;
    def stop(self):
        try:
            self.__processLock.acquire()
            self.__stopProcessing = True
        finally:
            self.__processLock.release()
    def run(self):
        try:
            index = -1
            while not self.__stopProcessing:
                self.__processLock.acquire()
                try:
                    if len(self.__process) != 0:
                        if index < len(self.__process) - 1:
                            index = index + 1
                        else:
                            index = 0
                        if self.__process[index].checkStatus() == False:
                            del self.__process[index]
                            index = 0
                finally:
                    self.__processLock.release()
                time.sleep(.1)
            try:
                self.__processLock.acquire()
                for p in self.__process:
                    p.kill()
                del self.__process
            finally:
                self.__processLock.release()
        except Exception,e: 
            print str(e)

class routerVpnManager:   #TODO: this needs to down on the Connections level since there is only one vpn for all clients, well needs some locking
    __processManager = None
    __connectionStatus = None
    __currentConnection = None
    __processRequest = None#For Handling unexpected Disconnection of the Process
    VPN_CONNECTION_CODE = "openvpn "
    def __init__(self,processRequest):
        self.__processRequest = processRequest
        self.__processManager = subprocessManager()#When this is called an the code wants to exit ensure that this object is cleared up and the thread is stoped
    def exit(self):
        self.__processManager.stop()
        self.__processManager.join()
    def getOvpnFiles(self):
        path = os.path.dirname(os.path.realpath(__file__))
        vpnConnections = []
        for file in os.listdir(path):
            if file.endswith(".ovpn"):
                vpnConnections.append(file)
        return vpnConnections
    def isRunning(self):
        if self.__connectionStatus is not None and self.__connectionStatus.isRunning():
            return True
        else:
            return False
    def connectToVpn(self,str):
        files = self.getOvpnFiles()
        if str in files:
            if self.__connectionStatus is None or not self.__connectionStatus.isRunning():
                self.__connectionStatus = self.__processManager.startProcess(self.VPN_CONNECTION_CODE + str,self.__processRequest)
                self.__currentConnection = str
                return ""
            else:
                return "could not connect since it's already connect to a vpn"#TODO: could change this to a disconnect and reconnect sort of thing
        else:
            return "could not connect the VPN opvn file does not exist"
    def disconnectFromVpn(self):
        if self.__connectionStatus is not None and self.__connectionStatus.isRunning():
            self.__connectionStatus.kill()
            self.__connectionStatus = None
            return ""
        else: 
            return "could not disconnect since no vpn is connected"

    def getVpnConnection(self):
        if self.isRunning():
            return self.__currentConnection
        else:
            return ""

#this class will handle any socket request and respond to them
class processRequest:
    __stringJson = None
    __jsonObject = None
    __exception = ""
    __sock = None
    __vpnManager = None
    __connection = None
    __inputProcessLock = None
    def __init__(self):
        self.__vpnManager = routerVpnManager(self)
        self.__inputProcessLock = threading.Lock()
    def processInput(self,message,socket,connection):
        self.__inputProcessLock.acquire()
        try:
            self.__stringJson = message
            self.__sock = socket
            self.__connection = connection
            self.deseralizeJson()
            if(self.__jsonObject != None):
                return self.goThroughRequests()
            else:
                return False
        finally:
            self.__inputProcessLock.release()
    def exit(self):
        self.__vpnManager.exit()
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
        response["signature"] = self.__jsonObject["signature"]
        if(type == "response"):
            self.__sock.send(json.dumps(response))
    def unexpectedDisconnect(self):
        data = {}
        data["Status"] = ""
        data["Reason"] = "Unexpected Disconnection"
        self.__connection.sendBroadcast(self,"broadcast","disconnectfrompvpn",data)
    def goThroughRequests(self):
        if self.__jsonObject["type"] == "request":
            if self.__jsonObject["request"] == "connection":            
                self.sendResponse("response","connection","Connection Established")
                return True
            elif self.__jsonObject["request"] == "listovpn":
                self.sendResponse("response","listovpn",self.__vpnManager.getOvpnFiles())
                return True
            elif self.__jsonObject["request"] == "connecttovpn":
                data = {}
                data["VpnLocation"] = self.__jsonObject["data"][u'vpn']
                data["Status"] = self.__vpnManager.connectToVpn(data["VpnLocation"])
                self.sendResponse("response","connecttopvpn",data)
                self.__connection.sendBroadcast(self,"broadcast","connecttopvpn",data)
                return True
            elif self.__jsonObject["request"] == "disconnectfrompvpn":
                data = {}
                data["Reason"] = "Client Disconnected"
                data["Status"] = self.__vpnManager.disconnectFromVpn()
                self.__connection.sendBroadcast(self,"broadcast","disconnectfrompvpn",data)
            elif self.__jsonObject["request"] == "checkconnectionstatus":
                data = {}
                data["Running"] = self.__vpnManager.isRunning()
                data["ConnectedTo"] = self.__vpnManager.getVpnConnection()
                self.sendResponse("response","checkconnectionstatus",data)
                return True
            else:
                self.__exception = "The request does not exist"
        else:
            self.__exception = "Could not processs this type of request"
        return False
    def handleBroadcast(self,sender,response):
        #if(self != sender):#this may still deadlock when there is more then 1 broadcast at once, hummmm. There is only supposto be one client so i'll remove the lock and if it crashes, it crashes
            #self.__inputProcessLock.acquire()
            #try:
        if(response["type"] == "broadcast"):
            self.__sock.send(json.dumps(response));
            #finally:
                #self.__inputProcessLock.release()



#this will handle the socket connection for a paticular client
class client(threading.Thread):
    __connection = None
    __stopProcessing = False
    __request = None
    def __init__(self,socket,address,connection):
        threading.Thread.__init__(self)
        self.__connection = connection
        self.sock = socket
        self.add=address
        self.__request = processRequest()
        self.start()
    def stop(self):
        self.__stopProcessing = True
    def run(self):
        try:
            while not self.__stopProcessing:
                data = self.sock.recv(1024)
                if(data == ''):
                    break
                else:
                    print('client sent: ', data)
                    if (not self.__request.processInput(data,self.sock,self.__connection)):
                        self.sock.send('Messsage recived, could not process request: ', self.__request.getException())
        except Exception,e: 
            print str(e)
        finally:
            self.disconnect()
    def disconnect(self):
        print "client disconnected"
        self.__connection.disconnect(self)
        self.__request.exit()


#this handles all the connected clients
#this also handles any kind of broadcast
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
    def exit(self):
        for c in self.__clientsMap:
            c.stop()
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
    def sendBroadcast(self,sender,type,request,data):
        response = {}
        response["type"] = type
        response["request"] = request
        response["data"] = data
        if(type == "broadcast"):
            self.__clientsMapLock.acquire()
            try:
                for k in self.__clientsMap:
                    self.__clientsMap[k]._client__request.handleBroadcast(sender,response)
            except Exception,e: 
                print str(e)
            finally:
                self.__clientsMapLock.release()

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

#this method is just some basic code to parse the input parameters
def start():
    if len(sys.argv) >= 2:
        host = sys.argv[1]
        port = int(sys.argv[2])
        c = connections(host,port)
        c.listen();
    else:
        print("This script requires a host address and port")

#raw_input("Press Any Key Once The Debugger is hooked on")




start()#primary purpose of the above being in a method is because it makes it easier to comment out



#def testSubprocessManager():
#    print "ping started"

#    processManager = subprocessManager()

#    process = processManager.startProcess("ping 192.168.2.1",callback=finishedTask)

#    raw_input("Press any key to continue ")

#    process.kill()

#    processManager.stop()

#def finishedTask(handler):
#    print "ping finished"






