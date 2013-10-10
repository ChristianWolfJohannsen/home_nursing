using UnityEngine;
using System.Collections;

public class CounterTop : Interactive
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
            message = "Close Countertop Door";
        }
        else
        {
            message = "Open Countertop Door";
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