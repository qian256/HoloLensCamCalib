/*
 *  ARToolKitCalibrationFileGenerator.cpp
 *  HoloLensCamCalib
 * 
 *  This work is a modified version of the original "calib_camera.cpp" of
 *  ARToolKit. The copyright and license information of ARToolKit is included
 *  in this document as required by its GNU Lesser General Public License
 *  version 3.
 * 
 *  This file is a part of HoloLensCamCalib.
 *
 *  Copyright 2020 Long Qian
 *  Contact: lqian8@jhu.edu
 *
 */ 

/* The original copyright information: */
/*
 *  calib_camera.cpp
 *  ARToolKit5
 *
 *  Camera calibration utility.
 *
 *  Run with "--help" parameter to see usage.
 *
 *  This file is part of ARToolKit.
 *
 *  ARToolKit is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ARToolKit is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with ARToolKit.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  As a special exception, the copyright holders of this library give you
 *  permission to link this library with independent modules to produce an
 *  executable, regardless of the license terms of these independent modules, and to
 *  copy and distribute the resulting executable under terms of your choice,
 *  provided that you also meet, for each linked independent module, the terms and
 *  conditions of the license of that module. An independent module is a module
 *  which is neither derived from nor based on this library. If you modify this
 *  library, you may extend this exception to your version of the library, but you
 *  are not obligated to do so. If you do not wish to do so, delete this exception
 *  statement from your version.
 *
 *  Copyright 2015 Daqri, LLC.
 *  Copyright 2002-2015 ARToolworks, Inc.
 *
 *  Author(s): Hirokazu Kato, Philip Lamb
 *
 */

#define ARDOUBLE_IS_FLOAT
#include <AR/ar.h>


// adding this to handle "unresolved external symbol ___iob_func" in VS2015
extern "C" {FILE __iob_func[3] = { *stdin, *stdout, *stderr }; }


static ARdouble getSizeFactor(ARdouble dist_factor[], int xsize, int ysize, int dist_function_version)
{
	ARdouble  ox, oy, ix, iy;
	ARdouble  olen, ilen;
	ARdouble  sf, sf1;

	sf = 100.0;

	ox = 0.0;
	oy = dist_factor[7];
	olen = dist_factor[6];
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = dist_factor[6] - ix;
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = xsize;
	oy = dist_factor[7];
	olen = xsize - dist_factor[6];
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = ix - dist_factor[6];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = dist_factor[6];
	oy = 0.0;
	olen = dist_factor[7];
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = dist_factor[7] - iy;
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = dist_factor[6];
	oy = ysize;
	olen = ysize - dist_factor[7];
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = iy - dist_factor[7];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}


	ox = 0.0;
	oy = 0.0;
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = dist_factor[6] - ix;
	olen = dist_factor[6];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}
	ilen = dist_factor[7] - iy;
	olen = dist_factor[7];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = xsize;
	oy = 0.0;
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = ix - dist_factor[6];
	olen = xsize - dist_factor[6];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}
	ilen = dist_factor[7] - iy;
	olen = dist_factor[7];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = 0.0;
	oy = ysize;
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = dist_factor[6] - ix;
	olen = dist_factor[6];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}
	ilen = iy - dist_factor[7];
	olen = ysize - dist_factor[7];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	ox = xsize;
	oy = ysize;
	arParamObserv2Ideal(dist_factor, ox, oy, &ix, &iy, dist_function_version);
	ilen = ix - dist_factor[6];
	olen = xsize - dist_factor[6];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}
	ilen = iy - dist_factor[7];
	olen = ysize - dist_factor[7];
	//ARLOG("Olen = %f, Ilen = %f, s = %f\n", olen, ilen, ilen / olen);
	if (ilen > 0) {
		sf1 = ilen / olen;
		if (sf1 < sf) sf = sf1;
	}

	if (sf == 100.0) sf = 1.0;

	return sf;
}

static void convParam(ARdouble intr[3][4], ARdouble dist[4], int xsize, int ysize, ARParam *param)
{
	ARdouble   s;
	int      i, j;

	param->dist_function_version = 4;
	param->xsize = xsize;
	param->ysize = ysize;

	param->dist_factor[0] = (ARdouble)dist[0];     /* k1  */
	param->dist_factor[1] = (ARdouble)dist[1];     /* k2  */
	param->dist_factor[2] = (ARdouble)dist[2];     /* p1  */
	param->dist_factor[3] = (ARdouble)dist[3];     /* p2  */
	param->dist_factor[4] = (ARdouble)intr[0][0];  /* fx  */
	param->dist_factor[5] = (ARdouble)intr[1][1];  /* fy  */
	param->dist_factor[6] = (ARdouble)intr[0][2];  /* x0  */
	param->dist_factor[7] = (ARdouble)intr[1][2];  /* y0  */
	param->dist_factor[8] = (ARdouble)1.0;         /* s   */

	for (j = 0; j < 3; j++) {
		for (i = 0; i < 4; i++) {
			param->mat[j][i] = (ARdouble)intr[j][i];
		}
	}

	s = getSizeFactor(param->dist_factor, xsize, ysize, param->dist_function_version);
	param->mat[0][0] /= s;
	param->mat[0][1] /= s;
	param->mat[1][0] /= s;
	param->mat[1][1] /= s;
	param->dist_factor[8] = s;
}

/*
Conversion from opencv camera calibration to artoolkit camera calibration
OpenCV camera calibration format: http://opencv-python-tutroals.readthedocs.io/en/latest/py_tutorials/py_calib3d/py_calibration/py_calibration.html
*/
int main(int argc, char *argv[]) {

	printf("Please make sure to modify the following parameters\n\n");

	int xsize = 1504;
	int ysize = 846;

	if (xsize % 64 != 0) {
		xsize = ((xsize >> 6) + 1) << 6;
	}

	float intr[3][4] = {
		{ 1169.007992972877, 0.0, 758.54471508651, 0.0 },
		{ 0.0, 1170.7600067416026, 413.6251662702983, 0.0 },
		{ 0.0f, 0.0f, 1.0f, 0.0f },
	};
	float dist[4] = { 0.02999846564909687, -0.05913722400315286, 0.0032957434002814606, 0.003026287821656661 };
	ARParam paramHolo;
	convParam(intr, dist, xsize, ysize, &paramHolo);
	arParamDisp(&paramHolo);
	arParamSave("camera_param.dat", 1, &paramHolo);

	return 0;
}

