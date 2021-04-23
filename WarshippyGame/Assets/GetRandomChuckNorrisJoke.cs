using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class GetRandomChuckNorrisJoke : MonoBehaviour
{
    public TMP_Text jokeText;
    // Start is called before the first frame update
    void OnEnable()
    {
        jokeText.text = "Loading....";
        StartCoroutine(GetJoke());
    }
    IEnumerator GetJoke()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.chucknorris.io/jokes/random");
        Debug.Log(www.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var N = JSON.Parse(www.downloadHandler.text);

            string joke = N["value"].Value;
            jokeText.text = joke;
        }
    }
}
