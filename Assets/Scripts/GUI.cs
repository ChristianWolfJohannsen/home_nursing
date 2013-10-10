using UnityEngine;
using System.Collections;

public class GUI : MonoBehaviour
{
    public Texture cursorTextureOff;
    public Texture cursorTextureOver;
    public Texture leftMouseClickTexture;

    private Texture currCursorTexture;
    private Interactive currInteractiveObject;
    private InteractiveMessageGUI interactiveMessageScript;

    public void Awake()
    {
        Screen.showCursor = false;
        interactiveMessageScript = this.transform.FindChild("InteractiveMessage").GetComponent<InteractiveMessageGUI>();
       
        interactiveMessageScript.visible = false;
        interactiveMessageScript.gameObject.transform.position = new Vector3(0.05f, 0.85f, 0); //position GUI in correct position, so it can be hidden at start
    }
    public void Start()
    {
        currCursorTexture = cursorTextureOff;
    }

    public void Update()
    {
        Ray cursorRayCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2));
        RaycastHit hit;
        Transform currentTransformObj;
        currInteractiveObject = null;
        if (Physics.Raycast(cursorRayCenter, out hit, Mathf.Infinity)) //find first object lined up with the cursor
        {
            currentTransformObj =  hit.collider.transform; //don't use hit.transform as if you make a parent a ridigbody it will return that instead of objects within
            //this loops through to see if this gameobject or the parent of this gameobject is interactive
            do
            {
                currInteractiveObject = currentTransformObj.GetComponent<Interactive>();//if it is interactive set to to the current interactive object
                currentTransformObj = currentTransformObj.parent;
            } while (currInteractiveObject==null && currentTransformObj!=null);
            

            
        }

        TestScreenCursorLock();
        Interact();
    }

    private void TestScreenCursorLock()
    {
        if (!Screen.lockCursor && Input.GetMouseButtonDown(0))
        {
            Screen.lockCursor = true;
        }
    }

    private void Interact()
    {
        if (currInteractiveObject != null && Input.GetMouseButtonDown(0)) //if interactive object available and left mouse button down
        {
            currInteractiveObject.Interact();
        }
    }

    public void OnGUI()
    {
        if (currInteractiveObject != null)
        {
            currCursorTexture = cursorTextureOver;
            interactiveMessageScript.message = currInteractiveObject.GetInteractionMessage();
            interactiveMessageScript.visible = true;
            
        }
        else
        {
            currCursorTexture = cursorTextureOff;
            interactiveMessageScript.visible = false;
        }

        GUILayout.BeginArea(new Rect((Screen.width - currCursorTexture.width) / 2, (Screen.height - currCursorTexture.height) / 2, currCursorTexture.width, currCursorTexture.height));
        GUILayout.Label(currCursorTexture);
        GUILayout.EndArea();
    }
}