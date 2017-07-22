using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentParamaters : MonoBehaviour {
	public int ExperimentNum;//実験番号　※0のとき最後の実験
	public string ParticipantName;//被験者の名前
	public List<int> conductedStimuliNum;//すでに実行された刺激番号
	public List<List<List<string>>> Scores;
	public int StimuliNum;//提示刺激数
	public int Stimuli;//刺激番号

	void Start(){
		DontDestroyOnLoad (this);
		ExperimentNum = new int ();
		conductedStimuliNum = new List<int> ();

	}

}

