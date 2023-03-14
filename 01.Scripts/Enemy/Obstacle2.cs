using DG.Tweening;
using UnityEngine;
public class Obstacle2 : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalMoveY(0f, 2f);
    }
}