HoloLens Camera Calibration
===
Author: [Long Qian](http://longqian.me/aboutme)

Date: 2018-03-25

This repository contains **camera calibration file** of HoloLens [front-facing camera](https://developer.microsoft.com/en-us/windows/holographic/locatable_camera), with the following configurations:

* 896x504, recommended for fast processing
* 1344x756, recommended for wide FOV
* 1280x720, not recommended

OpenCV camera calibration scripts are used to calibrate the HoloLens camera (```src/calibrate.py```).  A custom Visual Studio solution (```src/ARToolKitCalibrationFileGenerator```) converts the parameters from [OpenCV format](https://docs.opencv.org/3.1.0/dc/dbb/tutorial_py_calibration.html) to [ARToolKit format](https://artoolkit.org/documentation/doku.php?id=2_Configuration:config_camera_calibration).

## FAQ

1. How to calibrate using images taken with my own HoloLens?

Run the script ```src/calibrate.py``` with your own checkerboard setting, and the path to your own images, for example:
```bash
python calibrate.py --square_size 0.03 --pattern_width 7 --pattern_height 5 PATH/TO/IMAGE/*.jpg
```

2. How to convert my own OpenCV calibration file to ARToolKit one?

Launch ```src/ARToolKitCalibrationFileGenerator.sln```, modify the parameters in ```main()```, and run it. The Visual Studio 2017 solution is shipped with ARToolKit binaries, and should run on Windows 10 machine instantly.

3. How to I verify the binary ARToolKit calibration file?

Run the ```src/ARToolKitCalibrationFileGenerator/ARToolKitCalibrationFileReader.py``` with the specific ```readFileName``` parameter.


