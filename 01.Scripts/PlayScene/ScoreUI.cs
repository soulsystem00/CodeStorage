using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text nickNameText;
    public Text scoreText;
    public Image medalImage;

    public void SetScoreUI(string _nickName, int _score, Sprite _medal = null)
    {
        nickNameText.text = _nickName;
        scoreText.text = _score.ToString();
        if (_medal != null)
            medalImage.sprite = _medal;
    }
}
