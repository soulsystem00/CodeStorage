using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEffect : MonoBehaviour
{
    [SerializeField] GameObject flameEffect;
    [SerializeField] GameObject slowEffect;
    [SerializeField] GameObject faintEffect;
    [SerializeField] GameObject silenceEffect;

    public void SetEffect(int[] _cannon)
    {
        if (_cannon[3] == 1)
        {
            flameEffect.SetActive(true);
        }
        if (_cannon[4] == 1)
        {
            slowEffect.SetActive(true);
        }
        if (_cannon[5] == 1)
        {
            faintEffect.SetActive(true);
        }
        if (_cannon[6] == 1)
        {
            silenceEffect.SetActive(true);
        }
    }

    public void DisableAll()
    {
        flameEffect.SetActive(false);
        slowEffect.SetActive(false);
        faintEffect.SetActive(false);
        silenceEffect.SetActive(false);
    }
}
