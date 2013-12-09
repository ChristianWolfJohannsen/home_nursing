using UnityEngine;
using System.Collections;

public class OpenFridge : MonoBehaviour
{
    bool isFridgeOpen = false;
    public AnimationClip animationClip;

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void OnMouseDown()
    {
        Debug.Log("You clicked fridge door");
        if (Input.GetMouseButtonDown(0) && isFridgeOpen == false)
        {
            Debug.Log("It plays the opening animation!!");
            PlayForwardsFridge();
            isFridgeOpen = true;

        }
        else if (Input.GetMouseButtonDown(0) && isFridgeOpen == true)
        {
            Debug.Log("It plays the closing animation!");
            PlayBackwardsFridge();
            isFridgeOpen = false;
        }
    }



    void PlayForwardsFridge()
    {
        animation["OpenFridge"].speed = 1.0f;
        animation.Play("OpenFridge");
    }

    void PlayBackwardsFridge()
    {
        animation["OpenFridge"].speed = -1.0f;
        animation["OpenFridge"].time = animation["OpenFridge"].length;
        animation.Play("OpenFridge");
    }
}