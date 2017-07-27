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
    public bool isScore = false;  //scoreをつけるかどうか
    [SerializeField]
    private List<int> conductedStimuliNum;//すでに実行された刺激番号
    private Animator anim;
	//被験者キャラクターのアニメーター
	[SerializeField]
	private AudioSource footSoundLeft;
	[SerializeField]
	private AudioSource footSoundRight;
	[SerializeField]
	private float stride = 0;//重複歩数
	[SerializeField]
	private int stimuli;//刺激番号
	private int stimuliNum　= 7;//刺激数(vision, kinesthetic, electricalの組み合わせ[ただし全てfalseは除く])
	[SerializeField]
	private int experimentNum = 0;//実験番号－１ ※最後の実験後0をExperimentParamatersに送る
	public string participantName;//被験者の名前
	private float maxStride = 27;//通路末端までの重複歩数
	private bool isWalk = false;
	public float ch1;
	public float ch2;
	public float ch3;
	public float ch4;
	public float ch5;
	public float ch6;
	public float ch7;
	public float ch8;

	private float WAIT_TIME = 0.4f;//映像と下肢駆動装置がずれる場合、要調整
	[SerializeField]
	private bool vision;
	[SerializeField]
	private bool kinesthetic;
	[SerializeField]
	private bool electrical;
	public bool lastStimulation;

	private AudioSource audioSourse;
	public AudioClip clip1;
	public AudioClip clip2;
	public AudioClip clip3;
    
    public HMD_TYPE hmd_type;
	public Recenter_Vive recenter_Vive;
    public Recenter_Oculus recenter_Oculus;


    
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
        getExperimentInfo();  //ExperimentParamatersの取得
        anim = GetComponent<Animator> ();//被験者キャラクターのアニメーター取得
		footSoundLeft = GameObject.Find (gameObject.name + "/Genesis 2 Male/hip/pelvis/lThigh/lShin/lFoot").GetComponent<AudioSource> ();
		footSoundRight = GameObject.Find (gameObject.name + "/Genesis 2 Male/hip/pelvis/rThigh/rShin/rFoot").GetComponent<AudioSource> ();
        switch (hmd_type)
        {
            case HMD_TYPE.Oculus:
                GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/Camera").SetActive(true);
                GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/[CameraRig]").SetActive(false);
                recenter_Oculus = GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/Camera").GetComponent<Recenter_Oculus>();
                break;

            case HMD_TYPE.Vive:
                GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/Camera").SetActive(false);
                GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/[CameraRig]").SetActive(true);
                recenter_Vive = GameObject.Find(gameObject.name + "/Genesis 2 Male/G2FSimplifiedEyes/[CameraRig]").GetComponent<Recenter_Vive>();
                break;
        }
        
        

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
        StartCoroutine(StartStimulation(2.0f));
	}

	

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q) || stride == maxStride) {
            EndStimulation();
        }

		if (Input.GetKeyDown (KeyCode.R)) {
            All_HMD_CameraReset();
		}

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.HumanoidWalk") && isWalk) {
			stride = Mathf.Floor (anim.GetCurrentAnimatorStateInfo (0).normalizedTime);

		}

		if (isWalk)
			UDPsend ();

	}

    void OnApplicationQuit()
    {
        Initialize();
        client1.Close();
        client2.Close();
    }

    private IEnumerator StartStimulation(float waitTime)
    {
        Debug.Log("StartStimulation");
        stimuli = randomStimuliGenerator(stimuliNum);
        yield return new WaitForSeconds(waitTime);
        if (vision) All_HMD_FadeIn();
        audioSourse.clip = clip1;
        audioSourse.Play();
        byte[] sceneCommand = new byte[6];
        sceneCommand[0] = 0x31;  // change scene command
        sceneCommand[1] = 0x01;  // number of data
        sceneCommand[2] = 0x00;  // number of data
        sceneCommand[3] = 0x00;  // number of data
        sceneCommand[4] = 0x00;  // number of data
        sceneCommand[5] = 0x01;  // number of data
        if (kinesthetic)
            client1.Send(sceneCommand, sceneCommand.Length);

        StartCoroutine(WalkTrue(WAIT_TIME));
    }

    private void EndStimulation()
    {
        stride = 0;//重複歩の初期化
        byte[] sceneCommand = new byte[6];
        sceneCommand[0] = 0x31;  // change scene command
        sceneCommand[1] = 0x01;  // number of data
        sceneCommand[2] = 0x00;  // number of data
        sceneCommand[3] = 0x00;  // number of data
        sceneCommand[4] = 0x00;  // number of data
        sceneCommand[5] = 0x00;  // number of data
        if (kinesthetic)
            client1.Send(sceneCommand, sceneCommand.Length);

        if (lastStimulation)
        {
            audioSourse.clip = clip3;
            audioSourse.Play();

            experimentNum = 0;
        }
        else
        {
            
            audioSourse.clip = clip2;
            audioSourse.Play();
            experimentNum += 1;
        }

        
        isWalk = false;
        anim.SetBool("isWalk", isWalk);
        if(vision)
        {
            Debug.Log("FadeOut");
            All_HMD_FadeOut();
        }
        Initialize();
        if (isScore) StartCoroutine(SceneChange(2.0f, hmd_type));
        else
        {
            StartCoroutine(ResetPosition(2.0f));
            StartCoroutine(StartStimulation(2.5f));
        }
    }

	private IEnumerator ResetPosition (float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		transform.position = new Vector3 (0, 0, 0);
        All_HMD_CameraReset();
	}

	private IEnumerator WalkTrue (float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		isWalk = true;
		anim.SetBool ("isWalk", isWalk);


	}

    private IEnumerator SceneChange(float waitTime, HMD_TYPE hmd_Type)
    {
        giveExperimentInfo();
        yield return new WaitForSeconds(waitTime);
        switch (hmd_Type)
        {
            case HMD_TYPE.Oculus:
                recenter_Oculus.SceneChange("ScoreSheet");
                break;
            case HMD_TYPE.Vive:
                recenter_Vive.SceneChange("ScoreSheet");
                break;
        }
    }

    

    private void Initialize()
    {
        ch1 = 0;
        ch2 = 0;
        ch3 = 0;
        ch4 = 0;
        ch5 = 0;
        ch6 = 0;
        ch7 = 0;
        ch8 = 0;
        UDPsend();
    }

    public void UDPsend()
    {
        sendParameter[0] = ch1;
        sendParameter[1] = ch2;
        sendParameter[2] = ch3;
        sendParameter[3] = ch4;
        sendParameter[4] = ch5;
        sendParameter[5] = ch6;
        sendParameter[6] = ch7;
        sendParameter[7] = ch8;

        sendMessage = "PCServer" + ",";//送信メッセージの先頭は"PCServer"

        for (int i = 0; i < 7; i++)
        {
            sendMessage += sendParameter[i].ToString() + ",";
        }
        sendMessage += sendParameter[7].ToString() + "\0";
        byte[] dgram = Encoding.UTF8.GetBytes(sendMessage);
        if (electrical)
            client2.Send(dgram, dgram.Length);

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
        switch (hmd_type)
        {
            case HMD_TYPE.Vive:
                recenter_Vive.ResetCamera();
                break;

            case HMD_TYPE.Oculus:
                recenter_Oculus.ResetCamera();
                break;

            case HMD_TYPE.None:
                break;
        }
    }

    private void All_HMD_FadeIn()
    {
        switch (hmd_type)
        {
            case HMD_TYPE.Vive:
                recenter_Vive.FadeIn();
                break;

            case HMD_TYPE.Oculus:
                StartCoroutine(recenter_Oculus.FadeIn());
                break;

            case HMD_TYPE.None:
                break;
        }
    }

    private void All_HMD_FadeOut()
    {
        switch (hmd_type)
        {
            case HMD_TYPE.Vive:
                recenter_Vive.FadeOut();
                break;

            case HMD_TYPE.Oculus:
                StartCoroutine(recenter_Oculus.FadeOut());
                break;

            case HMD_TYPE.None:
                break;
        }
    }
    private static void ThreadMethod ()
	{
		while (true) {

		}
	}

	private void giveExperimentInfo(){//ExperimentParamatersにパラメータを格納
        Debug.Log("give experiment paramaters");
        ExperimentParamaters.ParticipantName = participantName;
		ExperimentParamaters.ExperimentNum = experimentNum;
        ExperimentParamaters.Stimuli = stimuli;
        ExperimentParamaters.StimuliNum = stimuliNum;
        ExperimentParamaters.ConductedStimuliNum = conductedStimuliNum;
        ExperimentParamaters.HMD_Type = hmd_type;
        ExperimentParamaters.IsScore = isScore;
    }

    private void getExperimentInfo()
    {
        Debug.Log("get experiment paramaters");
        conductedStimuliNum = ExperimentParamaters.ConductedStimuliNum;
        isScore = ExperimentParamaters.IsScore;
        experimentNum = ExperimentParamaters.ExperimentNum;

    }

	private void stimuliCombination(int num){//刺激番号とそれぞれの刺激の対応
		switch (num) {
		case 0:
			vision = false;
			kinesthetic = false;
			electrical = true;
			break;

		case 1:
			vision = false;
			kinesthetic = true;
			electrical = false;
			break;

		case 2:
			vision = false;
			kinesthetic = true;
			electrical = true;
			break;

		case 3:
			vision = true;
			kinesthetic = false;
			electrical = false;
			break;

		case 4:
			vision = true;
			kinesthetic = false;
			electrical = true;
			break;

		case 5:
			vision = true;
			kinesthetic = true;
			electrical = false;
			break;

		case 6:
			vision = true;
			kinesthetic = true;
			electrical = true;
			break;
		}
	}

	public int randomStimuliGenerator(int num){//提示刺激のランダム生成機
		int x = new int();
		x = Random.Range (0, num);
		while (conductedStimuliNum.Contains (x)) {
			x = Random.Range (0, num);
		}
		conductedStimuliNum.Add (x);
        stimuliCombination(x);
        if (conductedStimuliNum.Count == num)
        {
            lastStimulation = true;
            conductedStimuliNum.Clear();
        }
        else lastStimulation = false;
		return x;
	}
}
