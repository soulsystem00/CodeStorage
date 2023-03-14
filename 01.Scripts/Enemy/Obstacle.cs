using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "plate")
        {
            rb.isKinematic = true;
        }
    }
}
