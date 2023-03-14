using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask layer1;

    public LayerMask Layer1 { get => layer1; }

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
        DontDestroyOnLoad(this);
    }
}
