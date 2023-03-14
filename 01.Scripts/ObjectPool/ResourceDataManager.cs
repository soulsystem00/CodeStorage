
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 순서 바탕으로 스킬 나옴
/// </summary>
public enum Enemy
{

    // level 1
    FollowingThunder,
    Thunder,
    ExplosionBarrel,

    // level 2
    Bombing,
    CircleThunder,
    RollingBarrel,

    // level 3
    BombBot,
    Turret,
    Mine,

    // level 4
    Sniper,
    Meteor,
    Charlse,

    // obstacle 12, 13
    Obstacle,
    Obstacle2,
}

public class ResourceDataManager : MonoBehaviour
{
    static private bool initState = false;

    // static public GameObject Sample;
    static public GameObject minimiControlCanvas;

    static public GameObject zeusThunder;
    static public GameObject CharlesImage;
    static public GameObject zeusCircleThunder;
    static public GameObject explosionBarrel;
    static public GameObject rollingBarrel;
    static public GameObject turret;
    static public GameObject bullet;
    static public GameObject mine;
    static public GameObject bombard;
    static public GameObject sniper;
    static public GameObject bombBot;
    static public GameObject meteor;
    static public GameObject followingThunder;
    static public GameObject itemBox;
    static public GameObject booster;
    static public GameObject timer;
    static public Dictionary<Enemy, GameObject> unitDB = new Dictionary<Enemy, GameObject>();

    public static void LoadResourcesData()
    {
        if (!initState)
        {
            initState = true;
            minimiControlCanvas = Resources.Load("UI/MinimiControlCanvas") as GameObject;

            zeusThunder = Resources.Load("Enemy/Thunder") as GameObject;
            CharlesImage = Resources.Load("Image") as GameObject;
            zeusCircleThunder = Resources.Load("Enemy/CircleThunder") as GameObject;
            explosionBarrel = Resources.Load("Enemy/ExplosionBarrel") as GameObject;
            rollingBarrel = Resources.Load("Enemy/RollingBarrel") as GameObject;
            turret = Resources.Load("Enemy/Turret") as GameObject;
            bullet = Resources.Load("Enemy/Bullet") as GameObject;
            mine = Resources.Load("Enemy/Mine") as GameObject;
            bombard = Resources.Load("Enemy/Bombard") as GameObject;
            sniper = Resources.Load("Enemy/Sniper") as GameObject;
            bombBot = Resources.Load("Enemy/BombBot") as GameObject;
            meteor = Resources.Load("Enemy/Meteor") as GameObject;
            followingThunder = Resources.Load("Enemy/FollowingThunder") as GameObject;

            itemBox = Resources.Load("PlayScene/ItemBoxObj") as GameObject;
            booster = Resources.Load("PlayScene/Booster") as GameObject;

            timer = Resources.Load("UI/Timer") as GameObject;

            AddEnemy(Enemy.Thunder, zeusThunder);
            AddEnemy(Enemy.CircleThunder, zeusCircleThunder);
            AddEnemy(Enemy.ExplosionBarrel, explosionBarrel);
            AddEnemy(Enemy.RollingBarrel, rollingBarrel);
            AddEnemy(Enemy.Turret, turret);
            AddEnemy(Enemy.Mine, mine);
            AddEnemy(Enemy.Bombing, bombard);
            AddEnemy(Enemy.Sniper, sniper);
            AddEnemy(Enemy.BombBot, bombBot);
            AddEnemy(Enemy.Charlse, CharlesImage);
            AddEnemy(Enemy.Meteor, meteor);
            AddEnemy(Enemy.FollowingThunder, followingThunder);
            // Sample = Resources.Load("Sample") as GameObject;
        }
    }

    public static void AddEnemy(Enemy _enemy, GameObject _go)
    {
        if (!unitDB.ContainsKey(_enemy))
        {
            unitDB.Add(_enemy, _go);
        }
    }

    public static GameObject GetGameObject(Enemy _enemy)
    {
        if (unitDB.ContainsKey(_enemy))
        {
            return unitDB[_enemy];
        }
        else return null;
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position, Quaternion rotate)
    {
        GameObject obj = ObjectPoolManager.Instance.Instantiate(resource, position, rotate);
        T script = obj.GetComponent<T>();

        return script;
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position)
    {
        return CreateObjectAndComponent<T>(resource, position, Quaternion.identity);
    }

    public static T CreateObjectAndComponent<T>(GameObject resource)
    {
        return CreateObjectAndComponent<T>(resource, Vector3.zero, Quaternion.identity);
    }
}

public static class SkillNameDB
{
    public static Dictionary<Enemy, String> attackerSkillNameDB = new Dictionary<Enemy, string>()
    {
        {Enemy.Bombing, "폭격"},
        {Enemy.BombBot, "자폭로봇"},
        {Enemy.Charlse, "찰스"},
        {Enemy.CircleThunder, "파동전류"},
        {Enemy.ExplosionBarrel, "폭약통"},
        {Enemy.Mine, "지뢰"},
        {Enemy.RollingBarrel, "통 굴리기"},
        {Enemy.Sniper, "저격"},
        {Enemy.Turret, "터렛"},
        {Enemy.Thunder, "낙뢰"},
        {Enemy.FollowingThunder, "유도전기"},
        {Enemy.Obstacle, "장애물 1"},
        {Enemy.Obstacle2, "장애물 2"},
        {Enemy.Meteor, "메테오"},
    };

    public static Dictionary<PlayerSkill, string> minimiSkillNameDB = new Dictionary<PlayerSkill, string>()
    {
        {PlayerSkill.Dash, "대쉬"},
        {PlayerSkill.Flash, "점멸"},
        {PlayerSkill.Healing, "회복"},
        {PlayerSkill.Invincibility, "무적"},
        {PlayerSkill.Stealth, "은신"},
    };

    public static string GetAttackerSkillName(Enemy _enemy)
    {
        if (attackerSkillNameDB.ContainsKey(_enemy))
        {
            return attackerSkillNameDB[_enemy];
        }
        return null;
    }

    public static string GetMinimiSkillName(PlayerSkill _skill)
    {
        if (minimiSkillNameDB.ContainsKey(_skill))
        {
            return minimiSkillNameDB[_skill];
        }
        return null;
    }
}