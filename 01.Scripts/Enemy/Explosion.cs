using UnityEngine;

public class Explosion : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.5f);
            }
        }
    }
}
