using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using UnityWebSocket;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class HelloRequester
{
    private readonly Thread receiveThread;
    private bool running;
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    private string message;
    public HelloRequester()
    {
        /*string address = "tcp://localhost:5555";
        WebSocket socket = new WebSocket(address);
        receiveThread = new Thread((object callback) => 
        {
            socket.OnMessage += OnMessage;
            //var socket = new RequestSocket();
            try{
                socket.ConnectAsync();

                Debug.Log("Connected to socket 'server'");
                socket.SendAsync("ACK");
                    while (running)
                    {
                        if (!queue.IsEmpty)
                        {
                            string request;
                            queue.TryDequeue(out request);
                            Debug.Log("Sending " + request + " to python server");
                            // send the request to server
                            // wait for the response
                            // use the response to do whatever you want the button to do
                        }
                        
                        socket.SendAsync("ACK");
                        
                        ((Action<String>)callback)(message);
                    }
            }finally{
                if (socket != null)
                {
                    socket.CloseAsync();
                }
            }
        }
        );*/
        
    }
    public void Start(Action<String> callback)
    {
        running = true;
        receiveThread.Start(callback);
    }

    public void Stop()
    {
        running = false;
        receiveThread.Join();
    }

    public void add_string_to_queue(string message)
    {
        queue.Enqueue(message);
    }
}