using System.Collections;
using RealTime;
using UnityEngine;
public class ObjectLimit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
                PlayerInfo.Instance.DestroyPlayer2(player.gameObject, true);
        }
        else if (other.tag == "Meteor")
        {
            StartCoroutine(DestroyObj(2f, other.gameObject));
        }
        else if (other.tag == "RollingBarrel")
        {
            ObjectPoolManager.Instance.Destroy(other.transform.parent.gameObject);
        }
        else
        {
            ObjectPoolManager.Instance.Destroy(other.gameObject);
        }
    }

    IEnumerator DestroyObj(float _time, GameObject _go)
    {
        yield return new WaitForSeconds(_time);
        ObjectPoolManager.Instance.Destroy(_go);
    }

    IEnumerator Destroy(Player player)
    {
        player.sendScore(true);
        yield return new WaitForEndOfFrame();
        RealTimeNetwork.Destroy(player.gameObject);
    }
}