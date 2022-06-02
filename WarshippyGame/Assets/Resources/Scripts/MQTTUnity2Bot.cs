using System.Collections;
using System.Collections.Generic;
using M2MqttUnity.Examples;
using UnityEngine;

public static class MQTTUnity2Bot
{
    public static void SendMessageToBot(string message, MqttClient client)
    {
        Debug.Log("MQTT DEPRECATED");
        //client.PublishMsg(message,false);
    }
    public static void SendImageToBot(string path, MqttClient client)
    {
        Debug.Log("MQTT DEPRECATED");
        //client.PublishMsg(path,true);
    }
}
