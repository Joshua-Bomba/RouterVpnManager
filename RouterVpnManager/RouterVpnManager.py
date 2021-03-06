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
import shutil
import Queue


CONFIG_FOLDER_NAME = "configurations"
OPENVPNCL_PATH = "/tmp/openvpncl" #I highly recommend against changing this
VPN_CONNECTION_CODE = "openvpn --route-up " + OPENVPNCL_PATH + "/route-up.sh --route-pre-down " + OPENVPNCL_PATH +"/route-down.sh --config " + OPENVPNCL_PATH +"/openvpn.conf"

#raw_input("Press Any Key Once The Debugger is hooked on")

class logger(threading.Thread):
    __connections = None
    __output = None
    #__outputQueue = None
    def __init__(self):
        threading.Thread.__init__(self)
        self.__output = True
        #self.__outputQueue = Queue.Queue()
        #self.start()#whoops forgot to start it https://youtu.be/KIrCOfDbL_E?t=1m44s
    def setConnections(self,connections):
        self.__connections = connections
        #self.writeLine("Broadcast Logging has now began")
    def write(self,outputString):
        if not isinstance(outputString, basestring):
            outputString = str(outputString)
        #self.__outputQueue.put(outputString)
        sys.stdout.write(outputString)
    def writeLine(self,outputString):
        if not isinstance(outputString, basestring):
            outputString = str(outputString)
        outputString += "\n"
        self.write(outputString)
    def run(self):
        #try:
        #    while self.__output:
        #        str = self.__outputQueue.get()
        #        sys.stdout.write(str)
        #        if self.__connections is not None:
        #            m = {}
        #            m["message"] = str
        #            self.__connections.sendBroadcast("broadcast","broadcastlog",m,"")
        #except Exception, e:
        #    print "Queue Exception Exiting log" + str(e)#probably not a good idea to broadcast this exception out
        pass
    def stop(self):
        self.__output = False

log = logger()



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
            log.writeLine(str(e))
    def output(self,str):
        if self.__PRINTSUBPROCESS:
            log.write(str)
        if self.__outputCallback is not None:
            self.__outputCallback(self,str)

#this handler class is used to control a single instance of a subprocess
class subprocessHandler:
    __lock = None
    __command = None
    __handler = None
    __connections = None
    __running = False
    __output = None
    def __init__(self,command,connections = None,outputCallback = None):
        self.__lock = threading.Lock()
        self.__command = command
        self.__connections = connections
        self.__output = subprocessOutputHandler(outputCallback)
        self.execute()
    def execute(self):
        self.__running = True
        try:
            self.__handler = subprocess.Popen(self.__command,stdout=subprocess.PIPE,shell=True,preexec_fn=os.setsid)
            self.__output.addHandler(self.__handler)
            self.__output.start()
        except:
            log.writeLine("an issue has occured")
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
        if self.__connections is not None:
            self.__connections.vpnUnexpectedDisconnectionBroadcast()	
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
    def wait(self):
        self.__handler.wait()

# this class is incharge on managing subprocess and creating new ones
class subprocessManager(threading.Thread):
    __processLock = None
    __process = []
    __stopProcessing = False
    def __init__(self):
        threading.Thread.__init__(self)
        self.__processLock = threading.Lock()
        self.start()
    def startProcess(self,command,connections):
        handler = None
        self.__processLock.acquire()
        try:
            handler = subprocessHandler(command,connections)
            self.__process.append(handler)
        finally:
            self.__processLock.release()
            return handler;
    def stop(self):
        try:
            self.__processLock.acquire()
            self.__stopProcessing = True
            for p in self.__process:
                p.kill()
            del self.__process
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
        except Exception,e: 
            log.writeLine( str(e))


