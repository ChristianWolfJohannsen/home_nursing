using UnityEngine;
using System.Collections;

public class Sink : CounterTop
{
    public override string GetInteractionMessage()
    {
        string message = "";
        if (isDoorOpen)
        {
            message = "Close Sink Door";
        }
        else
        {
            message = "Open Sink Door";
        }
        return message;
    }
}