using UnityEngine;
using System.Collections;

public class questTriggerMad : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("check");
            Parley.GetInstance().TriggerQuestEvent("fandtIngenMad");
        }
    }
}