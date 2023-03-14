using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(MakeObjLookCam());
    }

    IEnumerator MakeObjLookCam()
    {
        while (true)
        {
            var qua = Quaternion.LookRotation(Camera.main.transform.position - this.transform.position);
            transform.rotation = Quaternion.Euler(-qua.eulerAngles.x, 0f, 0f);
            yield return new WaitForEndOfFrame();
        }
    }
}
