using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Island", menuName = "Island/Create new Island")]
public class IslandBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] IslandOwner owner;
    [SerializeField] IslandType type;
    [SerializeField] Stat stat;
    [SerializeField] SkillType skill;
    [SerializeField] CannonType cannon;
    [SerializeField] float amount;
    [SerializeField] float health;
    [SerializeField] Mesh islandMesh;

    public string Name => name;
    public IslandOwner Owner { get => owner; set => owner = value; }
    public IslandType Type => type;
    public Stat Stat => stat;
    public float Amount => amount;
    public float Health => health;
    public Mesh IslandMesh => islandMesh;

    public SkillType Skill { get => skill; set => skill = value; }
    public CannonType Cannon { get => cannon; set => cannon = value; }
}

[Serializable]
public enum IslandType
{
    None,
    CannonUpgrade,
    StatUpgrade,
    AllStat,
    Skill,
}

[SerializeField]
public enum Stat
{
    None,
    Attack,
    AttackSpeed,
    Health,
    Speed,
    Critical,
    CannonCount,
    Range,
    Explosion,
}

[Serializable]
public enum IslandOwner
{
    None,
    Mine,
    Friendly,
    Enemy,
    Neutrality,
}

[SerializeField]
public enum SkillType
{
    None,
    Flash,
    Interceptor,
    TuttleShip,
    Heal,
    HealRange,
    BoosterRange,
}

[SerializeField]
public enum CannonType
{
    None,
    Penetrate,
    Explosion,
    Flame,
    Slow,
    Faint,
    Silence,
}