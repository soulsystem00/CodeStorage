using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Sniper : MonoBehaviour, IAttackFlow
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image image;

    void OnEnable()
    {
        StartCoroutine(AttackFlow());
    }

    public void Init(Vector3 pos)
    {
        rectTransform.anchoredPosition = pos;
        image.color = Color.white;
    }

    public IEnumerator AttackFlow()
    {
        yield return new WaitForSeconds(0.1f);
        yield return rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 30f, 0.5f);
        yield return image.DOFade(0f, 0.5f);
        yield return new WaitUntil(() => image.color.a == 0);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }
}
