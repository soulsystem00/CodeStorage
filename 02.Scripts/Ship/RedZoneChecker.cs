using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;

public class RedZoneChecker : RealTimeMonoBehaviour
{
    [SerializeField] Vector3 center;
    [SerializeField] ShipHealthController shipHealthController;

    void Start()
    {
        if (RedZone.i != null)
            StartCoroutine(RedZoneCheck());
    }

    IEnumerator RedZoneCheck()
    {
        while (true)
        {
            if (realtimeView.IsMine || (realtimeView.IsOutOwner && RealTimeNetwork.IsMasterClient))
            {
                if (transform.position.x > 500f)
                {
                    if ((center - transform.position).magnitude > RedZone.i.GetSize())
                    {
                        shipHealthController.ApplyDamage(5f);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}