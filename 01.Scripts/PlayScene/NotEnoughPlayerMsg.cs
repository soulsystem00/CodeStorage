using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughPlayerMsg : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Text msg;

    void OnEnable()
    {
        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        rectTransform.anchoredPosition = new Vector3(0f, -200f, 0f);
        msg.color = Color.red;

        yield return new WaitForSeconds(0.5f);
        msg.DOFade(0f, 1f);
        rectTransform.DOAnchorPosY(-100f, 1f);
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
