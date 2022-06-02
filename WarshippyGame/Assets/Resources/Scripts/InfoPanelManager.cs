using System.Collections;
using System.Collections.Generic;
using M2MqttUnity.Examples;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
public class Message
{
    public string message;
    public bool pass2bot;
    public Message(string m, bool pas)
    {
        message = m;
        pass2bot = pas;
    }
}
public class InfoPanelManager : MonoBehaviour
{
    
    public GameObject InfoPanel;
    public Animator PanelAnimator;
    public TMPro.TMP_Text InfoText;
    //public int CurrIndex = 0;
    public static InfoPanelManager instance = null;

    static List<Message> _messageQueue = new List<Message>();

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
            if (_messageQueue.Count > 0)
            {
                yield return StartCoroutine(HandleMessage(_messageQueue[0]));
                _messageQueue.RemoveAt(0);
            }

            yield return null;
        }
    }
    
    // Start is called before the first frame update
    public void SpawnInfoMessage(string message, bool passToBot = false)
    {
        _messageQueue.Add(new Message(message,passToBot));
    }
    IEnumerator HandleMessage(Message message)
    {
        InfoText.text = message.message;
        
        if (message.pass2bot)
        {
            MessagePacket message_from_panel = new MessagePacket(message.message, "text");

            TelegramServerRequesterHelper.SendMessageToBot(message_from_panel.ToJson(),this);
        }
        
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
}
