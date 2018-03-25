#!/usr/bin/env python

'''
ARToolKitCalibrationFileReader.py
This file is a part of ARTolKitCamCalib.

Copyright 2017 Long Qian, Alexander Winkler
Contact: lqian8@jhu.edu

This scripts is able to read ARToolKit camera calibration file, and
write ARToolKit camera calibration file.
You may use it however you want.

'''

import numpy as np
import struct

np.set_printoptions(suppress=True)
np.set_printoptions(precision=4)
np.set_printoptions(formatter={'float_kind': '{: 0.4f}'.format})

readFileName = "hololens1344x756.dat"
outFileName = "temp.dat"

fileIn = open(readFileName, 'rb')
fileOut = open(outFileName, 'wb')

try:
	data  = struct.unpack(">2i12d9d", fileIn.read(176))
	xsize = data[0]
	ysize =  data[1]
	mat =  np.reshape(data[2:14], (3, 4))
	k1 = data[14]
	k2 = data[15]
	p1 = data[16]
	p2 = data[17]
	fx  = data[18]
	fy  = data[19]
	x0 = data[20]
	y0 = data[21]
	s   = data[22]
	print("xsize: " + str(xsize))
	print("ysize: " + str(ysize))
	print("mat:" + str(mat))
	print("k1:" + str(k1))
	print("k2:" + str(k2))
	print("p1:" + str(p1))
	print("p2:" + str(p2))
	print("fx:" + str(fx))
	print("fy:" + str(fy))
	print("x0:" + str(x0))
	print("y0:" + str(y0))
	print("s:" + str(s))
	
	# Do random stuff to your calibration here. Like: 
	# mat[1,0] = 200.0
	# or
	# s = 1.0
	
	
	fileOut.write(struct.pack(">2i", xsize, ysize))
	fileOut.write(struct.pack(">12d", *mat.reshape(-1).tolist()))
	fileOut.write(struct.pack(">9d", k1, k2, p1, p2, fx, fy, x0, y0, s))
	

finally:
	fileIn.close()
	fileOut.close()
