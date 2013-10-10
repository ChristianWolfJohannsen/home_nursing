using UnityEngine;
using System.Collections;

public class Fridge : Interactive
{
    FridgeMainDoor mainDoor;
    FridgeFreezerDoor freezerDoor;

    public void Awake()
    {
        mainDoor = this.GetComponentInChildren<FridgeMainDoor>();
        freezerDoor = this.GetComponentInChildren<FridgeFreezerDoor>();
    }

    public override string GetInteractionMessage()
    {
        return "BROKEN";
    }

    public override void Interact()
    {
        mainDoor.Interact();       
    }

    public void closeMainDoor()
    {
        mainDoor.closeDoor();
    }
    public void closeFreezerDoor()
    {
        freezerDoor.closeDoor();
    }
}