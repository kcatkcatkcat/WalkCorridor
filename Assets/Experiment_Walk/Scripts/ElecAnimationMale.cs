using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ElecAnimationMale : MonoBehaviour
{

	private Animator anim;
	//被験者キャラクターのアニメーター
	[SerializeField]
	private AudioSource footSoundLeft;
	[SerializeField]
	private AudioSource footSoundRight;
	[SerializeField]
	private float stride = 0;
	//重複歩数
	public int experimentNum = 0;
	/*
	public static int ExperimentNum{
		get{ return experimentNum;}
		private set{ experimentNum = value;}
	}
	*/
	public float maxStride;
	private bool isWalk = false;
	public float ch1;
	public float ch2;
	public float ch3;
	public float ch4;
	public float ch5;
	public float ch6;
	public float ch7;
	public float ch8;

	public float waitTime;
	public bool vision;
	public bool kinesthetic;
	public bool electrical;
	public bool lastStimulation;

	private AudioSource audioSourse;
	public AudioClip clip1;
	public AudioClip clip2;
	public AudioClip clip3;

	public Recenter_Vive recenter_Vive;
    public Recenter_Oculus recenter_Oculus;

    public enum HMD_Type
    {
        Vive,
        Oculus,
        None
    }

    public HMD_Type hmd_Type;
    /***********************************************/

    #region UDP関連パラメータ

    public string hostIP = "133.10.79.170";
	//電気刺激装置（Arduino）のIP
	private string footIP = "133.10.79.89";
	//研究室IP
	public int hostPort = 61000;
	static private UdpClient client1;
	static private UdpClient client2;
	private float[] sendParameter;
	//Arduinoへの送信パラメータ
	private int sendNumber;
	//研究室への送信番号
	private string sendMessage;
	//送信メッセージ

	#endregion

	/**********************************************/

	Thread thread;

   

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();//被験者キャラクターのアニメーター取得
		//footSoundLeft = transform.Find (gameObject.name + "/hip/pelvis/lThigh/lShin/lFoot").GetComponent<AudioSource> ();
		//footSoundRight = transform.Find (gameObject.name + "/hip/pelvis/rThigh/rShin/rFoot").GetComponent<AudioSource> ();
		client1 = new UdpClient ();
		client2 = new UdpClient ();

		/////////////UDP接続(研究室)//////////////////
		client1.Connect (footIP, hostPort);
		/////////////UDP接続(Arduino)////////////////
		client2.Connect (hostIP, hostPort);
		sendMessage = "";
		sendParameter = new float[8];

		/******************************************/
		#region マルチスレッド

		thread = new Thread (new ThreadStart (ThreadMethod));
        //thread.Start();

        #endregion
        /******************************************/

        All_HMD_CameraReset();
        audioSourse = GetComponent<AudioSource> ();

        if (vision)
            All_HMD_FadeIn();
	}

	void OnApplicationQuit ()
	{
		Initialize ();
		client1.Close ();
		client2.Close ();
	}

	private void Initialize ()
	{
		ch1 = 0;
		ch2 = 0;
		ch3 = 0;
		ch4 = 0;
		ch5 = 0;
		ch6 = 0;
		ch7 = 0;
		ch8 = 0;
		UDPsend ();
	}

	public void UDPsend ()
	{
		sendParameter [0] = ch1;
		sendParameter [1] = ch2;
		sendParameter [2] = ch3;
		sendParameter [3] = ch4;
		sendParameter [4] = ch5;
		sendParameter [5] = ch6;
		sendParameter [6] = ch7;
		sendParameter [7] = ch8;

		sendMessage = "PCServer" + ",";//送信メッセージの先頭は"PCServer"

		for (int i = 0; i < 7; i++) {
			sendMessage += sendParameter [i].ToString () + ",";
		}
		sendMessage += sendParameter [7].ToString () + "\0";
		//Debug.Log (sendMessage);
		byte[] dgram = Encoding.UTF8.GetBytes (sendMessage);
		if (electrical)
			client2.Send (dgram, dgram.Length);

	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S)) {
			audioSourse.clip = clip1;
			audioSourse.Play ();
			byte[] sceneCommand = new byte[6];
			sceneCommand [0] = 0x31;  // change scene command
			sceneCommand [1] = 0x01;  // number of data
			sceneCommand [2] = 0x00;  // number of data
			sceneCommand [3] = 0x00;  // number of data
			sceneCommand [4] = 0x00;  // number of data
			sceneCommand [5] = 0x01;  // number of data
			if (kinesthetic)
				client1.Send (sceneCommand, sceneCommand.Length);

			StartCoroutine ("WalkTrue", waitTime);


		} else if (Input.GetKeyDown (KeyCode.Q) || stride == maxStride) {
			stride = 0;//重複歩の初期化
			if (lastStimulation) {
				experimentNum = 0;
				ExperimetParamaters.ExperimentNum = experimentNum;
				audioSourse.clip = clip3;
				audioSourse.Play ();
			} else {
				experimentNum += 1;
				ExperimetParamaters.ExperimentNum = experimentNum;
				audioSourse.clip = clip2;
				audioSourse.Play ();
			}
			byte[] sceneCommand = new byte[6];
			sceneCommand [0] = 0x31;  // change scene command
			sceneCommand [1] = 0x01;  // number of data
			sceneCommand [2] = 0x00;  // number of data
			sceneCommand [3] = 0x00;  // number of data
			sceneCommand [4] = 0x00;  // number of data
			sceneCommand [5] = 0x00;  // number of data
			if (kinesthetic)
				client1.Send (sceneCommand, sceneCommand.Length);

			isWalk = false;
			anim.SetBool ("isWalk", isWalk);
            All_HMD_FadeOut();
			StartCoroutine ("ResetPosition");

			Initialize ();

		}

		if (Input.GetKeyDown (KeyCode.R)) {
            All_HMD_CameraReset();
		}

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.HumanoidWalk")) {
			stride = Mathf.Floor (anim.GetCurrentAnimatorStateInfo (0).normalizedTime);
		}

		if (isWalk)
			UDPsend ();

	}

	private IEnumerator ResetPosition ()
	{
		yield return new WaitForSeconds (2f);
		transform.position = new Vector3 (0, 0, 0);
        All_HMD_CameraReset();
		yield return new WaitForSeconds (2f);
        if (vision) All_HMD_FadeIn();
        
	}

	private IEnumerator WalkTrue (float wait)
	{
		yield return new WaitForSeconds (wait);
		Debug.Log ("Co");
		Debug.Log (wait);
		isWalk = true;
		anim.SetBool ("isWalk", isWalk);


	}

	public void FootSoundLeft ()
	{
		footSoundLeft.Play ();
	}

	public void FootSoundRight ()
	{
		footSoundRight.Play ();
	}

    private void All_HMD_CameraReset()
    {
        switch (hmd_Type)
        {
            case HMD_Type.Vive:
                recenter_Vive.ResetCamera();
                break;

            case HMD_Type.Oculus:
                recenter_Oculus.ResetCamera();
                break;

            case HMD_Type.None:
                break;
        }
    }

    private void All_HMD_FadeIn()
    {
        switch (hmd_Type)
        {
            case HMD_Type.Vive:
                recenter_Vive.FadeIn();
                break;

            case HMD_Type.Oculus:
                recenter_Oculus.FadeIn();
                break;

            case HMD_Type.None:
                break;
        }
    }

    private void All_HMD_FadeOut()
    {
        switch (hmd_Type)
        {
            case HMD_Type.Vive:
                recenter_Vive.FadeOut();
                break;

            case HMD_Type.Oculus:
                recenter_Oculus.FadeOut();
                break;

            case HMD_Type.None:
                break;
        }
    }
    private static void ThreadMethod ()
	{
		while (true) {

		}
	}
}
