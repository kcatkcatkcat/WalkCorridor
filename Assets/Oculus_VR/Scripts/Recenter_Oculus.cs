using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

public class Recenter_Oculus : MonoBehaviour
{
    //[SerializeField]
    public GameObject fade;
    private Color color;

	void Start ()
	{
		fade = GameObject.Find(gameObject.name + "/Fade");
        color = fade.GetComponent<MeshRenderer>().material.color;
	}
	

	public void ResetCamera ()
	{
		InputTracking.Recenter ();
		Debug.Log ("Reset");
	}

	public IEnumerator FadeIn ()
	{
        float time = 0;
        Debug.Log("Fade In Oculus");
        fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1);        
        while (time <= 1f)
        {
            time += Time.deltaTime;
            fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1 - time);
            yield return null;
        }
        fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    }

	public IEnumerator FadeOut ()
	{
        float time = 0;
        Debug.Log("Fade Out Oculus");
        fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
        while (time <= 1f)
        {
            time += Time.deltaTime;
            fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, time);
            yield return null;
        }
        fade.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 1);
    }

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}