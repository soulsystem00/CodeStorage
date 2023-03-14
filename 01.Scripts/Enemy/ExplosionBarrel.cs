using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ExplosionBarrel : MonoBehaviour, IAttackFlow
{
    [SerializeField] ExplosionBarrelObj barrel;
    [SerializeField] MeshRenderer mr;
    [SerializeField] ParticleSystem ps;
    [SerializeField] SpriteRenderer redZone;
    bool isActive = false;

    void OnEnable()
    {
        Init();
        StartCoroutine(AttackFlow());
    }

    void Init()
    {
        barrel.OnCollisionEnterEvent = () => { isActive = true; };
        mr.material.color = Color.white;
        isActive = false;
        barrel.gameObject.SetActive(true);
        ps.gameObject.SetActive(false);
        redZone.gameObject.SetActive(true);
        redZone.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    public IEnumerator AttackFlow()
    {
        yield return redZone.transform.DOScale(0.2f, 0.5f).OnComplete(() => redZone.gameObject.SetActive(false));
        while (true)
        {
            if (isActive)
            {
                yield return new WaitForSeconds(0.5f);
                for (var i = 0; i < 3f; i++)
                {
                    var colorOffset = (3f - i) * 0.3f;
                    barrel.transform.DOShakeRotation(0.5f, 10f);
                    mr.material.DOColor(new Color(1f, colorOffset, colorOffset), 0.5f);
                    yield return new WaitForSeconds(1f);
                }
                barrel.gameObject.SetActive(false);
                ps.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.7f);
                ObjectPoolManager.Instance.Destroy(this.gameObject);
            }
            yield return null;
        }
    }
}
