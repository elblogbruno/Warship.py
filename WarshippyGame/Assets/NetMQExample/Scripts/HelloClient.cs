using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.Events;

public class HelloClient : MonoBehaviour
{
    private HelloRequester _helloRequester;
    public string message;
    public UnityAction<string> OnNewMessageReceived;
    private void Start()
    {
        _helloRequester = new HelloRequester();
        //_helloRequester.Start();
        _helloRequester.OnMessageReceived.AddListener(OnNewMessageReceived);
    }
    void OnNewMessageReceiveFromServer(string NewMessage)
    {
        print("Received message from server at HelloClient: " + NewMessage);
        OnNewMessageReceived(NewMessage);
    }

    public void CloseConnection()
    {
        _helloRequester = new HelloRequester();
        _helloRequester.messageToSend = "Quit";
        _helloRequester.Start();
    }
    public void SendText(string message)
    {
        _helloRequester = new HelloRequester();
        _helloRequester.messageToSend = message;
        _helloRequester.Start();
    }
    private void OnDestroy()
    {
        _helloRequester.Stop();
    }
}