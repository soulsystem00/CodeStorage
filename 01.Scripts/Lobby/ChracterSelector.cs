using UnityEngine;
using UnityEngine.UI;

public class ChracterSelector : MonoBehaviour
{
    [SerializeField] GameObject[] minimiObjs;
    [SerializeField] Toggle[] minimiSelector;

    void Start()
    {
        // SetVisibleMinimiObj(PlayerInfo.Instance.minimiIndex);
        minimiSelector[PlayerInfo.Instance.minimiIndex].isOn = true;
    }

    public void OnToggleValueChanged(int _index)
    {
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.minimiIndex = _index;
            SetVisibleMinimiObj(_index);
        }
    }

    private void SetVisibleMinimiObj(int _index)
    {
        for (int i = 0; i < minimiObjs.Length; i++)
        {
            if (i == _index)
            {
                minimiObjs[i].SetActive(true);
            }
            else
            {
                minimiObjs[i].SetActive(false);
            }
        }
    }
}
