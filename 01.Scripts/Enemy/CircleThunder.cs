using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class CircleThunder : MonoBehaviour, IAttackFlow
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] GameObject col;
    float particleTime = 3f;

    List<GameObject> hitObj = new List<GameObject>();

    void OnEnable()
    {
        Init();
        StartCoroutine(AttackFlow());
    }

    void Init()
    {
        hitObj.Clear();
        var asdf = ps.shape;
        asdf.radius = 1f;
        col.transform.localScale = Vector3.zero;
    }

    public IEnumerator AttackFlow()
    {
        col.SetActive(false);
        yield return new WaitForSeconds(1f);
        col.SetActive(true);
        var asdf = ps.shape;
        DOTween.To(() => asdf.radius, x => asdf.radius = x, 25f, particleTime)/*.SetEase(Ease.InCirc)*/;
        col.transform.DOScale(19f, particleTime);
        yield return new WaitForSeconds(particleTime);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Player")
        {
            var player = collisionInfo.collider.GetComponent<Player>();
            if (player != null)
            {
                if (!hitObj.Contains(player.gameObject))
                {
                    player.SetPlayerHp(0.2f);
                    hitObj.Add(player.gameObject);
                }
            }
        }
    }
}