/*
*  FpsDisplayHUD.cs
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


using System;
using UnityEngine;
using UnityEngine.UI;


public class FpsDisplayHUD : MonoBehaviour {
    public Text fpsRender, fpsPreview;

    private float deltaTimeRender = 0.0f;
    private float deltaTimePreview = 0.0f;
    private long lastPreviewTick;

    private void Start() {
        fpsRender.text = string.Format("Render:  {0:0.0} ms ({1:0.} fps)", 0.0f, 0.0f);
        fpsPreview.text = string.Format("Video:   {0:0.0} ms ({1:0.} fps)", 0.0f, 0.0f);
        lastPreviewTick = DateTime.Now.Ticks;
    }

    private void Update() {
        RenderTick();
        fpsRender.text = string.Format("Render:  {0:0.0} ms ({1:0.} fps)", deltaTimeRender * 1000.0f, 1.0f / deltaTimeRender);
        fpsPreview.text = string.Format("Video:   {0:0.0} ms ({1:0.} fps)", deltaTimePreview / 10000.0f, 10000000.0f / deltaTimePreview);
    }
    
    public void RenderTick() {
        deltaTimeRender += (Time.deltaTime - deltaTimeRender) * 0.1f;
    }

    public void PreviewTick() {
        long currentTick = DateTime.Now.Ticks;
        deltaTimePreview += (currentTick - lastPreviewTick - deltaTimePreview) * 0.1f;
        lastPreviewTick = currentTick;
    }
}