class vpnFileManager:
    __path = None
    __processManager = None
    def __init__(self,path,processManager):
        try:
            if os.path.isdir(path):
                configPath = path + "/" + CONFIG_FOLDER_NAME
                if not os.path.isdir(configPath):
                    os.makedirs(configPath)
                self.__path = configPath
            self.__processManager = processManager
        except Exception, e:
            log.writeLine(e)
    def getAvaliableConnections(self):
        vpnConnections = []
        try:

            for file in os.listdir(self.__path):
                if os.path.isdir(os.path.join(self.__path,file)):
                    if self.folderValid(os.path.join(self.__path,file)):
                        vpnConnections.append(file)
        except Exception, e:
           log.writeLine(e)
        finally:
            return vpnConnections
    def configExists(self,name):
        return name in self.getAvaliableConnections()
    def folderValid(self,path):
        try:
            return os.path.isdir(path) and os.path.isfile(path + "/openvpn.conf") and os.path.isfile(path + "/route-up.sh") and os.path.isfile(path + "/route-down.sh")
        except Exception, e:
            log.writeLine(e)
            return False
    def pathValid(self):
        return self.__path != None
    # copys the current openvpncl config to a new folder under the configuration folder
    def copyCurrentConfig(self,name):
        try:
            if not os.path.isdir(CONFIG_FOLDER_NAME + "/" + name):
                if self.folderValid(OPENVPNCL_PATH):
                    os.makedirs(CONFIG_FOLDER_NAME + "/" + name)
                    for f in os.listdir(OPENVPNCL_PATH):
                        shutil.copyfile(OPENVPNCL_PATH + "/" + f,CONFIG_FOLDER_NAME + "/" + name + "/" + f)
                    return ""
                else:
                    return "No configuration found"
            else:
                return "Configuration already exists"
        except Exception, e:
            log.writeLine(e)
            return "unhandle exception"

    # this will clear the current openvpncl 
    def clearCurrentConfig(self):
        try:
            if os.path.isdir(OPENVPNCL_PATH):
                shutil.rmtree(OPENVPNCL_PATH)
            else:
                return "no config to clear"
        except Exception, e:
            log.writeLine(e)
            return "unhandle exception"
    # this will copy the current config file to the openvpncl folder
    def copyConfig(self,folderName):
        try:
            if not os.path.isdir(OPENVPNCL_PATH):
                os.makedirs(OPENVPNCL_PATH)
            for f in os.listdir(CONFIG_FOLDER_NAME + "/" + folderName):
                shutil.copyfile(CONFIG_FOLDER_NAME + "/" + folderName + "/" + f,OPENVPNCL_PATH + "/" + f)
            return self.applyPermissions()
        except Exception, e:
            log.writeLine(e)
            return "unhandle exception"
    def deleteConfig(self,folderName):
        try:
            if folderName != "" and os.path.isdir(CONFIG_FOLDER_NAME + "/" + folderName):
                shutil.rmtree(CONFIG_FOLDER_NAME + "/" + folderName)      
            else:
                return "could not find config"
        except Exception, e:
            log.writeLine(e)
            return "unhandle exception"
    def applyPermissions(self):
        if self.folderValid(OPENVPNCL_PATH):
            try:
                chmod1 = self.__processManager.startProcess("chmod +x " + OPENVPNCL_PATH + "/route-up.sh", None)
                chmod2 = self.__processManager.startProcess("chmod +x " + OPENVPNCL_PATH + "/route-down.sh", None)
                chmod1.wait()
                chmod2.wait()
            except:
                return "There was an issue applying the required permissions to the config"
        else:
            return "There was an issue with the config"


