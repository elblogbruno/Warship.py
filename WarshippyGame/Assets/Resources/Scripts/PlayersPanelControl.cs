using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayersPanelControl : MonoBehaviour
{
    public RawImage PlayerImage;
    public TMP_Text PlayerName;
    // Start is called before the first frame update
    void OnEnable()
    {
        string name = PlayerPrefs.GetString("PlayerName");
        string url = PlayerPrefs.GetString("PhotoURI");
        PlayerName.text = name;
        StartCoroutine(GetImage(url));
    }
    IEnumerator GetImage(string url)
    {
        WWW www = new WWW(url);
        while (!www.isDone)
            yield return null;
        PlayerImage.texture = www.texture;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
