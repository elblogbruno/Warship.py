using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoatType {
    ATAGO = 0,
    ATLANTA = 1,
    MURMANKS = 2
}
public class Boat : MonoBehaviour
{
    public List<ButtonManifest> spawnedBoats;
    public Sprite[] parts;
    public string boat_name;

    public BoatType boatType;

    private ButtonManifest buttonManifest;
    private bool isMouseOver = false, isPlaced = false;

    public GameObject buttonCrosshair;
    public GameObject tipPanel;

    public GameObject boat;

    public int x{
        get{
            return buttonManifest.x;
        }
    }

    public int y{
        get{
            return buttonManifest.y;
        }
    }

    public string orientation {
        get{
            return buttonManifest.ButtonOrientation;
        }
    }

    void Start()
    {
        buttonManifest = GetComponent<ButtonManifest>();
        buttonCrosshair.SetActive(false);
        tipPanel.SetActive(false);
    }

    private void InitializeBoat()
    {
        boat_name = AINamesGenerator.Utils.GetRandomName();
        spawnedBoats = new List<ButtonManifest>();
        isPlaced = true;
    }

    public bool TryPlaceBoat(string boatType)
    {
        bool canBePlaced = false;

        if (CanPlaceBoat(x, y, "v"))
        {
            buttonManifest.ButtonOrientation = "v";
            canBePlaced = true;
        }
        else if (CanPlaceBoat(x, y, "h")) 
        {
            buttonManifest.ButtonOrientation = "h";
            canBePlaced = true;
        }

        if (canBePlaced)
        {
            InitializeBoat();

            SpawnBoat(x, y, orientation);
            buttonManifest.SetHiddenShip(true);
            // buttonManifest.ButtonGridImage.sprite = boatSprite;       
        }

        return canBePlaced;
    }

    public void DesinitializeBoat()
    {
        boat_name = ""; 
        
        isPlaced = false;

        UnspawnBoat();
    }

    private int counter = 0;
    public void UnspawnBoat()
    {
        bool isOwner = buttonManifest.isOwner;

        if (!isOwner)
        {
            int ownerX = buttonManifest.ownerX;
            int ownerY = buttonManifest.ownerY;
            
            Debug.Log("Owner X: " + ownerX + " Owner Y: " + ownerY);

            ButtonManifest owner = PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(ownerX, ownerY);

            if (owner != null)
            {
                Debug.Log("Owner is not null: " + owner.name +  " " + owner.x + "," + owner.y);
                // owner.boat.spawnedBoats.Remove(this.GetComponent<ButtonManifest>());
                owner.boat.UnspawnBoat();
            }
        }
        else
        {
            for (int i = 0; i < spawnedBoats.Count; i++)
            {
                UnspawnBoatPart(spawnedBoats[i]);
            }

            spawnedBoats.Clear();

            counter = 0;
        }
    }

    //function that spawns 3 boat parts based on orientation and if the boat is placed on the board
    public void SpawnBoat(int x, int y, string orientation)
    {
        Debug.Log("Spawning boat: " + CanPlaceBoat(x,y, orientation));
        if (CanPlaceBoat(x, y, orientation))
        {   
            int ownerX = x , ownerY = y;

            if (orientation == "h")
            {
                counter = 2;


                for (int i = x; i < x + 3; i++)
                {
                    SpawnBoatPart(i, y, ownerX, ownerY, i == x, false);
                }
            }
            else
            {
                for (int i = y; i < y + 3; i++)
                {
                    SpawnBoatPart(x, i, ownerX, ownerY,  i == y, true);   
                }
            }

            counter  = 0;
        }
        else
        {
            string s = "Boat can't be rotated {0}";
            InfoPanelManager.instance.SpawnInfoMessage(string.Format(s, orientation == "h" ? "horitzontally" : "vertically"));
        }
    }


    private void UnspawnBoatPart(ButtonManifest button)
    {
        //ButtonManifest button = ButtonGridSpawner.instance.GetButtonByPosition(x, y);
        button.isOwner = false;        
        button.SetNameOfBoatOwner(name);
        button.SetHiddenShip(false);
    }
    private void SpawnBoatPart(int x, int y, int ownerX, int ownerY, bool isOwner = false, bool vertical = false)
    {
        ButtonManifest button = PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(x, y);
        
        if (!isOwner)
        {
            Debug.Log("CHILD PART NOT OWNER: " + button.name + " " + x + "," + y);
            button.SetNameOfBoatOwner(name);
            button.SetOwnerCoordenates(ownerX, ownerY);
            button.isOwner = false;
        }
        else
        {
            button.isOwner = true;
            buttonCrosshair.SetActive(true);
        }

        button.SetCustomSprite(parts[counter], vertical);

        button.SetHiddenShip(true);

        spawnedBoats.Add(button);

        if (vertical)
            counter++;
        else
            counter--;
    }
    public void RespawnBoat(int x,int y, string orientation, string newOrientation)
    {
        UnspawnBoat();
        
        SpawnBoat(x,y, newOrientation);
    }

    // function that checks if the boat can be placed on the board based on the orientation
    public bool CanPlaceBoat(int x, int y, string orientation)
    {   
        if (spawnedBoats != null && spawnedBoats.Count > 0)
        {
            return false;
        }
        Debug.Log("Checking if boat can be placed on the board at " + x + " " + y);
        if (orientation == "v")
        {
            if (y + 3 > 5)
            {
                return false;
            }

            for (int i = y; i < y+3; i++)
            {
                Debug.Log(PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(x, i).HasGotHiddenShip());
                if (i != y && PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(x, i).HasGotHiddenShip())
                {
                    return false;
                }
            }
        }
        else
        {
            if (x + 3 > 5)
            {
                return false;
            }
            
            for (int i = x; i < x+3; i++)
            {
                Debug.Log(PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(i, y).HasGotHiddenShip());
                if (i != x && PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(i, y).HasGotHiddenShip())
                {
                    return false;
                }
            }
        }
        return true;
    }


    public void OnMouseOver()
    {
        isMouseOver = true;

        if (buttonCrosshair != null)
        {
            buttonCrosshair.SetActive(true);
        }

        if (tipPanel != null)
        {
            tipPanel.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        isMouseOver = false;

        if (buttonCrosshair != null)
        {
            buttonCrosshair.SetActive(false);
        }

        if (tipPanel != null)
        {
            tipPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isMouseOver && isPlaced)
        {
            Debug.Log("Mouse is over: " + this.name);

            if (Input.GetKeyDown(KeyCode.V) && buttonManifest.ButtonOrientation != "v")
            {
                buttonManifest.RotateButton(false);
                RespawnBoat(buttonManifest.x, buttonManifest.y, "h", "v");
            }
            else if (Input.GetKeyDown(KeyCode.H) && buttonManifest.ButtonOrientation != "h")
            {
                buttonManifest.RotateButton(true);
                RespawnBoat(buttonManifest.x, buttonManifest.y, "v", "h");
            }
        }
        else
        {
            Debug.Log("Mouse is not over: " + this.name);
        }
    }
}
