using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace InternetGame {
	public class ScoreSaver : MonoBehaviour {

		public static string SCORE_DIRECTORY = "Assets";
		public static string SCORE_FILE = "high-scores.json";

		public static int SCORES_SHOWN = 5;
		public static int SCORES_SAVED = 50;

		[Header("Test Attributes")]
		public string TestName = "TEST";
		public int TestScore = 42;

		[Serializable]
		public struct HighScore {
			public string name;
			public int score;

			public HighScore(string name, int score) {
				this.name = name;
				this.score = score;
			}

			public override string ToString () {
				return name + ": " + score.ToString ();
			}
		}

		public struct HighScores {
			public List<HighScore> scores;
			public HighScores(List<HighScore> scores) {
				this.scores = scores;
			}
		}

		public struct ScoresInfo {
			public int rank;
			public int firstScoreRank;
			public int scoreCount;
			public HighScore[] displayScores;
		}

		public static ScoresInfo SaveScore(string name, int score) {
			HighScore hScore = new HighScore (name, score);
			List<HighScore> scores = LoadScores ();
			scores.Add (hScore);
			scores.Sort ((s1, s2) => (-s1.score).CompareTo (-s2.score));

			if (scores.Count > SCORES_SAVED)
				scores.RemoveAt (scores.Count - 1);
			StoreScores (scores);

			int rank = scores.IndexOf (hScore);
			ScoresInfo ret = new ScoresInfo ();
			if (rank >= 0) {
				ret.rank = rank + 1;
				ret.firstScoreRank = Math.Max (1, ret.rank - SCORES_SHOWN / 2);
			} else {
				ret.rank = -1;
				ret.firstScoreRank = Math.Max (1, scores.Count - SCORES_SHOWN);
			}
			int lastRank = Math.Min (ret.firstScoreRank + SCORES_SHOWN, scores.Count + 1);
			ret.scoreCount = lastRank - ret.firstScoreRank;
			List<HighScore> displayScores = scores.GetRange (ret.firstScoreRank - 1, ret.scoreCount);
			ret.displayScores = displayScores.ToArray ();

			return ret;
		}

		public static void StoreScores(List<HighScore> scoreList) {
			string path = SCORE_DIRECTORY + "/" + SCORE_FILE;
			Debug.Log ("Storing scores in " + path);
			HighScores scores = new HighScores (scoreList);
			string json = JsonUtility.ToJson (scores, true);
			using (FileStream fs = new FileStream (
				                       path, FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (json);
				}
			}
		}

		public static List<HighScore> LoadScores() {
			string path = SCORE_DIRECTORY + "/" + SCORE_FILE;
			Debug.Log ("Loading scores from " + path);
			if (File.Exists (path)) {
				string json;
				using (FileStream fs = new FileStream (
					                       path, FileMode.Open)) {
					using (StreamReader reader = new StreamReader (fs)) {
						json = reader.ReadToEnd ();
					}
				}
				HighScores scores = JsonUtility.FromJson<HighScores> (json);
				return scores.scores;
			} else {
				Debug.Log ("File does not exist");
				return new List<HighScore> ();
			}
		}

		public void TestSaveScore() {
			ScoresInfo info = SaveScore (TestName, TestScore);
			Debug.Log ("Current rank: " + info.rank.ToString());
			Debug.Log ("First score shown: " + info.firstScoreRank.ToString ());
			Debug.Log ("Scores shown: " + info.scoreCount.ToString ());
		}
	}
}