using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HMD_TYPE
{
    Vive,
    Oculus,
    None
}


public class ExperimentParamaters : MonoBehaviour {
	public static int ExperimentNum;//実験番号　※0のとき最後の実験
	public static string ParticipantName;//被験者の名前
	public static List<int> ConductedStimuliNum = new List<int>();//すでに実行された刺激番号
	public static List<List<List<string>>> Scores;
	public static int StimuliNum;//提示刺激数
	public static int Stimuli;//刺激番号
    public static bool IsScore;
    public static HMD_TYPE HMD_Type;

    void Start(){
		DontDestroyOnLoad (this);
	}

}

