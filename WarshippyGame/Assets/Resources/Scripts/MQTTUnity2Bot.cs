using System.Collections;
using System.Collections.Generic;
using M2MqttUnity.Examples;
using UnityEngine;

public static class MQTTUnity2Bot
{
    public static void SendMessageToBot(string message, MqttClient client)
    {
        client.PublishMSG(message,false);
    }
    public static void SendImageToBot(string path, MqttClient client)
    {
        client.PublishMSG(path,true);
    }
}
