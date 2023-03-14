using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class SailController : RealTimeMonoBehaviour
{
    [SerializeField] GameObject[] SailBundle;

    public void SetActiveSail(int _index)
    {
        for (int i = 0; i < SailBundle.Length; i++)
        {
            SailBundle[i].SetActive(i == _index);
        }
    }
}
