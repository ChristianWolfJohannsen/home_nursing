using UnityEngine;
using System.Collections;

public class OpenFreezerDoor : MonoBehaviour
{
    bool isDoorOpen = false;
    public AnimationClip animationClip;

    // Use this for initialization
    void Start()
    {
        

    }

    // Update is called once per frame
    void OnMouseDown()
    {
        Debug.Log("You clicked freezer door!");
        if (Input.GetMouseButtonDown(0) && isDoorOpen == false)
        {
            Debug.Log("It plays the opening animation!!");
            PlayForwards();
            isDoorOpen = true;

        }
        else if (Input.GetMouseButtonDown(0) && isDoorOpen == true)
        {
            Debug.Log("It plays the closing animation!");
            PlayBackwards();
            isDoorOpen = false;
        }
    }



    void PlayForwards()
    {
        animation["OpenFreezer"].speed = 1.0f;
        animation.Play("OpenFreezer");
    }

    void PlayBackwards()
    {
        animation["OpenFreezer"].speed = -1.0f;
        animation["OpenFreezer"].time = animation["OpenFreezer"].length;
        animation.Play("OpenFreezer");
    }
}