using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class Recenter_Oculus : MonoBehaviour
{
    //[SerializeField]
    public GameObject fade;

	void Start ()
	{
		//fade = GameObject.Find(gameObject.name + "/Fade");
	}
	

	public void ResetCamera ()
	{
		InputTracking.Recenter ();
		Debug.Log ("Reset");
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