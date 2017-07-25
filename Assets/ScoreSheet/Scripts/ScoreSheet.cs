using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class ScoreSheet : MonoBehaviour {
	[SerializeField]
	private int questionNum;//質問数
	[SerializeField]
	public int stimuliNum = 5;//提示刺激数
	public int stimuli;//現在の刺激番号
	private string[] _text;
	private string[] _minimumText;
	private string[] _maximumText;
	private string templatePath;//テンプレートのパス
	private string[] resultPath;//結果CSVのパス
	private FileInfo templateFi;
	private FileInfo[] resultFi;
	//public TextAsset scoreSheetTemplate;
	public Canvas canvas;
	private Text[] question;
	private Text[] text;
	private Text[] minimumText;
	private Text[] maximumText;
    [SerializeField]
	private Slider[] slider;
	// Use this for initializatio
	void Start () {
		templatePath = Application.dataPath + "/CSV/ScoreSheetTemplate.csv";//テンプレートのパス
		templateFi = new FileInfo (templatePath);//テンプレートのFileInfo
		questionNum = csvLineNumber(templateFi) - 1;
        Debug.Log("questionNum:" + questionNum);
		resultPath = new string[questionNum];
		resultFi = new FileInfo[questionNum];
		List<string> listTemp1 = new List<string>();;
		List<List<string>> listTemp2 = new List<List<string>>();;
		List<List<List<string>>> listTemp3 = new List<List<List<string>>>();
		for (int i = 0; i < questionNum; i++) {
			resultPath[i] =  Application.dataPath + "/CSV/resultQuestion" + (i+1) + ".csv";
			resultFi [i] = new FileInfo (resultPath [i]);
			if (!resultFi [i].Exists) {//同名のファイルがなければ何もしない
                Debug.Log("!resultFi[" + i + "]");
				StreamWriter sw = resultFi [i].CreateText ();
				sw.WriteLine ("\"Question" + (i + 1) + "\"");
                listTemp1.Add("\"Question" + (i + 1) + "\"");
                listTemp2.Add(listTemp1);
                listTemp1 = new List<string>();
                for (int j = 0; j < stimuliNum; j++){
					sw.WriteLine("\"stimulation" + (j+1) + "\"");
                    listTemp1.Add("\"stimulation" + (j + 1) + "\"");
                    listTemp2.Add(listTemp1);
                    listTemp1 = new List<string>();
                }
                listTemp3.Add(listTemp2);
                listTemp2 = new List<List<string>>();
                sw.Flush ();
				sw.Close ();
			} else {//同名のファイルが見つかれば読み取り、Listに格納
                Debug.Log("resultFi[" + i + "]");
				string[,] temp =  csvRead(resultFi[i]);
				for (int j = 0; j < temp.GetLength (0); j++) {
					for (int k = 0; k < temp.GetLength (1); k++) {
                        if(temp[j, k] == null)
                        {
                            listTemp1.Add("\"\"");
                        }
                        else
                        {
                            listTemp1.Add(temp[j, k]);//listTemp1:行リスト
                        }
						
                    }
					listTemp2.Add (listTemp1);//listTemp2:シートの列リスト
                    listTemp1 = new List<string>();
				}
				listTemp3.Add (listTemp2);//listTemp3:質問ごとのシート
                listTemp2 = new List<List<string>>();
			}
		}
		ExperimentParamaters.Scores = listTemp3;
		listTemp3 = new List<List<List<string>>>();
		stimuli = ExperimentParamaters.Stimuli;


		//ExperimentNumが0のときExitExperimentボタンを表示、0以外のときNextExperimentボタンを表示
		if (ExperimentParamaters.ExperimentNum == 0) {
			GameObject.Find ("NextExperiment").SetActive (false);
			GameObject.Find ("ExitExperiment").SetActive (true);
		} else {
			GameObject.Find ("NextExperiment").SetActive (true);
			GameObject.Find ("ExitExperiment").SetActive (false);
		}
			
		_text = new string[questionNum];
		_minimumText = new string[questionNum];
		_maximumText = new string[questionNum];

		for(int i = 0; i < questionNum; i++){
			_text [i] = csvRead(templateFi) [i+1,1];
			_minimumText [i] = csvRead(templateFi) [i+1,2];
			_maximumText [i] = csvRead(templateFi) [i+1,3];		
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
			Instantiate (question [i], canvas.transform);
			Instantiate (text [i], canvas.transform);
			Instantiate (minimumText [i], canvas.transform);
			Instantiate (maximumText [i], canvas.transform);
            slider[i] = Instantiate(Resources.Load("prefabs/Slider", typeof(Slider)) as Slider, canvas.transform, false);//sliderはinstantiateで生成された個別のvalueが欲しいから生成したオブジェクトを配列に代入している
			question [i].rectTransform.sizeDelta = new Vector2 (200, 35);
			question [i].rectTransform.position = new Vector3 (partsPosX - 300f, partsPosY - question [i].rectTransform.sizeDelta.y/2, 0);
			question [i].text = "Question" + (i+1);
			partsPosY =partsPosY - question [i].rectTransform.sizeDelta.y;
			text [i].rectTransform.sizeDelta = new Vector2 (700, 20 * (_text[i].Length/35 + 1));
			text [i].rectTransform.position = new Vector3 (partsPosX, partsPosY - 10f - text [i].rectTransform.sizeDelta.y/2, 0);
			text [i].text = _text [i];
			partsPosY = partsPosY - 10f - text [i].rectTransform.sizeDelta.y;
            //↓プレハブをinstantiateしたオブジェクにpositionを指定すると親の中心座標を原点として計算されるっぽい（rectTransformでも）
            slider [i].GetComponent<RectTransform>().localPosition = new Vector3 (partsPosX, partsPosY - 10f - slider[i].GetComponent<RectTransform>().sizeDelta.y/2 +　canvas.GetComponent<RectTransform>().sizeDelta.y/2, 0);
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
        //Delete test
        if (Input.GetKeyDown(KeyCode.D))
        {
            for(int i = 0; i<ExperimentParamaters.Scores.Count; i++)
            {
                if (resultFi[i].Exists)
                {
                    Debug.Log("Delete resultFi[" +  i + "]");
                    resultFi[i].Delete();
                }
            }
        }
		//test
		if (Input.GetKeyDown (KeyCode.Space)) {
            Debug.Log("Writing the data...");
			for (int i = 0; i < ExperimentParamaters.Scores.Count; i++) {
				if(ExperimentParamaters.ExperimentNum == 1)
                {
                    ExperimentParamaters.Scores[i][0].Add("\"" + ExperimentParamaters.ParticipantName + "\"");
                }
				ExperimentParamaters.Scores [i] [stimuli + 1].Add ("\"" + slider[i].value.ToString() + "\"");

				if (resultFi [i].Exists)
                {
                    Debug.Log("Delete resultFi[" + i + "]");
                    resultFi[i].Delete();
                }
				resultFi [i] = new FileInfo (resultPath [i]);
				StreamWriter sw = resultFi [i].CreateText ();
				string writeText = "";
				for (int j = 0; j < ExperimentParamaters.Scores [i].Count; j++) {
					for (int k = 0; k < ExperimentParamaters.Scores [i] [j].Count; k++) {
                        if(k < ExperimentParamaters.Scores[i][j].Count - 1)
                        {
                            writeText += ExperimentParamaters.Scores[i][j][k] + ",";

                        }
						else
                        {
                            writeText += ExperimentParamaters.Scores[i][j][k];
                        }
                    }
					sw.Write (writeText + "\n");
                    writeText = "";
                }
				sw.Flush ();
				sw.Close ();
			}
		}
	}

	public void OnNextExperiment () {
		//stimuli = ExperimentParamaters.Stimuli;


	}

	public void OnExitExperiment () {
		
	}


	private string[,] csvRead(FileInfo fi){
		if (!fi.Exists) {
			throw new FileNotFoundException ();
		} else {
			StreamReader sr = fi.OpenText ();
			string text = sr.ReadToEnd ();
			sr.Close ();
			string[] lineText = text.Split ('\n');
            int columNum = lineText[0].Split(',').Length;//列数の最大数
            int lineNum = lineText.Length - 1;//行数
            string[,] csv = new string[lineNum,columNum];
			for (int i = 0; i < lineNum; i++) {
                columNum = lineText[i].Split(',').Length;
                for (int j = 0; j < columNum; j++) {
					string[] words = lineText [i].Split (',');
                    csv[i, j] = words[j];
                }
			}
			return csv;
		}
	}

	private int csvLineNumber(FileInfo fi){
		if (!fi.Exists) {
			throw new FileNotFoundException ();
		} else {
			StreamReader sr = fi.OpenText ();
			string text = sr.ReadToEnd ();
			string[] lineText = text.Split ('\n');
			sr.Close ();
			return lineText.Length -1;
		}
	
	}
	private int csvColumnNumber(FileInfo fi){
		if (!fi.Exists) {
			throw new FileNotFoundException ();
		} else {
			StreamReader sr = fi.OpenText ();
			string text = sr.ReadToEnd ();
			string[] lineText = text.Split ('\n');
			lineText = text.Split ('\n');
			sr.Close ();
			return lineText [0].Split (',').Length - 1;
		}
	}


}
