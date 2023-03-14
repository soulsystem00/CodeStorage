using System.Collections;
using UnityEngine;

public class BombBot : MonoBehaviour, IAttackFlow
{
    [SerializeField] float speed;
    [SerializeField] SphereCollider col;
    private GameObject target = null;
    private Vector3 dirVec;

    void OnEnable()
    {
        col.enabled = false;
        StartCoroutine(DestoryObj());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (target == null)
            {
                target = other.gameObject;
                StartCoroutine(AttackFlow());
            }
        }
    }

    public IEnumerator AttackFlow()
    {
        while (target != null)
        {
            dirVec = (this.transform.position - target.transform.position);
            if (dirVec.magnitude >= 0.2f)
                transform.rotation = Quaternion.LookRotation(dirVec.normalized);
            var arrival = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            arrival.y = transform.position.y;
            transform.position = arrival;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DestoryObj()
    {
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        yield return new WaitForSeconds(9f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }
}
