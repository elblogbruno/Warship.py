﻿using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class HelloRequester : RunAbleThread
{
   
    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
    /// 
    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            
            client.Connect("tcp://localhost:5555");

            if(Running){
                Debug.Log("Sending the message to send.");
                client.SendFrame(messageToSend);
                
            }
            
            
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
    /// 
    protected override void Receive()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {

            client.Connect("tcp://localhost:5555");

            if (Running)
            {
                string message = null;
                bool gotMessage = false;
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                    if (gotMessage) break;
                }

                if (gotMessage)
                {
                    Debug.Log("Received " + message);
                    receivedMessage = message;
                    OnNewText(message);
                }

            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}