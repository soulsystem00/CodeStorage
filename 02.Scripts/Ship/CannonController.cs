using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class CannonController : RealTimeMonoBehaviour
{
    [SerializeField] GameObject[] Cannons;
    public void SetActiveCannon(int _index)
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            Cannons[i].SetActive(i < _index);
        }
    }
}