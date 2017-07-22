using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class Recenter_Vive : MonoBehaviour
{
	//[SerializeField]
	public GameObject fade;
    private float fadeTime;
	void Start ()
	{
		fade = GameObject.Find (gameObject.name +  "/Camera(head)/Camera(eye)/Fade");
        fadeTime = 2.0f;
    }


	public void ResetCamera ()
	{
		SteamVR.instance.hmd.ResetSeatedZeroPose ();
		Debug.Log ("ResetPos");
	}

	public void FadeIn ()
	{
        SteamVR_Fade.Start(Color.black, fadeTime);
	}

	public void FadeOut ()
	{
        SteamVR_Fade.Start(Color.clear, fadeTime);
	}

    public void SceneChange(string scene)
    {
        SteamVR_LoadLevel.Begin(scene);
    }


}