using System.Collections;
using DG.Tweening;
using RealTime;
using UnityEngine;
public class Mine : MonoBehaviour, IAttackFlow
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] GameObject particle;
    [SerializeField] BoxCollider boxCollider;

    void OnEnable()
    {
        SetStealth(false);
        mesh.material.color = Color.white;
        boxCollider.enabled = false;
        StartCoroutine(AttackFlow());
        StartCoroutine(DestroyAfterTime(20f));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.5f);
                StartCoroutine(EnableParticle());
            }

        }
    }

    public IEnumerator AttackFlow()
    {
        yield return new WaitForSeconds(1f);
        SetStealth(true);
        yield return mesh.material
        .DOFade(0f, 1f)
        .OnComplete(() =>
        {
            boxCollider.enabled = true;
            if (!RealTimeNetwork.IsMasterClient)
            {
                mesh.enabled = false;
            }

        });
    }

    public IEnumerator EnableParticle()
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        boxCollider.enabled = false;
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    IEnumerator DestroyAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);
        boxCollider.enabled = false;
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    public void SetStealth(bool _value)
    {
        StandardShaderUtils.ChangeRenderMode(mesh.material, _value ? StandardShaderUtils.BlendMode.Transparent : StandardShaderUtils.BlendMode.Opaque);
    }
}
