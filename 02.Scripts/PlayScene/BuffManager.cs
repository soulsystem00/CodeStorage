using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    [SerializeField] Text buffText;
    [SerializeField] Toggle toggle;
    ShipStatManager shipStatManager;
    public GameObject playerObj;
    float time;

    public void Init(GameObject _playerObj)
    {
        shipStatManager = _playerObj.GetComponent<ShipStatManager>();
    }

    public void Apply(IslandType _islandType, float _amount, Stat _stat, SkillType _skill, CannonType _cannon, bool _enable = true)
    {
        shipStatManager.ApplyBoost(_islandType, _amount, _stat, _skill, _cannon, _enable);
    }
    void Update()
    {
        if (shipStatManager != null)
        {
            if (toggle.isOn)
            {
                time = 0f;
                if (shipStatManager.stats.Count > 0)
                    buffText.text =
            $": {shipStatManager.stats[Stat.Attack]}\n"
            + $": {shipStatManager.stats[Stat.AttackSpeed]}\n"
            + $": {shipStatManager.stats[Stat.CannonCount]}\n"
            + $": {shipStatManager.stats[Stat.Range]}\n"
            + $": {shipStatManager.stats[Stat.Critical]}\n"
            + $": {shipStatManager.stats[Stat.Health]}\n"
            + $": {shipStatManager.stats[Stat.Speed]}\n"
            + $": {StringDB.SkillName[shipStatManager.skill]}\n";
            }
            else
            {
                buffText.text = ": 공격력\n: 공격속도\n: 대포개수\n: 공격범위\n: 크리티컬\n: 체력\n: 이동속도\n: 스킬";
                time += Time.deltaTime;
                if (time >= 3f)
                {
                    toggle.isOn = true;
                    time = 0f;
                }
            }
        }

    }
}