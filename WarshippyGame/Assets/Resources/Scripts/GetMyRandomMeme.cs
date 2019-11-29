using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetMyRandomMeme : MonoBehaviour
{
    public Vector3 endPosition;
    public Vector3 StartPosition;
    public GameObject LogoObject;
    public GameObject MemeObject;
    public void Start()
    {
        MemeObject.SetActive(false);
           endPosition = new Vector3(-143, 170, 0);
        StartPosition = LogoObject.transform.localPosition;
    }
    public RawImage RawImage;
    public void OpenMemePanel()
    {
        LogoObject.transform.localPosition = endPosition;
        MemeObject.SetActive(true);
        RandomMeme();
    }
    public void CloseMeme()
    {
        LogoObject.transform.localPosition = StartPosition;
        MemeObject.SetActive(false);
    }
    public void RandomMeme()
    {
        StartCoroutine(GetRandomMeme());
    }
    IEnumerator GetRandomMeme()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://meme-api.herokuapp.com/gimme");
        yield return www.SendWebRequest();
        var N = JSON.Parse(www.downloadHandler.text);

        string thumbnail_url = N["url"].Value;
        UnityWebRequest url = UnityWebRequestTexture.GetTexture(thumbnail_url);
        yield return url.SendWebRequest();
        Texture2D myTexture = DownloadHandlerTexture.GetContent(url);
        RawImage.texture = myTexture;
    }
}
