using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class GetRandomChuckNorrisJoke : MonoBehaviour
{
    public bool chuck_mode = false;

    public string[] jokes_endpoint = new[]
        {"https://official-joke-api.appspot.com/random_joke", "https://api.chucknorris.io/jokes/random"};
    public TMP_Text jokeText;
    // Start is called before the first frame update
    void OnEnable()
    {
        jokeText.text = "Loading....";
        StartCoroutine(GetJoke());
    }
    IEnumerator GetJoke()
    {
        string api_endpoint = chuck_mode ? jokes_endpoint[1] : jokes_endpoint[0];
        UnityWebRequest www = UnityWebRequest.Get(api_endpoint);
        Debug.Log(www.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var N = JSON.Parse(www.downloadHandler.text);
            string joke = "";
            
            if (chuck_mode)
                joke = N["value"].Value;
            else
                joke = N["setup"].Value + " " + N["punchline"].Value;
            
            jokeText.text = joke;
        }
    }
}
