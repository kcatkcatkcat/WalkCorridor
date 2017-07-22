using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class Recenter_Vive : MonoBehaviour
{
	//[SerializeField]
	public GameObject fade;

	void Start ()
	{
		fade = GameObject.Find (gameObject.name +  "/Camera(head)/Camera(eye)/Fade");
	}

	public void ResetCamera ()
	{
		SteamVR.instance.hmd.ResetSeatedZeroPose ();
		Debug.Log ("ResetPos");
	}

	public void FadeIn ()
	{
		iTween.FadeTo (fade, 0, 2f);
	}

	public void FadeOut ()
	{
		iTween.FadeTo (fade, 1f, 2f);
	}
}