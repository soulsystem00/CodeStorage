using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ItemBox : MonoBehaviour
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
            yield return boxObj.transform.DORotate(new Vector3(boxObj.transform.rotation.eulerAngles.x, boxObj.transform.rotation.eulerAngles.y + 45f, boxObj.transform.rotation.eulerAngles.z), 2f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>(); // 랜덤 스킬 활성화
            if (player != null)
            {
                if (player.realtimeView.IsMine)
                {
                    int randValue = UnityEngine.Random.Range(1, 101);
                    if (randValue <= 70)
                    {
                        int randomSkill = UnityEngine.Random.Range(0, player.GetSkillCount);
                        player.SetPlayerSkill((PlayerSkill)randomSkill);
                    }
                    else
                    {
                        Debug.Log(randValue + " Fail");
                    }
                }
                StartCoroutine(DesObj());
            }
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
        yield return new WaitForSeconds(5f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }
}

public enum PlayerSkill
{
    Dash,
    Stealth,
    Healing,
    Invincibility,
    Flash,
}