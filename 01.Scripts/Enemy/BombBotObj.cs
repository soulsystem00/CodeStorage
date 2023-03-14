using System.Collections;
using UnityEngine;

public class BombBotObj : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] ParticleSystem ps;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.3f);
                StartCoroutine(DestoryObj());
            }

        }
    }

    IEnumerator DestoryObj()
    {
        this.gameObject.SetActive(false);
        ps.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ps.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        ObjectPoolManager.Instance.Destroy(parent);
    }
}
