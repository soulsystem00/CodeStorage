using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Image titleImage;
    [SerializeField] Button startBtn;
    [SerializeField] Image btnImage;
    [SerializeField] TextMeshProUGUI btnText;

    void Awake()
    {
        StartCoroutine(TitleAnim());
    }

    IEnumerator TitleAnim()
    {
        startBtn.enabled = false;
        titleImage.color = new Color(1f, 1f, 1f, 0f);
        btnImage.color = new Color(0f, 0f, 0f, 1f);
        btnText.color = new Color(0f, 0f, 0f, 0f);

        titleImage.DOFade(1f, 1.5f).SetEase(Ease.Linear);
        btnImage.DOFade(0f, 1.5f).SetEase(Ease.Linear);
        btnText.DOFade(1f, 1.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        startBtn.enabled = true;

    }

    public void OnGameStartBtnClicked()
    {
        SceneManager.LoadScene(2);
    }
}
