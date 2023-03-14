using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class ParticlaControl : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    private ParticleSystem.ShapeModule sm;
    void Awake()
    {
        sm = ps.shape;
        sm.radius = 100f;
        StartCoroutine(asdfasdf());
    }

    IEnumerator asdfasdf()
    {
        yield return new WaitForSeconds(10f);
        DOTween.To(() => sm.radius, x => sm.radius = x, 10f, 90f).SetEase(Ease.Linear);
    }
}
