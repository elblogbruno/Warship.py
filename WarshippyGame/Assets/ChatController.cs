using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChatController : MonoBehaviour
{
    public static ChatController instance;
    public Transform chatBubbleContainer;
    
    // public Transform chatContainerPlayer1;

    // public Transform chatContainerPlayer2;

    public ChatBubble chatBubblePrefab;

    public Transform lastChatBubble;

    public InputField inputPrefab;

    void Awake()
    {
        instance = this;
    }

    public void SendMessage()
    {
        //read input and send to bot.
        string message = inputPrefab.text;
        if (message != "")
        {
            AddChatBubble(message, false);

            //send to bot
            MessagePacket messagePacket = new MessagePacket(message, "text");

            TelegramServerRequesterHelper.SendMessageToBot(messagePacket.ToJson(), this);
        }

    }
    public void ReceiveMessageFromBot(string message)
    {
        if (message != "")
        {
            AddChatBubble(message, true);
        }
    }

    public void AddChatBubble(string text, bool isPlayer2)
    {
        Vector3 newPos = new Vector3(0, 0, 0);

        if (lastChatBubble != null)
        {
            newPos = lastChatBubble.position +  new Vector3(0, lastChatBubble.position.y, 0);
        }

        ChatBubble chatBubble = Instantiate(chatBubblePrefab, newPos, Quaternion.identity);
        chatBubble.transform.SetParent(chatBubbleContainer);

        chatBubble.SetText(text);
        chatBubble.SetPlayer(isPlayer2);

        lastChatBubble = chatBubble.GetComponent<Transform>();
    }

}
