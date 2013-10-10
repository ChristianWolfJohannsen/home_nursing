using UnityEngine;
using System.Collections;

public class FridgeMainDoor : Interactive
{
    Fridge fridge;
    bool isDoorOpen = false;
    public void Awake()
    {
        setFridge();
    }

    private void setFridge()
    {
        Transform nextTransform = this.transform.parent;

        while (fridge == null && nextTransform != null)
        {
            fridge = nextTransform.GetComponent<Fridge>();
            nextTransform = this.transform.parent;
        }
    }
    public override string GetInteractionMessage()
    {
        string messageStr = "";

        if (isDoorOpen)
        {
            messageStr = "Close Main Door";
        }
        else
        {
            messageStr = "Open Main Door";
        }

        return messageStr;
    }

    public override void Interact()
    {
        fridge.closeFreezerDoor();
        if (isDoorOpen)
        {
            fridge.transform.animation.Play("closeBottom");
        }
        else
        {
            fridge.transform.animation.Play("openBottom");
        }
        isDoorOpen = !isDoorOpen;

    }

    public void closeDoor()
    {
        if (isDoorOpen)
        {
            fridge.transform.animation.Play("closeBottom");
            isDoorOpen = false;
        } 
    }
}