using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ScoreBoard : MonoBehaviour
{
    [SerializeField] ScoreUI attacker;
    [SerializeField] ScoreUI[] minimi;
    [SerializeField] Sprite[] medals;
    PlayerInfo playerInfo;

    void Awake()
    {
        playerInfo = PlayerInfo.Instance;
    }

    void OnEnable()
    {
        if (playerInfo.GetPlayerCount() > 0)
        {
            attacker.SetScoreUI(playerInfo.playerNicknameList[playerInfo.attackerID], playerInfo.playerScoreList[playerInfo.attackerID]);

            Dictionary<ulong, int> playerlist = new Dictionary<ulong, int>();
            playerlist = playerInfo.playerScoreList;
            playerlist = playerlist.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int i = 0;
            foreach (var player in playerlist.Where(player => player.Key != playerInfo.attackerID))
            {
                minimi[i].gameObject.SetActive(true);
                minimi[i].SetScoreUI(playerInfo.playerNicknameList[player.Key], player.Value, (i < 3) ? medals[i] : null);
                i++;
            }

            for (; i < 4; i++)
            {
                minimi[i].gameObject.SetActive(false);
            }
        }
    }
}
