import socket
import msvcrt
import sys

if len(sys.argv) < 2:
	print('Usage: python keyboard.py IP')
	sys.exit()

UDP_IP = sys.argv[1]
UDP_PORT = 48055

sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP
# sock.bind((UDP_IP, UDP_PORT))

while True:
    c = msvcrt.getch()
    if c == b'\x00':
    	continue
    print('You entered: ' + str(c))
    if c == b'q':
        break
    sock.sendto(c, (UDP_IP, UDP_PORT))

sock.close()
