using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;

namespace BlackHole
{
    public class LeaderboardMenu : Menu<LeaderboardMenu>
    {

        [SerializeField] private List<TextMeshProUGUI> _names;
        [SerializeField] private List<TextMeshProUGUI> _scores;
        private readonly string _publicLeaderboardKey = "f3b5592574eedbd3136877354f8e17c664ac66f8920531537b0e58e12ef8d100";

        protected override void OnEnable()
        {
            base.OnEnable(); // Subscribe to EscapeActionPressed
            GetLeaderboard();
        }

        public void GetLeaderboard()
        {
            LeaderboardCreator.GetLeaderboard(_publicLeaderboardKey, (msg =>
            {
                int loopLength = (msg.Length < _names.Count) ? msg.Length : _names.Count;
                Debug.Log("ll " + loopLength);
                for (int i = 0; i < loopLength; i++)
                {
                    _names[i].text = msg[i].Username;
                    _scores[i].text = msg[i].Score.ToString();
                }
            }));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

    }
}