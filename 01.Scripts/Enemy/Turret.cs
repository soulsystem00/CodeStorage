using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject turretHead;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzlePos;
    [SerializeField] Image timeUI;

    [SerializeField] float maxTime = 10f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] bool isAttacking = false;

    void OnEnable()
    {
        timeUI.fillAmount = 1f;
        StartCoroutine(FillImage());
    }

    void OnTriggerStay(Collider other)
    {
        if (!isAttacking)
        {
            if (other.tag == "Player")
            {
                isAttacking = true;
                StartCoroutine(AttackFlow(other.gameObject));
            }
        }
    }

    public IEnumerator AttackFlow(GameObject _target)
    {
        var dirVec = (_target.transform.position - this.gameObject.transform.position).normalized;
        var deg = Mathf.Atan2(dirVec.x, dirVec.z) * Mathf.Rad2Deg;
        // 1. rotate object & fire
        turretHead.transform.DORotate(new Vector3(0f, deg, 0f), 0.5f).SetEase(Ease.Linear).OnComplete(() => Fire(dirVec, deg));
        // 2. reload
        yield return new WaitForSeconds(reloadTime);
        isAttacking = false;
    }

    IEnumerator FillImage()
    {
        while (timeUI.fillAmount > 0f)
        {
            timeUI.fillAmount -= Time.deltaTime / maxTime;
            yield return null;
        }
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    private void Fire(Vector3 _dir, float _deg)
    {
        ObjectPoolManager.Instance.Instantiate(ResourceDataManager.bullet, muzzlePos.position, Quaternion.identity)
            .GetComponent<Bullet>()
            .InitBullet(_dir, Quaternion.Euler(0f, _deg, 0f));
    }
}
