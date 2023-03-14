using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image buttonImg;
    [SerializeField] Text buttonTxt;
    [SerializeField] Button button;
    AsyncOperation scene;

    void Awake()
    {
        scene = SceneManager.LoadSceneAsync(2);
        scene.allowSceneActivation = false;
        button.onClick.AddListener(() =>
        {
            scene.allowSceneActivation = true;
        });
    }

    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        button.interactable = false;
        sequence.Append(text.rectTransform.DOAnchorPosY(-220f, 1.5f));
        sequence.OnComplete(() =>
        {
            buttonImg.DOFade(1f, 0.5f);
            buttonTxt.DOFade(1f, 0.5f);
            button.interactable = true;
        });
    }
}
