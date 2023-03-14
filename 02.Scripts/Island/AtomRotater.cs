using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AtomRotater : MonoBehaviour
{
    float rotSpeed = 50f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, rotSpeed * Time.deltaTime, 0f));
    }
}
