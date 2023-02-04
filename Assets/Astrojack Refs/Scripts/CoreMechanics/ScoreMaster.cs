using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMaster : MonoBehaviour {

	public static ScoreMaster instance;

	public int score, highScore;

	public Text scoreText, highScoreText, gameOverScoreText;

	private void Awake()
	{
		instance = this;

		if(PlayerPrefs.HasKey("HighScore"))
			{
				highScore = PlayerPrefs.GetInt ("HighScore");
				// highScoreText.text = highScore.ToString ();	
			}
	}

	public void AddScore()
	{
		score++;

		UpdateHighScore ();

		scoreText.text = score.ToString ();
		gameOverScoreText.text = highScore.ToString ();
	}

	public void UpdateHighScore()
	{
		if (score > highScore)
		{
			highScore = score;

			highScoreText.text = highScore.ToString ();

			PlayerPrefs.SetInt ("HighScore", highScore);
		}
	}

	public void ResetScore()
	{
		score = 0;
		// scoreText.text = score.ToString ();
		gameOverScoreText.text = highScore.ToString ();
	}
}
