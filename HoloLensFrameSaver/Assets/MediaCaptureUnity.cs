/*
*  MediaCaptureUnity.cs
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
using System.Runtime.InteropServices;
using UnityEngine.UI;
#if !UNITY_EDITOR && UNITY_METRO
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Threading.Tasks;
using System;
using Windows.Media.MediaProperties;
using Windows.Graphics.Imaging;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
#endif


/// <summary>
/// The MediaCaptureUnity class manages video access of HoloLens. Many of the code
/// came from HoloLensARToolKit.
///
/// Author:     Long Qian
/// Email:      lqian8@jhu.edu
/// </summary>
public class MediaCaptureUnity : MonoBehaviour {
    
    public Material mediaMaterial;
    private Texture2D mediaTexture;

    public int targetVideoWidth, targetVideoHeight;
    private float targetVideoFrameRate = 60f;
    public Text calibrationText;
    
    private enum CaptureStatus {
        Clean,
        Initialized,
        Running
    }
    private CaptureStatus captureStatus = CaptureStatus.Clean;
    public FpsDisplayHUD fpsDisplayHUD;
    public static string TAG = "MediaCaptureUnity";

    private UDPKeyboardInput udpKeyboard;
    private FrameSaver frameSaver;


#if !UNITY_EDITOR && UNITY_METRO
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    private SoftwareBitmap upBitmap = null;
    private SoftwareBitmap _tempBitmap = null;
    private MediaCapture mediaCapture;


    private MediaFrameReader frameReader = null;

    private int videoWidth = 0;
    private int videoHeight = 0;
    private int HL = 0;



    private async Task<bool> InitializeMediaCaptureAsync() {
        if (captureStatus != CaptureStatus.Clean) {
            Debug.Log(TAG + ": InitializeMediaCaptureAsync() fails because of incorrect status");
            return false;
        }

        if (mediaCapture != null) {
            return false;
        }
    
        var allGroups = await MediaFrameSourceGroup.FindAllAsync();
        int selectedGroupIndex = -1;
        for (int i = 0; i < allGroups.Count; i++) {
            var group = allGroups[i];
            Debug.Log(group.DisplayName + ", " + group.Id);
            // for HoloLens 1
            if (group.DisplayName == "MN34150") {
                selectedGroupIndex = i;
                HL = 1;
                Debug.Log(TAG + ": Selected group " + i + " on HoloLens 1");
                break;
            }
            // for HoloLens 2
            else if (group.DisplayName == "QC Back Camera") {
                selectedGroupIndex = i;
                HL = 2;
                Debug.Log(TAG + ": Selected group " + i + " on HoloLens 2");
                break;
            }
        }

        if (selectedGroupIndex == -1) {
            Debug.Log(TAG + ": InitializeMediaCaptureAsyncTask() fails because there is no suitable source group");
            return false;
        }

        // Initialize mediacapture with the source group.
        mediaCapture = new MediaCapture();
        MediaStreamType mediaStreamType = MediaStreamType.VideoPreview;
        if (HL == 1) {
            var settings = new MediaCaptureInitializationSettings {
                SourceGroup = allGroups[selectedGroupIndex],
                // This media capture can share streaming with other apps.
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,
                // Only stream video and don't initialize audio capture devices.
                StreamingCaptureMode = StreamingCaptureMode.Video,
                // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
                // instead of preferring GPU D3DSurface images.
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };
            await mediaCapture.InitializeAsync(settings);
            Debug.Log(TAG + ": MediaCapture is successfully initialized in SharedReadOnly mode for HoloLens 1.");
            mediaStreamType = MediaStreamType.VideoPreview;
        }
        else if (HL == 2){
            string deviceId = allGroups[selectedGroupIndex].Id;
            // Look up for all video profiles
            //IReadOnlyList<MediaCaptureVideoProfile> profiles = MediaCapture.FindAllVideoProfiles(deviceId);
            //MediaCaptureVideoProfile selectedProfile;
            IReadOnlyList<MediaCaptureVideoProfile> profileList = MediaCapture.FindKnownVideoProfiles(deviceId, KnownVideoProfile.VideoConferencing);

            // Initialize mediacapture with the source group.
            var settings = new MediaCaptureInitializationSettings {
                //SourceGroup = allGroups[selectedGroupIndex],
                VideoDeviceId = deviceId,
                VideoProfile = profileList[0],
                // This media capture can share streaming with other apps.
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                // Only stream video and don't initialize audio capture devices.
                StreamingCaptureMode = StreamingCaptureMode.Video,
                // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
                // instead of preferring GPU D3DSurface images.
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };
            await mediaCapture.InitializeAsync(settings);
            Debug.Log(TAG + ": MediaCapture is successfully initialized in ExclusiveControl mode for HoloLens 2.");
            mediaStreamType = MediaStreamType.VideoRecord;
        }



        try {
            var mediaFrameSourceVideo = mediaCapture.FrameSources.Values.Single(x => x.Info.MediaStreamType == mediaStreamType);
            MediaFrameFormat targetResFormat = null;
            float framerateDiffMin = 60f;
            foreach (var f in mediaFrameSourceVideo.SupportedFormats.OrderBy(x => x.VideoFormat.Width * x.VideoFormat.Height)) {
                if (f.VideoFormat.Width == targetVideoWidth && f.VideoFormat.Height == targetVideoHeight ) {
                    if (targetResFormat == null) {
                        targetResFormat = f;
                        framerateDiffMin = Mathf.Abs(f.FrameRate.Numerator / f.FrameRate.Denominator - targetVideoFrameRate);
                    }
                    else if (Mathf.Abs(f.FrameRate.Numerator / f.FrameRate.Denominator - targetVideoFrameRate) < framerateDiffMin) {
                        targetResFormat = f;
                        framerateDiffMin = Mathf.Abs(f.FrameRate.Numerator / f.FrameRate.Denominator - targetVideoFrameRate);
                    }
                }
            }
            if (targetResFormat == null) {
                targetResFormat = mediaFrameSourceVideo.SupportedFormats[0];
                Debug.Log(TAG + ": Unable to choose the selected format, fall back");
            }
            // choose the smallest resolution
            //var targetResFormat = mediaFrameSourceVideoPreview.SupportedFormats.OrderBy(x => x.VideoFormat.Width * x.VideoFormat.Height).FirstOrDefault();
            // choose the specific resolution
            //var targetResFormat = mediaFrameSourceVideoPreview.SupportedFormats.OrderBy(x => (x.VideoFormat.Width == 1344 && x.VideoFormat.Height == 756)).FirstOrDefault();
            await mediaFrameSourceVideo.SetFormatAsync(targetResFormat);
            Debug.Log(TAG + ": mediaFrameSourceVideo.SetFormatAsync()");
            frameReader = await mediaCapture.CreateFrameReaderAsync(mediaFrameSourceVideo, targetResFormat.Subtype);
            Debug.Log(TAG + ": mediaCapture.CreateFrameReaderAsync()");
            frameReader.FrameArrived += OnFrameArrived;
            videoWidth = Convert.ToInt32(targetResFormat.VideoFormat.Width);
            videoHeight = Convert.ToInt32(targetResFormat.VideoFormat.Height);
            Debug.Log(TAG + ": FrameReader is successfully initialized, " + videoWidth + "x" + videoHeight + 
                ", Framerate: " + targetResFormat.FrameRate.Numerator + "/" + targetResFormat.FrameRate.Denominator);
        }
        catch (Exception e) {
            Debug.Log(TAG + ": FrameReader is not initialized");
            Debug.Log(TAG + ": Exception: " + e);
            return false;
        }
        
        captureStatus = CaptureStatus.Initialized;
        return true;
    }
    
    private async Task<bool> StartFrameReaderAsync() {
        Debug.Log(TAG + " StartFrameReaderAsync() thread ID is " + Thread.CurrentThread.ManagedThreadId);
        if (captureStatus != CaptureStatus.Initialized) {
            Debug.Log(TAG + ": StartFrameReaderAsync() fails because of incorrect status");
            return false;
        }
        
        MediaFrameReaderStartStatus status = await frameReader.StartAsync();
        if (status == MediaFrameReaderStartStatus.Success) {
            Debug.Log(TAG + ": StartFrameReaderAsync() is successful");
            captureStatus = CaptureStatus.Running;
            return true;
        }
        else {
            Debug.Log(TAG + ": StartFrameReaderAsync() is successful, status = " + status);
            return false;
        }
    }

    private async Task<bool> StopFrameReaderAsync() {
        if (captureStatus != CaptureStatus.Running) {
            Debug.Log(TAG + ": StopFrameReaderAsync() fails because of incorrect status");
            return false;
        }
        await frameReader.StopAsync();
        captureStatus = CaptureStatus.Initialized;
        Debug.Log(TAG + ": StopFrameReaderAsync() is successful");
        return true;
    }

    private bool onFrameArrivedProcessing = false;
    
    private unsafe void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args) {
        // TryAcquireLatestFrame will return the latest frame that has not yet been acquired.
        // This can return null if there is no such frame, or if the reader is not in the
        // "Started" state. The latter can occur if a FrameArrived event was in flight
        // when the reader was stopped.
        if (onFrameArrivedProcessing) {
            Debug.Log(TAG + " OnFrameArrived() is still processing");
            return;
        }
        onFrameArrivedProcessing = true;
        using (var frame = sender.TryAcquireLatestFrame()) {
            if (frame != null) {
                fpsDisplayHUD.PreviewTick();

                var softwareBitmap = SoftwareBitmap.Convert(frame.VideoMediaFrame.SoftwareBitmap, BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore);
                Interlocked.Exchange(ref _tempBitmap, softwareBitmap);
                frame.VideoMediaFrame.SoftwareBitmap?.Dispose();
            }
        }
        onFrameArrivedProcessing = false;
    }
    
    
    async void InitializeMediaCaptureAsyncWrapper() {
        await InitializeMediaCaptureAsync();
    }

    async void StartFrameReaderAsyncWrapper() {
        await StartFrameReaderAsync();
    }

    async void StopFrameReaderAsyncWrapper() {
        await StopFrameReaderAsync();
    }

    private bool textureInitialized = false;

    // Update is called once per frame
    unsafe void Update() {
        string msg = udpKeyboard.GetLatestUDPPacket();

        if (!textureInitialized && captureStatus == CaptureStatus.Initialized) {
            mediaTexture = new Texture2D(videoWidth, videoHeight, TextureFormat.RGBA32, false);
            mediaMaterial.mainTexture = mediaTexture;
            textureInitialized = true;
            ToggleVideo();
        }
        //Debug.Log(TAG + " Update() thread ID is " + Thread.CurrentThread.ManagedThreadId);

        if (_tempBitmap != null && textureInitialized) {
            Interlocked.Exchange(ref upBitmap, _tempBitmap);
            using (var input = upBitmap.LockBuffer(BitmapBufferAccessMode.Read))
            using (var inputReference = input.CreateReference()) {
                byte* inputBytes;
                uint inputCapacity;
                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputBytes, out inputCapacity);
                mediaTexture.LoadRawTextureData((IntPtr)inputBytes, videoWidth * videoHeight * 4);
                mediaTexture.Apply();

                if (msg == " ") {
                    byte[] bytes = mediaTexture.EncodeToPNG();
                    Debug.Log(TAG + ": Trigger sending or saving");
                    frameSaver.SaveData(bytes);
                }
            }
        }

        if (msg == "s") {
            ToggleVideo();
        }

        if (captureStatus == CaptureStatus.Clean)
            calibrationText.text = "Calibration not started";
        else if (captureStatus == CaptureStatus.Initialized) 
            calibrationText.text = "Calibration initializing";
        else if (captureStatus == CaptureStatus.Running) 
            calibrationText.text = "Calibration: " + frameSaver.saveCount + " images saved";
    }


    void Start() {
        Debug.Log(TAG + " Start() thread ID is " + Thread.CurrentThread.ManagedThreadId);
        udpKeyboard = GetComponent<UDPKeyboardInput>();
        frameSaver = GetComponent<FrameSaver>();
        Application.targetFrameRate = 60;
        captureStatus = CaptureStatus.Clean; 
        InitializeMediaCaptureAsyncWrapper();
    }


    void OnApplicationQuit() {
        if (captureStatus == CaptureStatus.Running) {
            StopFrameReaderAsyncWrapper();
        }
    }

    public void ToggleVideo() {
        Debug.Log(TAG + " OnClick()");
        if (captureStatus == CaptureStatus.Initialized) {
            StartFrameReaderAsyncWrapper();
        }
        else if (captureStatus == CaptureStatus.Running) {
            StopFrameReaderAsyncWrapper();
        }
    }

#else

    public void ToggleVideo() {
        ;
    }

#endif


}
