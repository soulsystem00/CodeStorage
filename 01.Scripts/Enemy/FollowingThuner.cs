using System.Collections;
using DG.Tweening;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class FollowingThuner : RealTimeMonoBehaviour, IAttackFlow
{
    [SerializeField] SpriteRenderer redZone;
    [SerializeField] GameObject thunder;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] float speed;
    private float maxScale = 0.2f;
    private float curTime = 0f;
    private Vector3 dirVec;

    void OnEnable()
    {
        Init();
        StartCoroutine(AttackFlow());
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

    private void Init()
    {
        curTime = 0f;
        capsuleCollider.enabled = false;
        thunder.SetActive(false);
        redZone.gameObject.SetActive(true);
        redZone.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    public IEnumerator AttackFlow()
    {

        yield return redZone.transform.DOScale(maxScale, 0.5f);
        yield return new WaitForSeconds(0.5f);
        thunder.SetActive(true);
        capsuleCollider.enabled = true;
        yield return new WaitForEndOfFrame();
        if (RealTimeNetwork.IsMasterClient)
        {
            while (curTime <= 5f)
            {
                curTime += Time.deltaTime;
                if (RealTimeNetwork.IsMasterClient)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Moved)
                            {
                                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                                RaycastHit hit;
                                if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
                                {
                                    SetDirVec(hit.point);
                                }
                            }
                        }
                    }
                    else if (Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
                            {
                                SetDirVec(hit.point);
                            }
                        }
                    }
                }
                if (dirVec != null)
                {
                    transform.Translate(dirVec * speed * Time.deltaTime);
                }
                yield return null;
            }
        }
        else
        {
            while (curTime <= 5f)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
        }
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    void SetDirVec(Vector3 _targetPos)
    {
        dirVec = (_targetPos - transform.position).normalized;
        dirVec = new Vector3(dirVec.x, 0f, dirVec.z);
    }
}
