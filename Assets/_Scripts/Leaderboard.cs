using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using MoreMountains.Feedbacks;

namespace BlackHole
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> _names;
        [SerializeField] private List<TextMeshProUGUI> _scores;

        private string _publicLeaderboardKey = "f3b5592574eedbd3136877354f8e17c664ac66f8920531537b0e58e12ef8d100";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GetLeaderboard()
        {
            LeaderboardCreator.GetLeaderboard(_publicLeaderboardKey, ((msg) =>
            {
                for (int i = 0; i< _names.Count; i++) {
                    _names[i].text = msg[i].Username;
                    _scores[i].text = msg[i].Score.ToString();
                }
            }));
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            LeaderboardCreator.UploadNewEntry(_publicLeaderboardKey, username, score, (_) =>
            {
                GetLeaderboard();
            });
        }
    }
}