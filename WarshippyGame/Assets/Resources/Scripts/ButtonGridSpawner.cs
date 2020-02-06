using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonGridSpawner : MonoBehaviour
{
    public ButtonManifest[] ListOfButtons;
    public int NumOfButtonsToSpawn;
    public Sprite WaterImage;
    public ButtonManifest ButtonTemplate;
    public string[] coordenates;
    //public HelloClient HelloClient;
    public static ButtonGridSpawner instance = null;
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
    }
    // Start is called before the first frame updateç
    int x = 0;
    int y = 0;
    void Start()
    {
        //HelloClient.OnNewMessageReceived = OnNewMessageReceived;
        coordenates = new string[25];
        coordenates[0] = (0 + ":" + 0);
        for (int i = 1; i < coordenates.Length; i++)
        {
            y++;
            if(y == 5)
            {
                y = 0;
                x = x +1;
            }
            coordenates[i] = (x+ ":" + y);
        }
        startButtons();
    }
    void OnNewMessageReceived(string message)
    {
        InfoPanelManager.instance.SpawnInfoMessage(message);
    }
    void startButtons()
    {
        ListOfButtons = new ButtonManifest[NumOfButtonsToSpawn];
        ButtonTemplate.transform.parent = this.transform;
        ListOfButtons[0] = ButtonTemplate;
       
        //ListOfButtons[0].PythonClient = HelloClient;
        ListOfButtons[0].ButtonGridImage.sprite = WaterImage;
        ListOfButtons[0].ButtonAttackCoordenates = coordenates[0];
        ListOfButtons[0].setText(coordenates[0]);
        for (int i = 1; i < ListOfButtons.Length; i++)
        {
            //Debug.Log(i);
            ButtonManifest ButtonInstance = Instantiate(ButtonTemplate, this.gameObject.transform);
            ListOfButtons[i] = ButtonInstance;
            //ListOfButtons[i].PythonClient = HelloClient;
            bool randomBoolean = (Random.value > 0.5f);
            ListOfButtons[i].setHiddenShip(randomBoolean);
            ListOfButtons[i].ButtonGridImage.sprite = WaterImage;
            ListOfButtons[i].ButtonAttackCoordenates = coordenates[i];
            ListOfButtons[i].setText(coordenates[i]);
        }
    }
    private ButtonManifest getButtonByPosition(string pos)
    {
        int index = 0;
        for (int i = 0; i < ListOfButtons.Length; i++)
        {    
            string coord = ListOfButtons[i].getCoordenates();
            if (coord == pos)
            {
                index = i;
                Debug.Log("[ButtonGridSpawner] Button was found with this position: " + pos);
            }
        }
        return ListOfButtons[index];
    }
    
    public void attackAtPosition(string pos)
    {
        ButtonManifest CurrentButton =  getButtonByPosition(pos);
        Debug.Log("[ButtonGridSpawner] Attacking at this position: " + pos);
        if (CurrentButton.hasGotHiddenShip())
        {
            ButtonState state = ButtonState.ShipDown;
            CurrentButton.UpdateState(state);
        }
        else
        {
            ButtonState state = ButtonState.WaterDown;
            CurrentButton.UpdateState(state);
        }
        
    }
}
