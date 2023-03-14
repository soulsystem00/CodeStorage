using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject obj;

    void Awake()
    {
        for (var i = -50; i < 50; i += 16)
        {
            for (var j = -50; j < 50; j += 16)
            {
                Instantiate(obj, new Vector3(i + 2f, 0, j + 2f), Quaternion.identity).transform.SetParent(this.transform);
            }
        }
    }
}
