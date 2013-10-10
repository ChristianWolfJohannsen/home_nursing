using UnityEngine;
using System.Collections;

public class InteractiveMessageGUI : MonoBehaviour
{
    public void Awake()
    {
        visible = false;
    }

    public string message
    {
        get
        {
            return this.guiText.text;
        }
        set
        {
            this.guiText.text = value;
        }
    }

    public bool visible
    {
        get
        {
            return base.gameObject.active;
        }
        set
        {
           base.gameObject.active = value;
        }

    }
}