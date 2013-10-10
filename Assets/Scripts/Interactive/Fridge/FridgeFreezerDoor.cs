using UnityEngine;
using System.Collections;

public class FridgeFreezerDoor : Interactive
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
            messageStr = "Close Freezer Door";
        }
        else
        {
            messageStr = "Open Freezer Door";
        }

        return messageStr;
    }

    public override void Interact()
    {
        fridge.closeMainDoor();

        if (isDoorOpen)
        {
            fridge.transform.animation.Play("closeTop");
        }
        else
        {
            fridge.transform.animation.Play("openTop");
        }
        isDoorOpen = !isDoorOpen;

    }

    public void closeDoor()
    {
        if (isDoorOpen)
        {
            fridge.transform.animation.Play("closeTop");
            isDoorOpen = false;
        }
    }
}