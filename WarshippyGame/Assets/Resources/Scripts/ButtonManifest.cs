using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;

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
        PythonClient.SendText(bytesstr);
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
        string bytesstr = Convert.ToBase64String(bytes);
        PythonClient.SendText(bytesstr);
        Destroy(tex);

    }

    

    // SERVER //////////////////////////

   

    public void onClick()
    {
        Debug.Log(ButtonAttackCoordenates);
        PythonClient.SendText(ButtonAttackCoordenates);
        TakeScreenshotOfPlay(WIDTH,HEIGHT);
    }
}
