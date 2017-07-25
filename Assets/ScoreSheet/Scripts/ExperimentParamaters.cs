using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentParamaters : MonoBehaviour {
	public static int ExperimentNum;//実験番号　※0のとき最後の実験
	public static string ParticipantName;//被験者の名前
	public static List<int> conductedStimuliNum;//すでに実行された刺激番号
	public static List<List<List<string>>> Scores;
	public static int StimuliNum;//提示刺激数
	public static int Stimuli;//刺激番号

	void Start(){
		DontDestroyOnLoad (this);
		ExperimentNum = new int ();
		conductedStimuliNum = new List<int> ();

	}

}

