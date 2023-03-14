using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour, IAttackFlow
{
    public ParticleSystem MeteorExplosionParticleSystem;
    public ParticleSystem MeteorShrapnelParticleSystem;
    public float speed;
    public Vector3 dirVec;

    void OnEnable()
    {
        StartCoroutine(AttackFlow());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "plate")
        {
            MeteorExplosionParticleSystem.transform.position = transform.position;
            MeteorExplosionParticleSystem.transform.rotation = Quaternion.LookRotation(transform.position);
            MeteorExplosionParticleSystem.Emit(10);
            MeteorShrapnelParticleSystem.transform.position = transform.position;
            MeteorShrapnelParticleSystem.Emit(10);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetPlayerHp(0.8f);
            }

        }
    }

    public IEnumerator AttackFlow()
    {
        while (true)
        {
            transform.Translate(dirVec * speed * Time.deltaTime);
            yield return null;
        }
    }
}
