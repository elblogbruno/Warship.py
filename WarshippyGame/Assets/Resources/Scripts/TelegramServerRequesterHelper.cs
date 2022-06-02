using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using SimpleJSON;
using UnityEngine.Events;


public static class TelegramServerRequesterHelper
{
    public static UnityAction<MessagePacket> OnBotStatus;
    //public UnityAction<BotStatus> onNewMessageMQTTImage;
    
    public static string API_URL
    {
        get
        {
            return string.Format("http://127.0.0.1:5001/");
        }
    }

    // Start is called before the first frame update
    public static void SendMessageToBot(string message, MonoBehaviour instance)
    {
       instance.StartCoroutine(SendMessage(message,false));
    }
    // Start is called before the first frame update
    public static void SendAudioToBot(string message, MonoBehaviour instance)
    {
        instance.StartCoroutine(SendMessage(message,true));
    }
    // Start is called before the first frame update
    public static void GetBotStatus(MonoBehaviour instance)
    {
        instance.StartCoroutine(GetBotStatus());
    }
    // Start is called before the first frame update
    public static void StartBotPhotoBehaviour(MonoBehaviour instance)
    {
        instance.StartCoroutine(AskForPhoto());
    }
    // Start is called before the first frame update
    public static void SendImageToBot(byte[] bytes, string filename, MonoBehaviour instance)
    {
        SendImageRequest(bytes,filename,instance);
    }
    static IEnumerator AskForPhoto()
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "askForPhoto");
        Debug.Log(www.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
        }
    }
    static IEnumerator GetBotStatus()
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "get_bot_status");
        Debug.Log(www.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log("Message from user: " + www.downloadHandler.text);
            
            MessagePacket bot = JsonUtility.FromJson<MessagePacket>(www.downloadHandler.text);
            
            OnBotStatus?.Invoke(bot);
        }
    }

    static IEnumerator SendMessage(string message,bool isAudio)
    {
        string query = "send_message";
        if (isAudio)
        {
            query = "send_audio";
        }
        string url = API_URL + query+ "?text="+message;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
    static void SendImageRequest(byte[] bytes, string filename, MonoBehaviour instance)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", bytes, filename, "filename");
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "send_image?", form);
        instance.StartCoroutine(SendImageRequest(www));
    }
    static IEnumerator SendImageRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var w = www;
            Debug.Log("Success sending table photo!\n" + www.downloadHandler.text);
        }
    }
}
