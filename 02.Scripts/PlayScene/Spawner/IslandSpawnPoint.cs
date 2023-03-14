using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSpawnPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 10f);
    }

    public Vector3 GetRandomPoint()
    {
        float x = Random.Range(-5f, 5f);
        float z = Random.Range(-5f, 5f);
        Vector3 point = new Vector3(transform.position.x + x, 0f, transform.position.z + z);

        return point;
    }
}