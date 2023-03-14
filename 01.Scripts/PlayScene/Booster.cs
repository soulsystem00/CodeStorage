using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] GameObject boxObj;

    void OnEnable()
    {
        Init();
        StartCoroutine(Roll());
        StartCoroutine(DestroyWithSecond());
    }

    void Init()
    {
        particle.SetActive(false);
        boxObj.SetActive(true);
    }

    IEnumerator Roll()
    {
        while (true)
        {
            var rot = transform.rotation.eulerAngles;
            yield return transform.DORotate(new Vector3(rot.x, rot.y + 45f, rot.z), 0.2f).SetEase(Ease.Linear).WaitForCompletion();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
                player.SpeedUp(0.05f);
            StartCoroutine(DesObj());
        }
    }

    IEnumerator DesObj()
    {
        particle.SetActive(true);
        boxObj.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    IEnumerator DestroyWithSecond()
    {
        yield return new WaitForSeconds(3f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }
}
