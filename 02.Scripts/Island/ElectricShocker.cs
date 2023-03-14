using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using Google.Protobuf;
using RealTime;
using UnityEngine;

public class ElectricShocker : MonoBehaviour
{
    [SerializeField] Transform startPos;
    Dictionary<GameObject, GameObject> electricDic = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, Coroutine> coroutineDic = new Dictionary<GameObject, Coroutine>();
    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);
    RealTimeEventManager realTimeEventManager;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        realTimeEventManager.OnGCBattleStart += DestroyAllElectric;
    }

    void OnDestroy()
    {
        realTimeEventManager.OnGCBattleStart -= DestroyAllElectric;
    }

    void OnTriggerEnter(Collider other)
    {
        if (transform.parent.CompareTag("Neutrality"))
        {
            if (other.CompareTag("Red") || other.CompareTag("Blue"))
            {
                if (!other.isTrigger)
                    SpawnElectric(other.gameObject);
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Red") || other.CompareTag("Blue"))
        {
            DestroyElectric(other.gameObject);
        }
    }

    private void SpawnElectric(GameObject target)
    {
        if (!electricDic.ContainsKey(target))
        {
            LightningBoltScript electric = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.electric, transform.position, Quaternion.identity).GetComponent<LightningBoltScript>();
            electric.StartObject = startPos.gameObject;
            electric.EndObject = target;

            electricDic.Add(target, electric.gameObject);

            ShipHealthController ship = target.GetComponent<ShipHealthController>();
            if (!coroutineDic.ContainsKey(target))
            {
                coroutineDic.Add(target, StartCoroutine(ApplyShock(ship)));
            }
        }
    }

    private void DestroyElectric(GameObject target)
    {
        if (electricDic.ContainsKey(target))
        {
            GameObject electric = electricDic[target];
            ObjectPoolManager.Instance.Destroy(electric);

            electricDic.Remove(target);
        }

        if (coroutineDic.ContainsKey(target))
        {
            StopCoroutine(coroutineDic[target]);
            coroutineDic.Remove(target);
        }
    }

    private void DestroyAllElectric(IMessage packet)
    {
        foreach (var e in electricDic)
        {
            ObjectPoolManager.Instance.Destroy(e.Value);
            if (coroutineDic.ContainsKey(e.Key))
            {
                StopCoroutine(coroutineDic[e.Key]);
                coroutineDic.Remove(e.Key);
            }
        }
    }

    private IEnumerator ApplyShock(ShipHealthController ship)
    {
        while (true)
        {
            if (transform.parent.CompareTag("Neutrality"))
            {
                if (ship.realtimeView.IsMine || (ship.realtimeView.IsOutOwner && RealTime.RealTimeNetwork.IsMasterClient))
                {
                    ship.ApplyDamage(5f);
                }
            }
            else
            {
                DestroyElectric(ship.gameObject);
            }

            yield return waitForSeconds;
        }
    }
}
