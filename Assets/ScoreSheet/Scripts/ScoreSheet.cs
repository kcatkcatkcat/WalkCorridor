using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class ScoreSheet : MonoBehaviour {

	public string participantName;
	[SerializeField]
	private int questionNum;//質問数
	[SerializeField]
	private string[] _text;
	[SerializeField]
	private string[] _minimumText;
	[SerializeField]
	private string[] _maximumText;
	private string templatePath;//テンプレートのパス
	private string[] resultPath;//結果CSVのパス
	//public TextAsset scoreSheetTemplate;
	public string scoreSheetTemplate;
	public Canvas canvas;
	private Text[] question;
	private Text[] text;
	private Text[] minimumText;
	private Text[] maximumText;
	private Slider[] slider;
	private List<List<float>> scores = new List<List<float>>(); 
	// Use this for initializatio
	void Start () {
		//ExperimentNumが0のときExitExperimentボタンを表示、0以外のときNextExperimentボタンを表示
		if (ExperimetParamaters.ExperimentNum == 0) {
			GameObject.Find ("NextExperiment").SetActive (false);
			GameObject.Find ("ExitExperiment").SetActive (true);
		} else {
			GameObject.Find ("NextExperiment").SetActive (true);
			GameObject.Find ("ExitExperiment").SetActive (false);
		}

		templatePath = Application.dataPath + "/CSV/ScoreSheetTemplate.csv";//テンプレートのパス
		StreamReader reader = new StreamReader (
			templatePath,
			Encoding.GetEncoding ("utf-8")
		);
		scoreSheetTemplate = reader.ReadToEnd ();
		string[] lineText = scoreSheetTemplate.Split ('\n');
		lineText = scoreSheetTemplate.Split ('\n');
		questionNum = lineText.Length - 1;
		int columNum = lineText[0].Split(',').Length;
		int lineNum = questionNum + 1;
		string[,] csv = new string[lineNum,columNum];
		for (int i = 0; i < lineNum; i++) {
			for (int j = 0; j < columNum; j++) {
				string[] words = lineText [i].Split (',');
				csv [i,j] = words[j];
			}
		}
		_text = new string[questionNum];
		_minimumText = new string[questionNum];
		_maximumText = new string[questionNum];
		for(int i = 0; i < questionNum; i++){
			_text [i] = csv [i+1,1];
			_minimumText [i] = csv [i+1,2];
			_maximumText [i] = csv [i+1,3];		
		}

		question = new Text[questionNum];
		text = new Text[questionNum];
		minimumText = new Text[questionNum];
		maximumText = new Text[questionNum];
		slider = new Slider[questionNum];

		canvas = GameObject.Find ("ScoreSheet").GetComponent<Canvas>();
		float partsPosX = 0;
		float partsPosY = 0;
		for (int i = 0; i < questionNum; i++) {
			question [i] = Resources.Load ("prefabs/Question", typeof(Text)) as Text;
			text [i] = Resources.Load ("prefabs/Text", typeof(Text))as Text;
			minimumText [i] = Resources.Load ("prefabs/minimumText", typeof(Text))as Text;
			maximumText [i] = Resources.Load ("prefabs/maximumText", typeof(Text))as Text;
			slider[i] = Resources.Load ("prefabs/Slider", typeof(Slider))as Slider;
			Instantiate (question [i], canvas.transform);
			Instantiate (text [i], canvas.transform);
			Instantiate (minimumText [i], canvas.transform);
			Instantiate (maximumText [i], canvas.transform);
			Instantiate (slider [i], canvas.transform);
			question [i].rectTransform.sizeDelta = new Vector2 (200, 35);
			question [i].rectTransform.position = new Vector3 (partsPosX - 300f, partsPosY - question [i].rectTransform.sizeDelta.y/2, 0);
			question [i].text = "Question" + (i+1);
			partsPosY =partsPosY - question [i].rectTransform.sizeDelta.y;
			text [i].rectTransform.sizeDelta = new Vector2 (700, 20 * (_text[i].Length/35 + 1));
			text [i].rectTransform.position = new Vector3 (partsPosX, partsPosY - 10f - text [i].rectTransform.sizeDelta.y/2, 0);
			text [i].text = _text [i];
			partsPosY = partsPosY - 10f - text [i].rectTransform.sizeDelta.y;
			slider [i].GetComponent<RectTransform>().position = new Vector3 (partsPosX, partsPosY - 10f - slider[i].GetComponent<RectTransform>().sizeDelta.y/2, 0);
			partsPosY = partsPosY - 10f - slider [i].GetComponent<RectTransform> ().sizeDelta.y;
			minimumText [i].rectTransform.sizeDelta = new Vector2 (200, 20 * (_minimumText [i].Length / 10 + 1));
			minimumText [i].rectTransform.position = new Vector3 (partsPosX - 300f, partsPosY - 10f - minimumText [i].rectTransform.sizeDelta.y/2, 0);
			minimumText [i].text = _minimumText [i];
			maximumText [i].rectTransform.sizeDelta = new Vector2 (200, 20 * (_maximumText [i].Length / 10 + 1));
			maximumText [i].rectTransform.position = new Vector3 (partsPosX + 300f, partsPosY - 10f - maximumText [i].rectTransform.sizeDelta.y/2, 0);
			maximumText [i].text = _maximumText [i];
			partsPosY = partsPosY - 10f - maximumText [i].rectTransform.sizeDelta.y;
			if (partsPosY < -600) {
				partsPosX += 850;
				partsPosY = 0;
				question [i].rectTransform.sizeDelta = new Vector2 (200, 35);
				question [i].rectTransform.position = new Vector3 (partsPosX - 300f, partsPosY - question [i].rectTransform.sizeDelta.y/2, 0);
				question [i].text = "Question" + (i+1);
				partsPosY =partsPosY - question [i].rectTransform.sizeDelta.y;
				text [i].rectTransform.sizeDelta = new Vector2 (700, 20 * (_text[i].Length/35 + 1));
				text [i].rectTransform.position = new Vector3 (partsPosX, partsPosY - 10f - text [i].rectTransform.sizeDelta.y/2, 0);
				text [i].text = _text [i];
				partsPosY = partsPosY - 10f - text [i].rectTransform.sizeDelta.y;
				slider [i].GetComponent<RectTransform>().position = new Vector3 (partsPosX, partsPosY - 10f - slider[i].GetComponent<RectTransform>().sizeDelta.y/2, 0);
				partsPosY = partsPosY - 10f - slider [i].GetComponent<RectTransform> ().sizeDelta.y;
				minimumText [i].rectTransform.sizeDelta = new Vector2 (200, 20 * (_minimumText [i].Length / 10 + 1));
				minimumText [i].rectTransform.position = new Vector3 (partsPosX - 300f, partsPosY - 10f - minimumText [i].rectTransform.sizeDelta.y/2, 0);
				minimumText [i].text = _minimumText [i];
				maximumText [i].rectTransform.sizeDelta = new Vector2 (200, 20 * (_maximumText [i].Length / 10 + 1));
				maximumText [i].rectTransform.position = new Vector3 (partsPosX + 300f, partsPosY - 10f - maximumText [i].rectTransform.sizeDelta.y/2, 0);
				maximumText [i].text = _maximumText [i];
				partsPosY = partsPosY - 10f - maximumText [i].rectTransform.sizeDelta.y - 30f;
			}else partsPosY -= 30f;
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnNextExperiment () {
		List<float> score = new List<float> ();
		for (int i = 0; i < questionNum; i++) {
			score.Add (slider[i].value);
		}
		ExperimetParamaters.Scores.Add (score);
	}

	public void OnExitExperiment () {
		FileInfo[] fi = new FileInfo[questionNum]; 
		for (int i = 0; i < questionNum; i++) {
			resultPath[i] =  Application.dataPath + "/CSV/resultQuestion" + (i+1) + ".csv";
			fi [i] = new FileInfo (resultPath [i]);
			StreamWriter sw = fi [i].AppendText ();
			sw.WriteLine ("xxx");
			sw.Flush ();
			sw.Close ();
		}
	}
	private void ReadTemplate(){

	}
}