class routerVpnManager:
    __processManager = None
    __connectionStatus = None
    __currentConnection = None
    __vpnFileManager = None
    __connections = None#For Handling unexpected Disconnection of the Process
    __lock = None
    def __init__(self,connections):
        self.__connections = connections
        self.__lock = threading.Lock()
        self.__processManager = subprocessManager()#When this is called an the code wants to exit ensure that this object is cleared up and the thread is stoped
        self.__vpnFileManager = vpnFileManager(os.path.dirname(os.path.realpath(__file__)),self.__processManager)
    def exit(self):
        if self.__processManager is not None:
            self.__processManager.stop()
    def getConnectionNames(self):#Async
        return self.__vpnFileManager.getAvaliableConnections()
    def isRunning(self):
        self.__lock.acquire()
        try:
            return self.isRunningInternal()
        finally:
            self.__lock.release()
    def isRunningInternal(self):
        if self.__connectionStatus is not None and self.__connectionStatus.isRunning():
            return True
        else:
            return False
    def connectToVpn(self,str):
        files = self.getConnectionNames()
        self.__lock.acquire()
        try:
            if str in files:
                if not self.isRunningInternal():
                    if self.__vpnFileManager.configExists(str):
                        self.__vpnFileManager.clearCurrentConfig()
                        self.__vpnFileManager.copyConfig(str)
                        self.__connectionStatus = self.__processManager.startProcess(VPN_CONNECTION_CODE,self.__connections)
                        self.__currentConnection = str
                        return ""
                else:
                    return "could not connect since it's already connect to a vpn"#TODO: could change this to a disconnect and reconnect sort of thing
            else:
                return "could not connect the VPN opvn file does not exist"
        finally:
            self.__lock.release()
    def disconnectFromVpn(self):
        self.__lock.acquire()
        try:
            if self.isRunningInternal():
                self.__connectionStatus.kill()
                self.__connectionStatus = None
                return ""
            else: 
                return "could not disconnect since no vpn is connected"
        finally:
            self.__lock.release()
    def getVpnConnection(self):
        self.__lock.acquire()
        try:
            if self.isRunningInternal():
                return self.__currentConnection
            else:
                return ""
        finally:
            self.__lock.release()

#this class will handle any socket request and respond to them
class processRequest:
    __stringJson = None
    __jsonObject = None
    __exception = ""
    __sock = None
    __vpnManager = None
    __connection = None
    __inputProcessLock = None
    def __init__(self,vpnManager):
        self.__vpnManager = vpnManager
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
            log.writeLine(e.message)
    def sendResponse(self,type,request,data):
        response = {}
        response["type"] = type
        response["request"] = request
        response["data"] = data
        response["signature"] = self.__jsonObject["signature"]
        if(type == "response"):
            self.__sock.send(json.dumps(response))
    def goThroughRequests(self):
        if self.__jsonObject["type"] == "request":
            if self.__jsonObject["request"] == "connection":            
                self.sendResponse("response","connection","Connection Established")
                return True
            elif self.__jsonObject["request"] == "listovpn":
                self.sendResponse("response","listovpn",self.__vpnManager.getConnectionNames())
                return True
            elif self.__jsonObject["request"] == "connecttovpn":
                data = {}
                data["vpnLocation"] = self.__jsonObject["data"][u'vpn']
                data["status"] = self.__vpnManager.connectToVpn(data["vpnLocation"])
                #self.sendResponse("response","connecttovpn",data)
                self.__connection.sendBroadcast("broadcast","connecttovpn",data,self.__jsonObject["signature"])
                return True
            elif self.__jsonObject["request"] == "disconnectfrompvpn":
                data = {}
                data["reason"] = "Client Disconnected"
                data["status"] = self.__vpnManager.disconnectFromVpn()#The disconnect from vpn will be callback twice because it shuts down the Subprocess and causes and unexpected disconnect
                self.__connection.sendBroadcast("broadcast","disconnectfrompvpn",data,self.__jsonObject["signature"])
                return True
            elif self.__jsonObject["request"] == "checkconnectionstatus":
                data = {}
                data["running"] = self.__vpnManager.isRunning()
                data["connectedTo"] = self.__vpnManager.getVpnConnection()
                self.sendResponse("response","checkconnectionstatus",data)
                return True
            elif self.__jsonObject["request"] == "copycurrentconfig":
                data = {}
                data["status"] = self.__vpnManager._routerVpnManager__vpnFileManager.copyCurrentConfig(self.__jsonObject["data"][u'name'])
                self.sendResponse("response","copycurrentconfig",data)
                return True
            elif self.__jsonObject["request"] == "deleteconfig":
                data = {}
                data["status"] = self.__vpnManager._routerVpnManager__vpnFileManager.deleteConfig(self.__jsonObject["data"][u'name'])
                self.sendResponse("response","deleteconfig",data)
                return True
            elif self.__jsonObject["request"] == "clearcurrentconfig":
                data = {}
                data["status"] = self.__vpnManager._routerVpnManager__vpnFileManager.clearCurrentConfig()
                self.sendResponse("response","clearcurrentconfig",data)
                return True
            elif self.__jsonObject["request"] == "copyconfig":
                data = {}
                data["status"] = self.__vpnManager._routerVpnManager__vpnFileManager.copyConfig(self.__jsonObject["data"][u'name'])
                self.sendResponse("response","copyconfig",data)
                return True
            else:
                self.__exception = "The request does not exist"
        else:
            self.__exception = "Could not processs this type of request"
        return False
    def handleBroadcast(self,response):
        if(response["type"] == "broadcast"):
            self.__sock.send(json.dumps(response));



