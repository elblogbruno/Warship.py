
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using UnityEngine.Events;

/// <summary>
/// Script for testing M2MQTT with a Unity UI
/// </summary>

namespace M2MqttUnity.Examples
{
    public class MqttClient1 : M2MqttUnityClient
    {
        const string BOT_TOPIC = "BOT";
        const string BOT_TOPIC_IMAGE = "IMAGE";

        public UnityAction<string> onNewMessageMQTT;
        public UnityAction<string> onNewMessageMQTTImage;

        /// <summary>
        /// Publishes a message on the topic
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isImage"></param>
        public void PublishMsg(string msg,bool isImage)
        {
            string topic = BOT_TOPIC;
            if (isImage)
            {
                topic = BOT_TOPIC_IMAGE;
            }
            
            //client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("Connected to broker on " + brokerAddress + "\n");
        }

        protected override void SubscribeTopics()
        {
           //client.Subscribe(new string[] { BOT_TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
           // client.Subscribe(new string[] { BOT_TOPIC_IMAGE }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            //client.Unsubscribe(new string[] { BOT_TOPIC });
            //client.Unsubscribe(new string[] { BOT_TOPIC_IMAGE });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            
            Debug.Log("[MQTT CLIENT] Received: " + msg);
            if (topic == BOT_TOPIC_IMAGE)
            {
                if(onNewMessageMQTTImage != null) 
                    onNewMessageMQTTImage(msg);
            }
            else if(topic == BOT_TOPIC && msg != "unblock" && msg != "block")
            {
                if (onNewMessageMQTT != null)
                    onNewMessageMQTT(msg);
            }
        }

        /*private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }


        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    //ProcessMessage(msg);
                    //onNewMessageMQTT(msg);
                }
                eventMessages.Clear();
            }
        }*/
        
        private void OnDestroy()
        {
            Disconnect();
        }
    }
}
