using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.SceneManagement;


public class Recenter_Vive : MonoBehaviour
{
	public GameObject fade;
	private float fadeTime;

	void Start ()
	{
		//fade = GameObject.Find (gameObject.name +  "/Camera(head)/Camera(eye)/Fade");
		fadeTime = 2.0f;
	}

	public void ResetCamera ()
	{
		GameObject.Find ("[CameraRig]").transform.position -= (GameObject.Find ("[CameraRig]/Camera (head)/Camera (eye)").transform.position - GameObject.Find ("HeadPos").transform.position);
		Debug.Log ("ResetPos");
	}
	/*
	public void FadeIn ()
	{
        Debug.Log("Fade In Vive");
        SteamVR_Fade.Start(Color.black, fadeTime);
	}

	public void FadeOut ()
	{
        Debug.Log("Fade Out Vive");
        SteamVR_Fade.Start(Color.clear, fadeTime);
	}
    */
    
	public IEnumerator FadeIn ()
	{
		Debug.Log ("Fade In Vive");
		float time = 0;
		fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 1);
		while (time <= 1f) {
			time += Time.deltaTime;
			fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 1 - time);
			yield return null;
		}
		fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 0);
	}

	public IEnumerator FadeOut ()
	{
		Debug.Log ("Fade Out Vive");
		float time = 0;
		fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 0);
		while (time <= 1f) {
			time += Time.deltaTime;
			fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, time);
			yield return null;
		}
		fade.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 1);
	}

	public void SceneChange (string scene)
	{
		SteamVR_LoadLevel.Begin (scene);
	}

}