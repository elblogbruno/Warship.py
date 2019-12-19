using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UnityEngine.Networking;

public class ButtonManifest : MonoBehaviour, IPointerEnterHandler
{
    #region Const

    int WIDTH = 452;
        int HEIGHT = 415;
    #endregion
    public string ButtonAttackCoordenates;
    public Image ButtonGridImage;
    public Text positionText;
    public Sprite BGon;
    public Sprite BGoff;
    [HideInInspector]
    public HelloClient PythonClient;

    public string API_URL
    {
        get
        {
            return string.Format("http://127.0.0.1:5001/");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySingle(SoundManager.SoundType.BUTTON_SOUND);
    }
    public void setText(string text){
        positionText.text = text;
    }
    public void TakeScreenshotOfPlay(int width,int height)
    {
        StartCoroutine(TakeSnapshot());
    }
    public void TakeScreenshot()
    {
        var width = 452;
        var height = 415;
        var startX = 168;
        var startY = 10;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToJPG();
        string bytesstr = Convert.ToBase64String(bytes);
        //PythonClient.SendText(bytesstr);
        Destroy(tex);

        //System.IO.File.WriteAllBytes(Application.dataPath + "SavedScreen.png", bytes);

    }
    WaitForSeconds waitTime = new WaitForSeconds(0.1F);
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return waitTime;
        yield return frameEnd;
        var width = 452;
        var height = 415;
        var startX = 168;
        var startY = 10;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToJPG();
        SendFile(tex.EncodeToJPG(), "name"+System.DateTime.UtcNow);
        //string bytesstr = Convert.ToBase64String(bytes);
        //PythonClient.SendText(bytesstr);
        Destroy(tex);

    }

    public void SendFile(byte[] bytes, string filename, string caption = "")
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", bytes, filename, "filename");
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendimage?", form);
        StartCoroutine(SendRequest(www));
    }

    IEnumerator SendRequest(UnityWebRequest www)
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

    // SERVER //////////////////////////



    public void onClick()
    {
        Debug.Log(ButtonAttackCoordenates);
        PythonClient.SendText(ButtonAttackCoordenates);
        TakeScreenshotOfPlay(WIDTH,HEIGHT);
        InfoPanelManager.instance.SpawnInfoMessage("Attacking Bot Player at this coordenates: "+ ButtonAttackCoordenates);
    }
}
