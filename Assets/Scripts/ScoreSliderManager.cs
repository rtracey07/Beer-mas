using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreSliderManager : MonoBehaviour {

	public Vector3 onPos;
	private Vector3 startPos;
	public Vector3 finalPos;
	private RectTransform m_Rect;

	public float scrollTime;
	public float scoreWait;
	public float scoreRate;

	public Text contestant;
	public Image[] scores;

	public Color correctColor;
	public Color wrongColor;

	public Text[] places;

	public ParticleSystem WinParticles;

	public AudioClip successSound;
	public AudioClip victorySound;

	public RectTransform m_NamesPos;
	public Vector3 m_NamesFinal;

	void Awake()
	{
		m_Rect = GetComponent<RectTransform> ();
		startPos = m_Rect.anchoredPosition;
	}

	// Use this for initialization
	public IEnumerator Tabulate() {

		float time = 0.0f;

		yield return new WaitForSeconds (3);
		foreach (Player curr in GameManager.Instance.m_Players) {
			foreach (Image s in scores) {
				s.color = Color.white;
			}
			contestant.text = curr.name;
			time = 0.0f;

			while (time <= scrollTime) {
				m_Rect.anchoredPosition = Vector3.Lerp (startPos, onPos, time / scrollTime);
				time += Time.deltaTime;
				yield return null;
			}

			m_Rect.anchoredPosition = onPos;

			yield return new WaitForSeconds (scoreWait);

			for (int i = 0; i < scores.Length; i++) {
				if (curr.correct [i]) {
					scores [i].color = correctColor;
					AudioSource.PlayClipAtPoint (successSound, Vector3.zero, 1.0f);
				} else
					scores [i].color = wrongColor;

				yield return new WaitForSeconds (scoreRate);
			}

			bool added = false;
			for (int i = 0; i < GameManager.Instance.m_Rankings.Count && !added; i++) {
				if (curr.score > GameManager.Instance.m_Rankings [i] [0].score) {
					GameManager.Instance.m_Rankings.Insert (i, new List<Player> ());
					GameManager.Instance.m_Rankings [i].Add (curr);
					added = true;
				} else if (curr.score == GameManager.Instance.m_Rankings [i] [0].score) {
					GameManager.Instance.m_Rankings [i].Add (curr);
					added = true;
				}
			}

			if (!added) {
				GameManager.Instance.m_Rankings.Add (new List<Player>());
				GameManager.Instance.m_Rankings[GameManager.Instance.m_Rankings.Count-1].Add (curr);
			}

			UpdateRankings ();

			time = 0.0f;

			while (time <= scrollTime) {
				m_Rect.anchoredPosition = Vector3.Lerp (onPos, finalPos, time / scrollTime);
				time += Time.deltaTime;
				yield return null;
			}

			m_Rect.anchoredPosition = finalPos;
		}

		yield return new WaitForSeconds (1);
		AudioSource.PlayClipAtPoint (victorySound, Vector3.zero);
		WinParticles.Play ();

		Vector3 currNamePos = m_NamesPos.anchoredPosition;
		time = 0.0f;

		while (time <= scrollTime) {
			m_NamesPos.anchoredPosition = Vector3.Lerp (currNamePos, m_NamesFinal, time / scrollTime);
			time += Time.deltaTime;
			yield return null;
		}

		m_NamesPos.anchoredPosition = m_NamesFinal;
	}

	void UpdateRankings()
	{
		string names;

		for (int i = 0; i < GameManager.Instance.m_Rankings.Count && i < 3; i++) {
			names = "";
			foreach (Player curr in GameManager.Instance.m_Rankings[i]) {
				names += curr.name + " ";
			}

			places [i].text = names; 
		}
	}
}
