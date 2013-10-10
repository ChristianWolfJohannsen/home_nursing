using UnityEngine;
using System.Collections;

public abstract class Interactive : MonoBehaviour
{
    public abstract string GetInteractionMessage();
    public abstract void Interact();
}