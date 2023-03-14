using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletMesh;
    public float speed = 4f;
    public Vector3 dirVec;

    public void InitBullet(Vector3 _dir, Quaternion _qua)
    {
        dirVec = _dir;
        bulletMesh.transform.rotation = _qua;
    }

    void FixedUpdate()
    {
        transform.Translate(dirVec.normalized * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.2f);
                ObjectPoolManager.Instance.Destroy(this.gameObject);
            }
        }
    }
}
