using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ElecAnimation : MonoBehaviour {

    private Animator anim;　//被験者キャラクターのアニメーター
    [SerializeField]
    private float stride=0;//重複歩数
    public float maxStride;
    private bool isWalk=false;
    public float ch1;
    public float ch2;
    public float ch3;
    public float ch4;
    public float ch5;
    public float ch6;
    public float ch7;
    public float ch8;

    /***********************************************/
    #region UDP関連パラメータ
    public string hostIP = "133.10.79.170";　//電気刺激装置（Arduino）のIP
    private string allIP = "133.10.79.255";　//研究室IP
    public int hostPort = 61000;
    static private UdpClient client1;
    static private UdpClient client2;
    private float[] sendParameter;//Arduinoへの送信パラメータ
    private int sendNumber;//研究室への送信番号
    private string sendMessage;//送信メッセージ

    #endregion
    /**********************************************/

    Thread thread;

   

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();//被験者キャラクターのアニメーター取得
        client1 = new UdpClient();
        client2 = new UdpClient();

        /////////////UDP接続(研究室)//////////////////
        client1.Connect(allIP, hostPort);
        /////////////UDP接続(Arduino)////////////////
        client2.Connect(hostIP, hostPort);
        sendMessage = "";
        sendParameter = new float[8];

        /******************************************/
        #region マルチスレッド

        thread = new Thread(new ThreadStart(ThreadMethod));
        //thread.Start();

        #endregion
        /******************************************/
        //Recenter.ResetCamera();
        //Recenter.FadeIn();

    }

    void OnApplicationQuit()
    {
        Initialize();
        client1.Close();
        client2.Close();
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
        Debug.Log(sendMessage);
        byte[] dgram = Encoding.UTF8.GetBytes(sendMessage);
        client2.Send(dgram, dgram.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            byte[] dgram = Encoding.UTF8.GetBytes("1");
            client1.Send(dgram, dgram.Length);

            isWalk = true;
            anim.SetBool("isWalk", isWalk);
            
        }else if (Input.GetKeyDown(KeyCode.Q) || stride>=maxStride)
        {
            byte[] dgram = Encoding.UTF8.GetBytes("0");
            client1.Send(dgram, dgram.Length);

            isWalk = false;
            anim.SetBool("isWalk", isWalk);

            //Recenter.FadeOut();
            StartCoroutine("ResetPosition");
            stride = 0;//重複歩の初期化
            Initialize();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Recenter.ResetCamera();
        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.HumanoidWalk"))
        {
            stride = Mathf.Floor(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        if(isWalk) UDPsend();

    }

    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(0, 0, 0);
        //Recenter.ResetCamera();
        yield return new WaitForSeconds(2f);
        //Recenter.FadeIn();
        
    }

    

    private static void ThreadMethod()
    {
        while (true)
        {

        }
    }
}
