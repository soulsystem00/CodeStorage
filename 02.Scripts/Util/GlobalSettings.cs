using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class GlobalSettings : GameSingleton<GlobalSettings>
{
    [SerializeField] float speed;
    public Vector3 ShipSpawnOffset = new Vector3(0f, 0.3f, 0f);
    public float Speed => speed;

    public float SkillCoolTime = 10f;
    public float HealRangeDuration = 5f;
    public float BoosterRangeDuration = 5f;
    public float RangeHealAmount = 10f;
    public float HealAmount = 30f;
    public float SupplyBoxAliveTime = 15f;
    public float SupplyBoxSpawnDuration = 5f;
    public float CrashDamage = 5f;

}