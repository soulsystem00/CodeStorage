using DG.Tweening;
using UnityEngine;
public class RollingBarrel : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject redZone;

    public Vector3 dirVec;
    bool isActive = false;

    void OnEnable()
    {
        redZone.SetActive(true);
        redZone.transform.localScale = new Vector3(0f, 1f, 1f);
        redZone.transform.DOScaleX(4f, 1f);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "plate" && isActive == false)
        {
            redZone.SetActive(false);
            dirVec = rb.gameObject.transform.rotation * Vector3.right;
            isActive = true;
        }
        else if (collisionInfo.collider.tag == "Player")
        {
            var player = collisionInfo.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.2f);
            }
        }
    }

    void FixedUpdate()
    {
        if (isActive && rb.velocity.magnitude < 5)
        {
            rb.AddForce(dirVec.normalized * speed, ForceMode.Impulse);
        }
        if (isActive)
            rb.transform.Rotate(new Vector3(0, 0, -5f));
    }

    void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.gameObject.transform.rotation = Quaternion.identity;
        rb.gameObject.transform.position = Vector3.zero;
        isActive = false;
    }
}