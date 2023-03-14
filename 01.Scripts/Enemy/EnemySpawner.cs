using System.Collections;
using DG.Tweening;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;
public class EnemySpawner : RealTimeGameSingleton<EnemySpawner>
{
    Coroutine CoFollowThunderSync;
    GameObject syncGameObj;

    public void SpawnEnemy(Enemy _enemy, Vector3 _pos, Quaternion _qua, bool _isUI = false)
    {
        realtimeView.RPC("RPCSpawnEnemy", RpcTarget.All, (int)_enemy, _pos.x, _pos.y, _pos.z, _qua.x, _qua.y, _qua.z, _qua.w, _isUI);
    }

    IEnumerator CoPosSync(GameObject _go)
    {
        while (true && RealTimeNetwork.IsInRoom)
        {
            realtimeView.RPC("RPCPosSync", RpcTarget.Others, syncGameObj.transform.position.x, syncGameObj.transform.position.y, syncGameObj.transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }
    }

    [RealTimeRPC]
    public void RPCSpawnEnemy(int _enemyType, float _posX, float _posY, float _posZ, float _quaX, float _quaY, float _quaZ, float _quaW, bool _isUI)
    {
        var pos = new Vector3(_posX, _posY, _posZ);
        var qua = new Quaternion(_quaX, _quaY, _quaZ, _quaW);
        var gameObject = ResourceDataManager.GetGameObject((Enemy)_enemyType);
        if (gameObject != null)
        {
            if (_isUI)
            {
                var obj = ObjectPoolManager.Instance.Instantiate(gameObject, pos, qua);
                obj.GetComponent<Sniper>().Init(pos);
            }
            else if ((Enemy)_enemyType == Enemy.Meteor)
            {
                var obj = ObjectPoolManager.Instance.Instantiate(gameObject, new Vector3(20f, 25f, 0f), qua);
                obj.GetComponent<Meteor>().dirVec = pos - new Vector3(20f, 25f, 0f);
            }
            else
            {
                var obj = ObjectPoolManager.Instance.Instantiate(gameObject, pos, qua);
                if (obj.GetComponent<FollowingThuner>() != null)
                {
                    syncGameObj = obj;
                    if (RealTimeNetwork.IsMasterClient)
                    {
                        if (CoFollowThunderSync != null)
                        {
                            StopCoroutine(CoFollowThunderSync);
                        }
                        CoFollowThunderSync = StartCoroutine(CoPosSync(obj));
                    }
                }
            }
        }
    }

    [RealTimeRPC]
    public void RPCPosSync(float _x, float _y, float _z)
    {
        if (syncGameObj != null)
            syncGameObj.transform.DOLocalMove(new Vector3(_x, _y, _z), 0.1f);
    }
}
