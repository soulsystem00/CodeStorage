using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBooster : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform shipTransform;
    public void Booster()
    {
        Debug.Log(shipTransform.eulerAngles.y * Mathf.Deg2Rad);
        Vector3 dirVec = new Vector3(Mathf.Sin(shipTransform.eulerAngles.y * Mathf.Deg2Rad), 0f, Mathf.Cos(shipTransform.eulerAngles.y * Mathf.Deg2Rad));
        Debug.Log(dirVec);
        rb.AddForce(dirVec * 1000f, ForceMode.Force);
    }
}
