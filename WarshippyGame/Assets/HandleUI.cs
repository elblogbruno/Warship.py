using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandleUI : MonoBehaviour
{
    public MainPanelUI mainPanel;
    public GameObject loadingJokesPanel;
    
    public void StartingUi()
    {
        Debug.Log("Changing UI to Starting UI");
        mainPanel.gameObject.SetActive(false);
        loadingJokesPanel.SetActive(true);
    }
    public void SetPlacingBoatsUi()
    {
        Debug.Log("Changing UI to Placing Boats");
        mainPanel.gameObject.SetActive(true);
        loadingJokesPanel.SetActive(false);
    }
}
