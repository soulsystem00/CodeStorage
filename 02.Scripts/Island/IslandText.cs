using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class IslandText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buffText2;
    [SerializeField] RectTransform rect;

    public void DisplayText(string _stat)
    {
        rect.anchoredPosition = Vector3.zero;
        buffText2.color = Color.red;
        buffText2.text = _stat + "+";
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0f, rect.DOAnchorPosY(rect.anchoredPosition.y + 2f, 2f));
        sequence.Insert(0f, buffText2.DOFade(0f, 2f));
        sequence.Play();
    }
}
