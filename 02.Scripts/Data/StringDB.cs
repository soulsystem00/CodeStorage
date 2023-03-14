using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringDB
{
    public static Dictionary<SkillType, string> SkillName = new Dictionary<SkillType, string>()
    {
        { SkillType.BoosterRange, "속도 증폭"},
        {SkillType.Flash, "부스터"},
        {SkillType.Heal, "회복" },
        {SkillType.HealRange, "광역 회복"},
        {SkillType.Interceptor, "인터셉터"},
        {SkillType.TuttleShip, "거북선"},
        {SkillType.None, "스킬 없음"}
    };

    public static Dictionary<Stat, string> StatName = new Dictionary<Stat, string>()
    {
        {Stat.None, "없음"},
        {Stat.Attack, "공격력"},
        {Stat.AttackSpeed, "공격속도"},
        {Stat.Health, "체력"},
        {Stat.Speed, "속도"},
        {Stat.Critical, "크리티컬"},
        {Stat.CannonCount, "대포갯수"},
        {Stat.Range, "공격범위"},
        {Stat.Explosion, "폭발"},
    };

    public static Dictionary<CannonType, string> CannonName = new Dictionary<CannonType, string>()
    {
        {CannonType.None, "없음"},
        {CannonType.Penetrate,"관통탄"},
        {CannonType.Explosion, "폭발탄"},
        {CannonType.Flame, "화염탄"},
        {CannonType.Slow, "슬로우탄"},
        {CannonType.Faint, "기절탄"},
        {CannonType.Silence, "침묵탄"}
    };
}
