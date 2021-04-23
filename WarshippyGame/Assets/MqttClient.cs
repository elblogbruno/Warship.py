
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
    public class MqttClient : M2MqttUnityClient
    {
        const string BOT_TOPIC = "BOT";
        const string BOT_TOPIC_IMAGE = "IMAGE";

        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;

        public UnityAction<string> onNewMessageMQTT;
        public UnityAction<string> onNewMessageMQTTImage;
        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;

        public void TestPublish()
        {
            client.Publish(BOT_TOPIC, System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message published");
        }
        
        /// <summary>
        /// Publishes a message on the topic
        /// </summary>
        /// <param name="msg"></param>
        public void PublishMSG(string msg,bool isImage)
        {
            string topic = BOT_TOPIC;
            if (isImage)
            {
                topic = BOT_TOPIC_IMAGE;
            }
            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
        public void SetBrokerAddress(string brokerAddress)
        {
            if (!updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (!updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
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

            if (autoTest)
            {
                TestPublish();
            }
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { BOT_TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { BOT_TOPIC_IMAGE }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { BOT_TOPIC });
            client.Unsubscribe(new string[] { BOT_TOPIC_IMAGE });
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

        private void UpdateUI()
        {
            updateUI = false;
        }

        protected override void Start()
        {
            Debug.Log("Ready.");
            updateUI = true;
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("[MQTT CLIENT] Received: " + msg);
            StoreMessage(msg);
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

        private void StoreMessage(string eventMsg)
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
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}
