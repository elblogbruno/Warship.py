using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class UserTable : MonoBehaviour
{
    #region Variable
    // public MqttClient client;
    public Transform buttonsGrid;
    // public BoatsSlot BoatsSlot;
    public ButtonManifest ButtonTemplate;

    [Header("Events")]
    public UnityEvent onTableEnabled, onTableDisabled;
    
    public int NumOfButtonsToSpawn ;
    
    private bool interactable = true;

    private ButtonManifest[] ListOfButtons;

    private string[] coordenates;
    
    public string[] Coordenates{
        get{
            return coordenates;
        }
    }
    
    int x = 0; 
    int y = 0;

    #endregion

    #region Setup
    /// <summary>
    /// Starts the grid
    /// </summary>
    public void StartGrid()
    {
        Debug.Log("Starting Grid " + NumOfButtonsToSpawn + " " + this.name);
        coordenates = new string[NumOfButtonsToSpawn];
        coordenates[0] = (0 + ":" + 0);
        for (int i = 1; i < coordenates.Length; i++)
        {
            // y++;
            // if(y == 5)
            // {
            //     y = 0;
            //     x = x +1;
            // }
            x++;
            if(x == 5)
            {
                x = 0;
                y = y + 1;
            }
            coordenates[i] = (x+ ":" + y);
        }
        SetupButtons();
        // BoatsSlot.InitBoatSlot();
    }
    
    public void disable(){
        onTableDisabled?.Invoke();
        
        // for each button in the list of buttons disable it
        foreach (ButtonManifest button in ListOfButtons)
        {
            button.GetComponent<Button>().interactable = false;
            // remove button hover effect
            button.GetComponent<Button>().GetComponent<EventTrigger>().enabled = false;

        }
    }

    public void enable(){
        onTableEnabled?.Invoke();
        // for each button in the list of buttons disable it
        foreach (ButtonManifest button in ListOfButtons)
        {
            button.GetComponent<Button>().interactable = true;

            // return button hover effect
            button.GetComponent<Button>().GetComponent<EventTrigger>().enabled = true;
        }
    }

    /// <summary>
    /// Setups the grid buttons.
    /// </summary>
    void SetupButtons()
    {
        ListOfButtons = new ButtonManifest[NumOfButtonsToSpawn];

        for (int i = 0; i < ListOfButtons.Length; i++)
        {
            ButtonManifest ButtonInstance = Instantiate(ButtonTemplate, buttonsGrid);
            ListOfButtons[i] = ButtonInstance;
            
            Player buttonOwner = PlayersPanelControl.instance.GetPlayer(0);
            ListOfButtons[i].SetText(coordenates[i]);
            
            string[] splitArray =  coordenates[i].Split(char.Parse(":"));

            ListOfButtons[i].x = System.Convert.ToInt32(splitArray[0]);
            ListOfButtons[i].y = System.Convert.ToInt32(splitArray[1]);
            
            ListOfButtons[i].SetButtonOwner(buttonOwner);
            ListOfButtons[i].SetPanelParent(this);
            ListOfButtons[i].UpdateState(ButtonState.Idle);
            ListOfButtons[i].GetComponent<Button>().interactable = interactable;
            ListOfButtons[i].ButtonAttackCoordenates = coordenates[i];
        }
    }
    #endregion

    #region Utils

    /// <summary>
    /// Returns a button on the grid by its coordenates.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public ButtonManifest GetButtonByPosition(string position)
    {
        int index = 0;
        for (int i = 0; i < ListOfButtons.Length; i++)
        {    
            string coord = ListOfButtons[i].getCoordenates();
            if (coord == position)
            {
                index = i;
                Debug.Log("[ButtonGridSpawner] Button was found with this position: " + position);
                break;
            }
        }
        return ListOfButtons[index];
    }
    
    /// <summary>
    /// Returns a button on the grid by its coordenates.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public ButtonManifest GetButtonByPosition(int x, int y)
    {
        int index = 0;
        for (int i = x; i < ListOfButtons.Length; i++)
        {
            if (ListOfButtons[i].x == x && ListOfButtons[i].y ==  y)
            {
                index = i;
                Debug.Log("[ButtonGridSpawner] Button was found with this position: " + x + ":" + y);
                break;
            }
        }
        return ListOfButtons[index];
    }

    public void SwitchTableToAttack()
    {
        for (int i = 0; i < ListOfButtons.Length; i++)
        {
            if (ListOfButtons[i].HasGotHiddenShip())
            {
                ListOfButtons[i].UpdateState(ButtonState.Idle);
            }
        }

        enable();
    }

    public void SwitchTableToViewMode()
    {
        for (int i = 0; i < ListOfButtons.Length; i++)
        {
            if (ListOfButtons[i].HasGotHiddenShip())
            {
                ListOfButtons[i].RestoreButtonState();
            }
        }

        disable();
    }

    public int GetNumberOfBoats()
    {
        int counter = 0;
        for (int i = 0; i < ListOfButtons.Length; i++)
        {
            if (ListOfButtons[i].HasGotHiddenShip())
            {
                counter++;
            }
        }

        return counter;
    }

    #region ScreenShot

    public void TakeScreenshotOfPlayAndSentToBot()
    {
        StartCoroutine(TakeSnapshot());
    }
    WaitForSeconds waitTime = new WaitForSeconds(0.1F);
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return waitTime;
        yield return frameEnd;


        var width = 700;
        var height = 600;
        var startX = 0;
        var startY = 0;


        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        TelegramServerRequesterHelper.SendImageToBot(tex.EncodeToJPG(),"game_table",this);
        Destroy(tex);
    }

    #endregion
    
    #endregion
    
}
