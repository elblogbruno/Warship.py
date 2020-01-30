using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class TelegramServerRequesterHelper
{
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
       instance.StartCoroutine(SendMessage(message));
    }

    // Start is called before the first frame update
    public static void GetMessageFromBot(string message, MonoBehaviour instance)
    {
        instance.StartCoroutine(GetMessage(message));
    }
    // Start is called before the first frame update
    public static void SendImageToBot(byte[] bytes, string filename, MonoBehaviour instance)
    {
        SendImageRequest(bytes,filename,instance);
    }

    static IEnumerator GetMessage(string message)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "getmessage?text=" + message);
        Debug.Log(www.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    static IEnumerator SendMessage(string message)
    {
            UnityWebRequest www = UnityWebRequest.Get(API_URL + "sendmessage?text="+message);
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
    }



    static void SendImageRequest(byte[] bytes, string filename, MonoBehaviour instance)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", bytes, filename, "filename");
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendimage?", form);
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
            Debug.Log("Success!\n" + www.downloadHandler.text);
        }
    }
}
