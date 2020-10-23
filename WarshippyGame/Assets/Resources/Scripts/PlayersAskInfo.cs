using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
public class PlayersAskInfo : MonoBehaviour
{
    public AudioSource CameraAudio;
    public RawImage m_CurrentImage;
    string deviceName;
    public InputField m_NameInputField;
    public string m_PlayerName;
    public TMP_Text NameTextStatus;
    public Dictionary<string,bool> CurrBoleans;
    public List<string> myKeys = new List<string>();
    WebCamTexture wct;
    public GameObject StatusPanel;
    bool once = false;
    void Start()
    {
        StatusPanel.SetActive(false);
           CurrBoleans = new Dictionary<string, bool>();
        CurrBoleans.Add("Picture", false);
        CurrBoleans.Add("Name", false);

        WebCamDevice[] devices = WebCamTexture.devices;
        deviceName = devices[0].name;
        Debug.Log(deviceName);
        wct = new WebCamTexture(deviceName, 400, 300, 12);
        m_CurrentImage.material.mainTexture = wct;
        if (!once){
            wct.Play();
        }
    }
    public void GoToGame()
    {
        StartCoroutine(LoadYourAsyncScene("GameScene"));
    }
    IEnumerator LoadYourAsyncScene(string levelName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    //private string _SavePath = ; //Change the path here!
    int _CaptureCounter = 0;
    public void GetName()
    {
        StatusPanel.SetActive(true);
        if (m_NameInputField.text.Length > 0)
        {
            Debug.Log("");
            m_PlayerName = m_NameInputField.text;
            PlayerPrefs.SetString("PlayerName", m_PlayerName);
            InfoPanelManager.instance.SpawnInfoMessage("Now I know your fucking ugly name!");
            NameTextStatus.text = "YOUR FUCKING NAME IS AVAILABLE CAUSE IT'S FUCKING UNIQUE";
            CurrBoleans["Name"] = true;
        }
        else
        {
            InfoPanelManager.instance.SpawnInfoMessage("Please write some fucking name before calling me out");
            NameTextStatus.text = "PLEASE WRITE ME SOME FUCKING NAME BEFORE CALLING ME OUTS";
            CurrBoleans["Name"] = false;
        }
    }
    int count  = 0;
    public void PlayGame()
    {
        Debug.Log("Play Game Button clicked");

        Debug.Log("text" + CurrBoleans["Name"] + "picture:" + CurrBoleans["Picture"]);
        myKeys = CurrBoleans.Keys.ToList();
        

            for (int i = 0; i < myKeys.Count; i++)
            {
                Debug.Log("This is count value: " + count);
                if (CurrBoleans[myKeys[i]])
                {
                    count++;
                }
                else
                {
                    count--;
                    InfoPanelManager.instance.SpawnInfoMessage("There are some things left: " + myKeys[i]);
                }
            }
        Debug.Log("Total count value: " + count);
        if (count == myKeys.Count)
        {
                InfoPanelManager.instance.SpawnInfoMessage("Playing game");
                GoToGame();
        }
        else
        {
            Debug.Log("count is not: " + count);
        }



    }
    public void TakeSnapshot()
    {
        wct.Play();
        Texture2D snap = new Texture2D(wct.width, wct.height);
        snap.SetPixels(wct.GetPixels());
        snap.Apply();
        string m_PhotoUri = Path.Combine(Application.persistentDataPath, _CaptureCounter.ToString() + "UserPhoto.png"); 
        System.IO.File.WriteAllBytes(m_PhotoUri, snap.EncodeToPNG());
        PlayerPrefs.SetString("PhotoURI", m_PhotoUri);
        ++_CaptureCounter;
        CurrBoleans["Picture"] = true;
        CameraAudio.Play();
        InfoPanelManager.instance.SpawnInfoMessage("Now I can See your fucking ugly face!");
    }
}
