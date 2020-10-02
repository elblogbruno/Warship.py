using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoPanelManager : MonoBehaviour
{

    public GameObject InfoPanel;
    public Animator PanelAnimator;
    public TMPro.TMP_Text InfoText;
    //public int CurrIndex = 0;
    public static InfoPanelManager instance = null;

    [SerializeField]
    public static List<string> _messageQueue = new List<string>();

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
        StartCoroutine(ProcessMessages());
    }

    IEnumerator ProcessMessages()
    {
        while (true)
        {
            // Codigo que se ejecuta todo el rato continuamente sin dar error al bucle infinito ejem ejem unity
            if (_messageQueue.Count > 0)
            {
                yield return StartCoroutine(HandleMessage(_messageQueue[0]));
                _messageQueue.RemoveAt(0);
            }

            yield return null;
        }
    }
    // Start is called before the first frame update
    public void SpawnInfoMessage(string message)
    {
        _messageQueue.Add(message);
    }

    IEnumerator HandleMessage(string message)
    {
        InfoText.text = message;
        InfoPanel.SetActive(true);
        PanelAnimator.Play("SlideIn");

        if (CameraShake.instance != null)
        {
            CameraShake.instance.TriggerShake(3.0f);
        }
        for (float i = 0f; i < 4; i += 0.02f)
        {
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < 50; i++) yield return new WaitForFixedUpdate();
    }

    // Update is called once per frame
    void Start()
    {
        //SpawnInfoMessage("Welcome to Warship.py!");
    }
}
