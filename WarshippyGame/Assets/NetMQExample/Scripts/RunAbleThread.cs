using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Events;

/// <summary>
///     The superclass that you should derive from. It provides Start() and Stop() method and Running property.
///     It will start the thread to run Run() when you call Start().
/// </summary>
public abstract class RunAbleThread
{
    private readonly Thread _runnerThread;
    private readonly Thread _ReceiveThread;
    public string messageToSend;
    public string logMessage;
    public string receivedMessage;
    public UnityEvent<String> OnMessageReceived;
    protected RunAbleThread()
    {
        // we need to create a thread instead of calling Run() directly because it would block unity
        // from doing other tasks like drawing game scenes
        _runnerThread = new Thread(Run);
        _ReceiveThread = new Thread(Receive);
    }
    public void OnNewText(string text)
    {
        OnMessageReceived.Invoke(text);
    }
    protected bool Running { get; private set; }


    /// <summary>
    /// This method will get called when you call Start(). Programmer must implement this method while making sure that
    /// this method terminates in a finite time. You can use Running property (which will be set to false when Stop() is
    /// called) to determine when you should stop the method.
    /// </summary>
    protected abstract void Run();
    protected abstract void Receive();
    protected bool IsBase64String()
    {
        string s = messageToSend;
        s = s.Trim();
        return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

    }
    
    public void Start()
    {
        Running = true;
        _runnerThread.Start();
        _ReceiveThread.Start();
    }
    
    public void Stop()
    {
        Running = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        _runnerThread.Join();
        _ReceiveThread.Join();
    }
}