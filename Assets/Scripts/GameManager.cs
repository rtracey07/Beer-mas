using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private static GameManager _Instance;
	public static GameManager Instance { get { return _Instance; } }

	public TextAsset file;
	public ScoreSliderManager m_ScoreSlider;

	public int playerCount;

	[HideInInspector]
	public List<Player> m_Players;
	public string[] answers;

	public List<List<Player>> m_Rankings;


	void Start() {

		if (_Instance == null)
			_Instance = this;

		if (_Instance != this) 
			Destroy (gameObject);

		m_Players = new List<Player> ();
		m_Rankings = new List<List<Player>> ();
		ReadFile ();
		CheckAnswers ();
		StartCoroutine(m_ScoreSlider.Tabulate());
	}
	

	void ReadFile()
	{
		string fileData = file.text;
		string[] lines = fileData.Split ("\n" [0]);

		for (int i = 1; i < lines.Length; i++) {
			string[] lineData = (lines [i].Trim ()).Split ("," [0]);

			if (lineData.Length > 2) {
				Player currPlayer = new Player ();

				currPlayer.name = lineData [1].Substring(1, lineData[1].Length-2);
				Debug.Log (lineData[1]);
				currPlayer.answers = new string[8];
				currPlayer.correct = new bool[8];

				for (int j = 0; j < 8; j++)
					currPlayer.answers [j] = lineData [j + 2];

				m_Players.Add (currPlayer);
				playerCount++;
			}
		}
	}

	void CheckAnswers()
	{
		foreach (Player curr in m_Players) {

			for (int i = 0; i < 8; i++) {
				if (curr.answers [i] == answers [i]) {
					curr.correct [i] = true;
					curr.score++;
				} else {
					curr.correct [i] = false;
				}
			}
		}
	}

}
