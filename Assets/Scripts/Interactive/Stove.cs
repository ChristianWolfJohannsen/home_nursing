using UnityEngine;
using System.Collections;

public class Stove : Interactive
{
    protected bool isDoorOpen = false;
    public void Awake()
    {
       
    }

    public override string GetInteractionMessage()
    {
        string message = "";
        if (isDoorOpen)
        {
            message = "Close Oven Door";
        }
        else
        {
            message = "Open Oven Door";
        }
        return message;
    }

    public override void Interact()
    {
        if (isDoorOpen)
        {
            this.transform.animation.Play("close");
        }
        else
        {
            this.transform.animation.Play("open");
        }
        isDoorOpen = !isDoorOpen;
       
    }
}