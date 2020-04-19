HoloLens Camera Calibration
===
Author: [Long Qian](http://longqian.me/aboutme)

Date: 2020-04-19

## Overview 

This repository guides you to calibrate your HoloLens [front-facing camera](https://docs.microsoft.com/en-us/windows/mixed-reality/locatable-camera) using OpenCV. It supports both HoloLens 1 and **HoloLens 2**. 

The intrinsics of HoloLens front-facing camera is available with Windows UWP API, but we still want to calibrate it for various reasons. Then it is the right repository for you.

This repository contains:
- `HoloLensFrameSaver/`: Unity project to capture and save images for calibration
- `OpenCVCalib/`: python scripts to calibrate the camera using saved images
- `CalibFileConverter/`: C++ program to convert the OpenCV calibration file to [ARToolKit format](https://artoolkit.org/documentation/doku.php?id=2_Configuration:config_camera_calibration)
- `Examples/`: Example calibration files and images for HoloLens 1 and HoloLens 2 (with my device)

## How to Calibrate?

1. Build **HoloLensFrameSaver**
	* Change the target resolution for the front-facing camera in Unity, available resolution profiles are listed [here](https://docs.microsoft.com/en-us/windows/mixed-reality/locatable-camera)
	* Apply approriate settings to build the app and deploy to device
2. Run **HoloLensFrameSaver** on the device
	* Soon after starting, the camera image should be displayed in the front
	* Prepare a chessboard marker for calibration, e.g. ```chessboard.pdf```
	* Run the python script on the host PC ```HoloLensFrameSaver\keyboard.py``` with IP address of HoloLens. The script sends keyboard command to the HoloLens
	* Adjust your head position to look at the chessboard from various angles and distances. Press **Space** key on the keyboard to trigger HoloLens to save the image locally
3. Download the images from the Device Portal, ```FileExplorer\LocalAppData\FrameSaver\RoamingState\```
4. Run the Python script in **OpenCVCalib** to calibrate the saved images
	* ```python calibrate.py --square_size 0.03 --pattern_width 7 --pattern_height 5 PATH/TO/IMAGE/*.png```
	* If you used your own chessboard marker, change the arguments of the python script
	* You will find the calibration file at ```output\data.yaml``` and ```output\data.json```
5. (Optional) If you come from [HoloLensARToolKit](https://github.com/qian256/HoloLensARToolKit), you need to convert the OpenCV format calibration file to ARToolKit format. You can achieve that by using the C++ program in `CalibFileConverter/`.
	* Modify the parameters in ```ARToolKitCalibrationFileGenerator.cpp``` using the results of previous calibration
	* Run the program, and copy the calibration file to HoloLensARToolKit


## FAQ

1. How to I verify the binary ARToolKit calibration file?

Run the ```src/ARToolKitCalibrationFileGenerator/ARToolKitCalibrationFileReader.py``` with the specific ```readFileName``` parameter.



## License

HoloLensCamCalib is open for use in compliance with [GNU Lesser General Public License (LGPL v3.0)](https://www.gnu.org/licenses/lgpl-3.0.en.html). Please see COPYING and COPYING.LESSER for license details.





