using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HumanWalk : MonoBehaviour
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
	public float maxStride;
	private bool isWalk = false;
	public float ch1;
	public float ch2;
	public float ch3;
	public float ch4;
	public float LeftTricepsSuraeMuscle;
	public float LeftTibialisAnteriorMuscle;
	public float LeftQuadricepsFemorisMuscle;
	public float LeftBicepsFemorisMuscle;
	public float RightTricepsSuraeMuscle;
	public float RightTibialisAnteriorMuscle;
	public float RightQuadricepsFemorisMuscle;
	public float RightBicepsFemorisMuscle;

	public GameObject leftTricepsSuraeMuscle;
	public GameObject leftTibialisAnteriorMuscle;
	public GameObject leftQuadricepsFemorisMuscle;
	public GameObject leftBicepsFemorisMuscle;
	public GameObject rightTricepsSuraeMuscle;
	public GameObject rightTibialisAnteriorMuscle;
	public GameObject rightQuadricepsFemorisMuscle;
	public GameObject rightBicepsFemorisMuscle;
    public GameObject Electrode1;
    public GameObject Electrode2;
    public GameObject Electrode3;
    public GameObject Electrode4;



    public float waitTime;
	public bool vision;
	public bool kinesthetic;
	public bool electrical;
	public bool lastStimulation;

	private AudioSource audioSourse;
	public AudioClip clip1;
	public AudioClip clip2;
	public AudioClip clip3;

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


   

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();//被験者キャラクターのアニメーター取得


	}

	void OnApplicationQuit ()
	{
		Initialize ();
		/*
		client1.Close ();
		client2.Close ();
		*/
	}

	private void Initialize ()
	{
		ch1 = 0;
		ch2 = 0;
		ch3 = 0;
		ch4 = 0;
		LeftTricepsSuraeMuscle = 0;
		LeftTibialisAnteriorMuscle = 0;
		LeftQuadricepsFemorisMuscle = 0;
		LeftBicepsFemorisMuscle = 0;
		RightTricepsSuraeMuscle = 0;
		RightTibialisAnteriorMuscle = 0;
		RightQuadricepsFemorisMuscle = 0;
		RightBicepsFemorisMuscle = 0;
		//UDPsend ();
	}



	// Update is called once per frame
	void Update ()
	{
		leftTricepsSuraeMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, LeftTricepsSuraeMuscle);
		leftTibialisAnteriorMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, LeftTibialisAnteriorMuscle);
		leftQuadricepsFemorisMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, LeftQuadricepsFemorisMuscle);
		leftBicepsFemorisMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, LeftBicepsFemorisMuscle);
		rightTricepsSuraeMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, RightTricepsSuraeMuscle);
		rightTibialisAnteriorMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, RightTibialisAnteriorMuscle);
		rightQuadricepsFemorisMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, RightQuadricepsFemorisMuscle);
		rightBicepsFemorisMuscle.GetComponent<Renderer> ().material.color = new Color (255 / 255, 250 / 255, 130 / 255, RightBicepsFemorisMuscle);

        Electrode1.GetComponent<Renderer>().material.color = new Color(255 / 255, 76 / 255, 60 / 255, ch1);
        Electrode2.GetComponent<Renderer>().material.color = new Color(255 / 255, 76 / 255, 60 / 255, ch3);
        Electrode3.GetComponent<Renderer>().material.color = new Color(255 / 255, 76 / 255, 60 / 255, ch2);
        Electrode4.GetComponent<Renderer>().material.color = new Color(255 / 255, 76 / 255, 60 / 255, ch4);

    }






}
