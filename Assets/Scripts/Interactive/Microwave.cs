using UnityEngine;
using System.Collections;

public class Microwave: Interactive
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
            message = "Close Microwave Door";
        }
        else
        {
            message = "Open Microwave Door";
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