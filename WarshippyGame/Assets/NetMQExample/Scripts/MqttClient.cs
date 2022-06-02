using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using AsyncIO;
using NetMQ;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MqttClient : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(AskBotStatus());
    }

    IEnumerator AskBotStatus()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            print("Requesting bot status");
            TelegramServerRequesterHelper.GetBotStatus(this);
        }
    }
}