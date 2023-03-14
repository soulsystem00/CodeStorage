using System.Collections;
using UnityEngine;

public class TmpEnemy : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(Des());
    }

    IEnumerator Des()
    {
        yield return new WaitForSeconds(1f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }
}
