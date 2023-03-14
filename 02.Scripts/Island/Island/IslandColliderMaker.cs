using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandColliderMaker : MonoBehaviour
{
    [SerializeField] MeshRenderer renderer;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] SpriteRenderer ping;

    public void MakeCollider()
    {
        var scale = renderer.bounds.size / transform.localScale.x;
        boxCollider.center = (renderer.bounds.center - transform.position) / transform.localScale.x;
        boxCollider.size = scale;
    }
}
