using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : MonoBehaviour
{
    [SerializeField] ShipHealthController shipHealthController;

    public void Heal()
    {
        shipHealthController.ApplyDamage(-GlobalSettings.Instance.HealAmount);
    }
}
