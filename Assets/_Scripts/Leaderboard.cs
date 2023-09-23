using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

namespace BlackHole
{
    public class Leaderboard : MonoBehaviour
    {
        private readonly string _publicLeaderboardKey = "f3b5592574eedbd3136877354f8e17c664ac66f8920531537b0e58e12ef8d100";

        private void OnEnable()
        {
            Scoring.OnScored += SubmitScore;
        }

        private void OnDisable()
        {
            Scoring.OnScored -= SubmitScore;
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            LeaderboardCreator.UploadNewEntry(_publicLeaderboardKey, username, score);
        }

        private void SubmitScore(int score)
        {
            SetLeaderboardEntry("testname", score);
        }
    }
}