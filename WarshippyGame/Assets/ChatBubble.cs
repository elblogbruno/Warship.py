using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ChatBubble : MonoBehaviour
{
    public TMP_Text text;

    public Image background;


    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void SetPlayer(bool isPlayer)
    {
        if (isPlayer)
        {
            background.color = Color.blue;
        }
        else
        {
            background.color = Color.red;
        }
    }
    
    public void SetColor(Color newColor)
    {
        background.color = newColor;
    }


}
