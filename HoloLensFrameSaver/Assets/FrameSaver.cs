/*
*  FrameSaver.cs
*  HoloLensCamCalib
*
*  This file is a part of HoloLensCamCalib.
*
*  HoloLensCamCalib is free software: you can redistribute it and/or modify
*  it under the terms of the GNU Lesser General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*
*  HoloLensCamCalib is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public License
*  along with HoloLensCamCalib.  If not, see <http://www.gnu.org/licenses/>.
*
*  Copyright 2020 Long Qian
*
*  Author: Long Qian
*  Contact: lqian8@jhu.edu
*
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_METRO
using System.IO;
using System.Text;
using Windows.Storage;
using System.Threading.Tasks;
using System;
#endif



/// <summary>
/// The FrameSaver class manages saving images to disk on UWP platforms
///
/// Author:     Long Qian
/// Email:      lqian8@jhu.edu
/// </summary>
public class FrameSaver : MonoBehaviour {

    [HideInInspector]
    public int saveCount = 0;

#if !UNITY_EDITOR && UNITY_METRO
    public async void SaveData(byte[] data) {
        var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(saveCount + ".png", CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(file, data);
        saveCount++;
        Debug.Log("Save done for " + saveCount + ".png");
    }
#else

    public void SaveData(byte[] data){
        Debug.Log("Not supported on this platform");
    }
#endif

}
