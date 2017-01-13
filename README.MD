HoloLens Camera Calibration
===
Author: [Long Qian](https://longqian.me/aboutme)

Date: 2017-01-12

This repository contains **camera calibration file** of HoloLens [front-facing camera](https://developer.microsoft.com/en-us/windows/holographic/locatable_camera), with the 896x504 configuration.

## OpenCV Format
* ```hololens896x504.json``` and ```hololens896x504.yaml``` are the camera calibration files of [OpenCV format](http://docs.opencv.org/2.4/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html).
* Note that ```CV_CALIB_FIX_K3``` flag was set.

## ARToolKit Format
* ```hololens896x504.dat``` is the calibration file of HoloLens camera, for [ARToolKit](https://artoolkit.org/documentation/doku.php?id=2_Configuration:config_camera_calibration).

