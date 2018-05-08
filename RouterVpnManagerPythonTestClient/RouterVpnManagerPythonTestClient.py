
import sys
import subprocess
import socket


if len(sys.argv) >= 2:
    host = sys.argv[1]
    port = int(sys.argv[2])
    s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
    s.connect((host,port))

    def ts(str):
        s.send('e'.encode())
        data = s.recv(1024).decode()
        print(data)
    while 2:
        r = input('enter')
        ts(r)
    s.close
else:
    print("This script requires a host address and port")
