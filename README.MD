HoloLens Camera Calibration
===
Author: [Long Qian](https://longqian.me/aboutme)

Date: 2017-02-01

This repository contains **camera calibration file** of HoloLens [front-facing camera](https://developer.microsoft.com/en-us/windows/holographic/locatable_camera), with the 896x504 configuration.

## Near range calibration
The folder ```near``` contains calibration images collected from HoloLens and the calibration results of it.
The calibration pattern for near range calibration is (7x5, 30cm).
The images are taken from a distance of **about 0.3m to 0.5m**.

## Far range calibration
The folder ```far``` contains the calibration images and results of HoloLens, with distance of **about 1m to 1.5m**.
The calibration pattern configuration is (11x7, 45cm).

## Scripts
* ```calibrate.py``` and ```common.py``` are adapted from OpenCV project. Example usage is:
```bash
python calibrate.py --square_size 0.03 --pattern_width 7 --pattern_height 5 data1/raw/*.jpg
```
* ```cv2.CALIB_FIX_K3``` flag is set for function ```cv2.calibrateCamera``` in order for consistent adpation to ARToolKit camera parameter.
* ```scripts/ARToolKitCalibrationFileGenerator.cpp``` generates binary camera calibration file for ARToolKit. You need to correctly include the header files of ARToolKit and link to ARToolKit libraries.
* ```scripts/ARToolKitCalibrationFileReader.py``` makes your life easier when you want to see the values of an ARToolKit calibration file, and modify them.


## Results
* ```hololens896x504.json``` and ```hololens896x504.yaml``` are the camera calibration files of [OpenCV format](http://docs.opencv.org/2.4/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html).
* ```hololens896x504.dat``` is the calibration file of HoloLens camera, for [ARToolKit](https://artoolkit.org/documentation/doku.php?id=2_Configuration:config_camera_calibration).

