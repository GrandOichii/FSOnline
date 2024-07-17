import requests
import time
import json
from sys import argv
import socket
# import rel

DEFAULT_ADDRESS = '127.0.0.1'
DEFAULT_PORT = 9090

class TcpConnection:
    def __init__(self, address, port):
        self.config_received = False
        self.first_state_received = False
        self.open = False

        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print((address, port))
        self.socket.connect((address, port))
        self.socket.settimeout(.2)
        self.on_connect()
        self.run_loop()

    def force_close(self):
        self.socket.close()
        self.open = False

    def on_connect(self):
        self.open = True
        self.write('TcpPlayer')
        # name = input('Enter name: ')
        # name = 'player1'

    def write(self, msg: str):
        message_length = len(msg)
        message_length_bytes = message_length.to_bytes(4, byteorder='little')
        message_bytes = msg.encode('utf-8')
        message_with_length = message_length_bytes + message_bytes
        self.socket.sendall(message_with_length)

    def read(self):
        message = ''
        try :
            message_length_bytes = self.socket.recv(4)
            message_length = int.from_bytes(message_length_bytes, byteorder='little')

            # Receive the message itself
            while len(message) < message_length:
                message_bytes = self.socket.recv(message_length)
                message += message_bytes.decode('utf-8')

        except socket.timeout:
            message = ''
        except socket.error:
            self.open = False

        return message
    
    def run_loop(self):
        while self.open:
            msg = self.read()
            if msg == '': continue
            self.respond(msg)

    def respond(self, msg: str):
        data = json.loads(msg)
        print(json.dumps(data, indent=4))
        if not 'Request' in data:
            return
        request = data['Request']
        if request == 'Update':
            return
        print(data['Hint'] + ': ',end='')
        result = input()
        self.write(result)

conn = TcpConnection(DEFAULT_ADDRESS, DEFAULT_PORT)