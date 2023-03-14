using RealTime;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Awake()
    {
        PlayerSpawn();
    }

    void PlayerSpawn()
    {
        switch (PlayerInfo.Instance.minimiIndex)
        {
            case 0:
                SpawnPlayer("Player/PlayerObj/Player8");
                break;
            case 1:
                SpawnPlayer("Player/PlayerObj/Player9");
                break;
            case 2:
                SpawnPlayer("Player/PlayerObj/Player10");
                break;
            case 3:
                SpawnPlayer("Player/PlayerObj/Player11");
                break;
            case 4:
                SpawnPlayer("Player/PlayerObj/Player12");
                break;
            default:
                break;
        }
    }

    void SpawnPlayer(string _objectName)
    {
        RealTimeNetwork.Instantiate(_objectName, Vector3.zero, Quaternion.identity * Quaternion.Euler(0f, 180f, 0f));
    }
}