using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    public List<string> ListOfMessages;
    public Animator PanelAnimator;
    public TMPro.TMP_Text InfoText;
    public int CurrIndex = 0;
    public static InfoPanelManager instance = null;
    public void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        Debug.Log("Info panel manager");
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    public void SpawnInfoMessage(string message)
    {
        ListOfMessages.Add(message);
        InfoText.text = message;
        this.gameObject.SetActive(true);
        PanelAnimator.Play("SlideIn");
        CameraShake.instance.TriggerShake(3.0f);
    }
    void OnMessageEnd()
    {
        CurrIndex++;
    }
    // Update is called once per frame
    void Start()
    {
        //SpawnInfoMessage("Welcome to Warship.py!");
    }
}
