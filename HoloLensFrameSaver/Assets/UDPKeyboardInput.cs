/*
*  UDPKeyboardInput.cs
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




using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

#if !UNITY_EDITOR && UNITY_METRO
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#endif


public class UDPKeyboardInput : MonoBehaviour {

    private int port = 48055;

#if !UNITY_EDITOR && UNITY_METRO
    private string lastReceivedUDPPacket = "";
    private readonly static Queue<string> receivedUDPPacketQueue = new Queue<string>();

    DatagramSocket socket;
    
    async void Start() {
        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;
        HostName IP = null;
        try
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
                hn =>
                    hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                    == icp.NetworkAdapter.NetworkAdapterId);

            await socket.BindEndpointAsync(IP, port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }
        Debug.Log("DatagramSocket setup done...");
    }

  
    
    // Update is called once per frame
    void Update() {
        ;
    }
    
    public string GetLatestUDPPacket() {
        string returnedLastUDPPacket = "";
        while (receivedUDPPacketQueue.Count > 0) {
            returnedLastUDPPacket = receivedUDPPacketQueue.Dequeue();
        }
        return returnedLastUDPPacket;
    }


    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        //Debug.Log("Received message: ");
        //Read the message that was received from the UDP echo client.
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        string message = await reader.ReadLineAsync();

        Debug.Log("Message: " + message);

        lastReceivedUDPPacket = message;
        receivedUDPPacketQueue.Enqueue(message);

    }


    private void OnDestroy() {
        if (socket != null) {
            socket.MessageReceived -= Socket_MessageReceived;
            socket.Dispose();
            Debug.Log("Socket disposed");
        }
    }
#else
    
    public string GetLatestUDPPacket() { return ""; }
#endif
}


