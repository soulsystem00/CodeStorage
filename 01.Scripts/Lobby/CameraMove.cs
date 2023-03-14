using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject minimiSelector;
    [SerializeField] GameObject attackerSelector;
    PlayerInfo playerInfo;

    void Start()
    {
        playerInfo = PlayerInfo.Instance;
        toggle.isOn = playerInfo.isAttacker;
    }

    public void SetSelectMode()
    {
        if (!toggle.isOn)
        {
            Camera.main.transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f);
            SetSelectorPanel(true);
        }
        else
        {
            Camera.main.transform.DORotate(new Vector3(0f, 180f, 0f), 0.5f);
            SetSelectorPanel(false);
        }
    }

    void SetSelectorPanel(bool _value)
    {
        minimiSelector.SetActive(_value);
        attackerSelector.SetActive(!_value);
    }

    void Update()
    {
        playerInfo.isAttacker = toggle.isOn;
    }
}
