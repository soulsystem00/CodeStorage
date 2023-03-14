using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Thunder : MonoBehaviour, IAttackFlow
{
    [SerializeField] SpriteRenderer redZone;
    [SerializeField] GameObject thunder;
    [SerializeField] CapsuleCollider capsuleCollider;
    private float maxScale = 0.2f;

    void OnEnable()
    {
        Init();
        StartCoroutine(AttackFlow());
    }

    private void Init()
    {
        capsuleCollider.enabled = false;
        thunder.SetActive(false);
        redZone.gameObject.SetActive(true);
        redZone.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    public IEnumerator AttackFlow()
    {
        yield return redZone.transform.DOScale(maxScale, 0.5f);
        yield return new WaitForSeconds(0.5f);
        redZone.gameObject.SetActive(false);
        thunder.SetActive(true);
        capsuleCollider.enabled = true;
        yield return new WaitForSeconds(0.5f);
        thunder.SetActive(false);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.2f);
            }
        }
    }
}