#this will handle the socket connection for a paticular client
class client(threading.Thread):
    __connection = None
    __stopProcessing = False
    __request = None
    def __init__(self,socket,address,connection,vpnManager):
        threading.Thread.__init__(self)
        self.__connection = connection
        self.sock = socket
        self.add=address
        self.__request = processRequest(vpnManager)
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
                    log.writeLine('client sent: '+ data)
                    if (not self.__request.processInput(data,self.sock,self.__connection)):
                        self.sock.send('Messsage recived, could not process request: ', self.__request.getException())
        except Exception,e: 
            log.writeLine( str(e))
        finally:
            self.disconnect()
    def disconnect(self):
        log.writeLine( "client disconnected")
        self.__connection.disconnect(self)


#this handles all the connected clients
#this also handles any kind of broadcast
class connections:
    __host = None
    __port = 0
    __serversocket = None;
    __clientsMap = {}
    __clientsMapLock = None
    __vpnManager = None
    def __init__(self,host,port):
        self.__host = host
        self.__port = port
        self.__clientsMapLock = threading.Lock()
        self.__vpnManager = routerVpnManager(self)
        log.setConnections(self)
        self.bind()
    def exit(self):
        for c in self.__clientsMap:
            self.__clientsMap[c].stop()
        self.__vpnManager.exit();
        if self.__serversocket is not None:
            self.__serversocket.close();
    def bind(self):
        log.writeLine('Port Binded')
        self.__serversocket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        self.__serversocket.bind((self.__host,self.__port))
    def listen(self):
        log.writeLine('server started and listening')
        self.__serversocket.listen(5)
        try:
            while 1:
                clientsocket, address = self.__serversocket.accept()
                self.connect(clientsocket,address)
        except KeyboardInterrupt:
            log.writeLine("keyboard interuption")
        except Exception,e:
            log.writeLine(str(e))

    def vpnUnexpectedDisconnectionBroadcast(self):
        data = {}
        data["status"] = ""
        data["reason"] = "Unexpected Disconnection"
        self.sendBroadcast("broadcast","disconnectfrompvpn",data,"")
    def sendBroadcast(self,type,request,data,signature):
        response = {}
        response["type"] = type
        response["request"] = request
        response["data"] = data
        response["signature"] = signature

        if(type == "broadcast"):
            self.__clientsMapLock.acquire()
            try:
                for k in self.__clientsMap:
                    self.__clientsMap[k]._client__request.handleBroadcast(response)
            except Exception,e: 
                log.writeLine(str(e))
            finally:
                self.__clientsMapLock.release()

    def connect(self,clientsocket,address):
        log.writeLine("connecting to  %s:%d" % (address[0],address[1]))
        self.__clientsMapLock.acquire()
        try:
            self.__clientsMap[address[1]] = client(clientsocket,address,self,self.__vpnManager)
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
        c = None
        try:
            c = connections(host,port)
            c.listen();
        except Exception,e:
            log.writeLine(e)
        finally:
            if c is not None:
                c.exit()
            log.stop()
        #Add some sort of closing code here or whatever to manage a crash/shutdown
    else:
        log.writeLine("This script requires a host address and port")

def signal_term_handler(signal, frame):
    log.writeLine('got SIGTERM')
    sys.exit(0)
 
signal.signal(signal.SIGTERM, signal_term_handler)





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






