using UnityEngine;
using System.Collections;

public class TV : Interactive
{
    private const string INTERACTIVE_MESSAGE = "Click To Change Channel";
    public int currTVChannel = 1;
    public Material[] tvChannelMaterials;
    public bool disableRenderTextureWhenNotInUse = false;
    private GameObject[] tvCameras;

    private MeshRenderer tvScreenMesh;
    private GameObject tvDial; 


    public void Awake()
    {
        tvScreenMesh = this.transform.FindChild("TVScreen").GetComponent<MeshRenderer>(); //get tv screen mesh
        tvDial = this.transform.FindChild("RetroTV").FindChild("Dial").gameObject; //get tv dial

        if (disableRenderTextureWhenNotInUse)
        {
            setupTVCamerasList();
        }

        checkAvailableTVChannel();
        updateTVChannel();
    }

    public override string GetInteractionMessage()
    {
        return INTERACTIVE_MESSAGE;
    }

    public override void Interact()
    {
        nextTVChannel();
    }

    //tests to make sure desired channel is not out of bounds
    private void checkAvailableTVChannel()
    {
        if (currTVChannel<0)
        {
           currTVChannel=0;
        }
        else if (currTVChannel >= tvChannelMaterials.Length)
        {
            currTVChannel = tvChannelMaterials.Length - 1;
        }
    }

    private void nextTVChannel()
    {
        currTVChannel++;
        if (currTVChannel >= tvChannelMaterials.Length)
        {
            currTVChannel = 0;
        }
        changeChannel();
    }

    private void previousTVChannel()
    {
        currTVChannel--;
        if (currTVChannel < 0)
        {
            currTVChannel = tvChannelMaterials.Length - 1;
        }
        changeChannel();
    }

    private void changeChannel()
    {
        updateTVChannel();
        updateDialRotation();
    }

    private void updateTVChannel()
    {
        tvScreenMesh.material = tvChannelMaterials[currTVChannel];
        if (tvScreenMesh.material.mainTexture is MovieTexture) //not used yet
        {
            if (!((MovieTexture)tvScreenMesh.material.mainTexture).isPlaying)
            {
                ((MovieTexture)tvScreenMesh.material.mainTexture).Play();
            }
        }
        else
        {
            //this is used to turn of render cameras used to render other view 
            // for the tv when they are not in use, do not enable if you are using multiple tvs
            if (disableRenderTextureWhenNotInUse)
            {
               disableRenderCamerasIfNotInUse(); 
            }
        }

    }

    private void disableRenderCamerasIfNotInUse()
    {
        for (int x = 0; x < tvCameras.Length; x++)
        {
            if (tvCameras[x] != null)
            {
               tvCameras[x].active = (x == currTVChannel);    
            }
        }
    }

    //setups on an array of all the cameras that are currently rendering textures for use on tv tv
    private void setupTVCamerasList()
    {
        Texture tvChannelMat;
        GameObject[] renderCameras = GameObject.FindGameObjectsWithTag("RenderCamera");
        tvCameras = new GameObject[tvChannelMaterials.Length];

        for (int x=0; x<tvChannelMaterials.Length; x++)
        {
            tvChannelMat = tvChannelMaterials[x].mainTexture;
            if (tvChannelMat is RenderTexture)
            {
                foreach (GameObject renderCamera in renderCameras)
                {
                    if (renderCamera.GetComponent<Camera>().targetTexture == tvChannelMat)
                    {
                        tvCameras[x] = renderCamera;
                    }
                }
            }
            else
            {
                tvCameras[x] = null;
            }
        }
    }

    private void updateDialRotation()
    {
        tvDial.transform.Rotate(Vector3.forward,(360 / tvChannelMaterials.Length)); 
    }
}