using System;
using UnityEngine;

public class ExplosionBarrelObj : MonoBehaviour
{
    public Action OnCollisionEnterEvent;

    void OnEnable()
    {
        gameObject.tag = "Enemy";
        if (transform.position.y < 5)
            transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "plate")
        {
            gameObject.tag = "ETC";
            OnCollisionEnterEvent?.Invoke();
        }
        else if (other.collider.tag == "Player")
        {
            var player = other.collider.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.1f);
            }
        }
    }
}
