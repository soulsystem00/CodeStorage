using System.Collections;
using DG.Tweening;
using UnityEngine;
public class Bombard : MonoBehaviour, IAttackFlow
{
    [SerializeField] GameObject redZone;
    [SerializeField] GameObject particle;
    [SerializeField] SphereCollider col;
    [SerializeField] float duration;

    Coroutine curCoroutine;
    void OnEnable()
    {
        Init();
        StartCoroutine(AttackFlow());
        curCoroutine = StartCoroutine(ParticleRandom());
    }

    private void Init()
    {
        redZone.transform.localScale = Vector3.zero;
        particle.SetActive(false);
        col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
                player.SetPlayerHp(0.2f);
        }
    }

    public IEnumerator AttackFlow()
    {

        yield return redZone.transform.DOScale(5f, 1f).OnComplete(() => { particle.SetActive(true); col.enabled = true; });
        yield return new WaitForSeconds(duration);
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);
        particle.SetActive(false);
        redZone.transform.DOScale(0f, 0.5f).OnComplete(() => { ObjectPoolManager.Instance.Destroy(this.gameObject); });
    }

    IEnumerator ParticleRandom()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            particle.SetActive(true);
            particle.transform.localPosition = GetRandomPos();
            yield return new WaitForSeconds(0.3f);
            particle.SetActive(false);
        }
    }

    Vector3 GetRandomPos()
    {
        Vector3 pos = UnityEngine.Random.insideUnitSphere;
        pos.y = 0.03f;

        float r = UnityEngine.Random.Range(0.0f, 2.0f);

        return (pos * r);
    }
}
