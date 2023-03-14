using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectPanel : MonoBehaviour
{
    [SerializeField] Button btn1;
    [SerializeField] Button btn2;

    int skillFirst;
    int skillSecond;

    public Action<Enemy> EnrollSkill;
    public Action<Enemy> OnSkillSelected;

    void Start()
    {
        btn1.onClick.AddListener(() =>
        {
            EnrollSkill?.Invoke((Enemy)skillFirst);
            OnSkillSelected?.Invoke((Enemy)skillFirst);
        });

        btn2.onClick.AddListener(() =>
        {
            EnrollSkill?.Invoke((Enemy)skillSecond);
            OnSkillSelected?.Invoke((Enemy)skillSecond);
        });
    }

    public void SetSkill(int _level)
    {
        if (_level <= 1)
        {
            var tmp = ResourceDataManager.unitDB.Count;
            int minIndex = _level * 3;
            int maxIndex = _level * 3 + 3;
            skillFirst = UnityEngine.Random.Range(minIndex, maxIndex);
            skillSecond = UnityEngine.Random.Range(minIndex, maxIndex);
            while (skillFirst == skillSecond)
            {
                skillSecond = UnityEngine.Random.Range(minIndex, maxIndex);
            }
        }
        else if (_level == 2)
        {
            skillFirst = 12;
            skillSecond = 13;
        }
        else if (_level == 3)
        {
            var tmp = ResourceDataManager.unitDB.Count;
            int minIndex = (_level - 1) * 3;
            int maxIndex = (_level - 1) * 3 + 3;
            skillFirst = UnityEngine.Random.Range(minIndex, maxIndex);
            skillSecond = UnityEngine.Random.Range(minIndex, maxIndex);
            while (skillFirst == skillSecond)
            {
                skillSecond = UnityEngine.Random.Range(minIndex, maxIndex);
            }
        }
        else if (_level == 4)
        {
            skillFirst = 9;
            skillSecond = 10;
        }
        var skillName1 = SkillNameDB.GetAttackerSkillName((Enemy)skillFirst);
        var skillName2 = SkillNameDB.GetAttackerSkillName((Enemy)skillSecond);

        if (skillName1 != null)
            btn1.GetComponentInChildren<Text>().text = SkillNameDB.attackerSkillNameDB[(Enemy)skillFirst].ToString();
        if (skillName2 != null)
            btn2.GetComponentInChildren<Text>().text = SkillNameDB.attackerSkillNameDB[(Enemy)skillSecond].ToString();
    }
}