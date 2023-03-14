using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using Google.Protobuf.ProtoObject;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class ShipStatManager : RealTimeMonoBehaviour
{
    public Dictionary<Stat, float> stats = new Dictionary<Stat, float>()
    {
        { Stat.Attack, 0f},
        {Stat.AttackSpeed, 0f},
        {Stat.CannonCount, 1f},
        {Stat.Health, 0f },
        {Stat.Critical, 0f},
        {Stat.Explosion, 0f},
        {Stat.Range, 0f},
        {Stat.Speed, 0f}
    };

    Dictionary<Stat, float> cannonBoosts = new Dictionary<Stat, float>();
    [SerializeField] GameObject[] Cannons;
    [SerializeField] SailController sailController;
    [SerializeField] CannonController cannonController;
    [SerializeField] ShipHealthController shipHealthController;
    [SerializeField] NickNameUI nickNameUI;
    [SerializeField] GameObject turtle;
    [SerializeField] SkillManager skillManager;

    float speedBase = 200f;
    float speedIcn = 20f;
    public int[] cannon;
    public SkillType skill = SkillType.None;
    public string team;
    public int index;
    public string nickName;
    public bool turtleShip = false;
    public bool interceptor = false;
    public bool boost = false;

    private float igniteTime;
    private float slowTime;
    private float faintTime;
    private float silenceTime;

    RealTimeEventManager realTimeEventManager;

    public float MoveSpeed
    {
        get
        {
            if (faintTime <= 0f)
            {
                if (boost && slowTime <= 0f)
                {
                    return (speedBase + (speedIcn * stats[Stat.Speed])) * 2;
                }
                else if (boost && slowTime > 0f)
                {
                    return (speedBase + (speedIcn * stats[Stat.Speed]));
                }
                else if (!boost && slowTime <= 0f)
                {
                    return (speedBase + (speedIcn * stats[Stat.Speed]));
                }
                else
                {
                    return (speedBase + (speedIcn * stats[Stat.Speed])) / 2;
                }
            }
            else
            {
                return 0f;
            }
        }
    }
    public float TurnSpeed { get => speedBase + stats[Stat.Speed]; }
    public float Health { get => 150f + (10f * stats[Stat.Health]); }
    public float Critical { get => 10f * stats[Stat.Critical]; }
    public float AttackSpeed
    {
        get
        {
            if (silenceTime <= 0f)
            {
                return 0.2f * stats[Stat.AttackSpeed];
            }
            else
                return -9999f;
        }
    }
    public float CannonCount { get => stats[Stat.CannonCount]; }
    public float AttackRange { get => 5f + (0.5f * stats[Stat.Range]); }
    public float Explosion { get => stats[Stat.Explosion]; }
    public float Attack { get => 10f + (10f * stats[Stat.Attack]); }
    public float curHp { get; set; }
    public float IgniteTime { get => igniteTime; }
    public float SlowTime { get => slowTime; }
    public float FaintTime { get => faintTime; }
    public float SilenceTime { get => silenceTime; }

    public Action<float> OnRangeChanged = null;
    public Action<SkillType, bool> OnSkillChanged = null;

    void Awake()
    {
        cannon = new int[7];
        realTimeEventManager = RealTimeEventManager.Instance;
        AddEvent();
        StartCoroutine(CheckIgnite());
        curHp = 1f;
    }

    void Start()
    {
        if (realtimeView.IsMine)
            realtimeView.RPC("RPCSetOrderNum", RpcTarget.All, ShipSpawner.i.myIndex);
    }

    void Update()
    {
        DecreaseCCTime();
    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCStatSyncEvent += GCStatSyncEvent;
        realTimeEventManager.OnNewJoinPlayerEvent += StatSyncEvent;
        realTimeEventManager.OnGCBattleStart += GCBattleStartEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCStatSyncEvent -= GCStatSyncEvent;
        realTimeEventManager.OnNewJoinPlayerEvent -= StatSyncEvent;
        realTimeEventManager.OnGCBattleStart -= GCBattleStartEvent;
    }

    private void GCStatSyncEvent(IMessage _packet)
    {
        G_C_Stat_Sync packet = (G_C_Stat_Sync)_packet;
        if (realtimeView.OwnerId == packet.SessionID)
        {
            InitStat(packet.ShipStat);
        }
    }

    public void StatSyncEvent(RT_G_C_New_Join_Player obj = null)
    {
        C_G_Stat_Sync packet = new C_G_Stat_Sync();
        packet.SessionID = realtimeView.OwnerId;
        packet.ShipStat = GetStat();
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGStatSync, packet);
    }

    private void GCBattleStartEvent(IMessage obj)
    {
        if (realtimeView.IsMine)
            skillManager.Init(skill);
    }

    private void DecreaseCCTime()
    {
        if (igniteTime > 0f)
        {
            igniteTime -= Time.deltaTime;
        }
        if (slowTime > 0f)
        {
            slowTime -= Time.deltaTime;
        }
        if (faintTime > 0f)
        {
            faintTime -= Time.deltaTime;
        }
        if (silenceTime > 0f)
        {
            silenceTime -= Time.deltaTime;
        }
    }

    IEnumerator CheckIgnite()
    {
        while (true)
        {
            if ((realtimeView.IsMine || (realtimeView.IsOutOwner && RealTimeNetwork.IsMasterClient)) && igniteTime > 0f)
            {
                realtimeView.RPC("RPCApplyDamage", RpcTarget.All, 5f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void ApplyBoost(IslandType _islandType, float _amount, Stat? _stat = null, SkillType? _skill = null, CannonType? _cannon = null, bool _enable = true)
    {
        if (_islandType == IslandType.AllStat)
        {
            AllStatIncrase(_enable);
        }
        else if (_islandType == IslandType.StatUpgrade)
        {
            UpgradeStat(_amount, _stat);
        }
        else if (_islandType == IslandType.CannonUpgrade)
        {
            UpgardeCannon(_cannon, _enable);
        }
        else if (_islandType == IslandType.Skill)
        {
            SetSkill(_skill.Value, _enable);
        }

        StatSyncEvent();
    }

    private void AllStatIncrase(bool _enable)
    {
        if (_enable)
        {
            stats[Stat.Attack] += 1;
            stats[Stat.AttackSpeed] += 1;
            stats[Stat.CannonCount] += 1;
            stats[Stat.Critical] += 1;
            stats[Stat.Explosion] += 10;
            stats[Stat.Health] += 1;
            stats[Stat.Range] += 1;
            stats[Stat.Speed] += 1;
        }
        else
        {
            stats[Stat.Attack] -= 1;
            stats[Stat.AttackSpeed] -= 1;
            stats[Stat.CannonCount] -= 1;
            stats[Stat.Critical] -= 1;
            stats[Stat.Explosion] -= 10;
            stats[Stat.Health] -= 1;
            stats[Stat.Range] -= 1;
            stats[Stat.Speed] -= 1;
        }

        Clamp(Stat.Attack, 0f, 10f);
        Clamp(Stat.AttackSpeed, 0f, 5f);
        Clamp(Stat.CannonCount, 1f, 7f);
        Clamp(Stat.Critical, 0f, 10f);
        Clamp(Stat.Explosion, 10f, 100f);
        Clamp(Stat.Health, 0f, 10f);
        Clamp(Stat.Range, 0f, 10f);
        Clamp(Stat.Speed, 0f, 9f);

        OnRangeChanged?.Invoke(AttackRange);
        cannonController.SetActiveCannon((int)CannonCount);
        sailController.SetActiveSail((int)stats[Stat.Speed]);
        shipHealthController.SetMaxHealth(Health);
    }

    private void UpgradeStat(float _amount, Stat? _stat)
    {
        if (stats.ContainsKey(_stat.Value))
        {
            stats[_stat.Value] += _amount;

            if (_stat == Stat.Range)
            {
                Clamp(_stat, 0f, 10f);
                OnRangeChanged?.Invoke(AttackRange);
            }
            else if (_stat == Stat.CannonCount)
            {
                Clamp(_stat, 1f, 7f);
                cannonController.SetActiveCannon((int)CannonCount);
            }
            else if (_stat == Stat.Speed)
            {
                Clamp(_stat, 0f, 9f);
                sailController.SetActiveSail((int)stats[Stat.Speed]);
            }
        }
    }

    private void UpgardeCannon(CannonType? _cannon, bool _enable)
    {
        if (_enable)
            cannon[(int)_cannon.Value] = 1;
        else
            cannon[(int)_cannon.Value] = 0;
    }

    private void SetSkill(SkillType _skill, bool _enable)
    {
        if (_skill == SkillType.TuttleShip)
        {
            turtleShip = _enable;
        }
        else if (_skill == SkillType.Interceptor)
        {
            interceptor = _enable;
        }
        else
        {
            if (_enable)
                this.skill = _skill;
            else
                this.skill = SkillType.None;
        }
        SetTurtle();
    }

    private void SetTurtle()
    {
        turtle.SetActive(turtleShip);
        if (team == "Red")
        {
            turtle.tag = "RedCannon";
        }
        else if (team == "Blue")
        {
            turtle.tag = "BlueCannon";
        }
    }

    private void Clamp(Stat? _stat, float _min, float _max)
    {
        stats[_stat.Value] = Mathf.Clamp(stats[_stat.Value], _min, _max);
    }

    public ShipStat GetStat()
    {
        ShipStat shipStat = new ShipStat();
        shipStat.Attack = stats[Stat.Attack];
        shipStat.AttackSpeed = stats[Stat.AttackSpeed];
        shipStat.CannonCount = stats[Stat.CannonCount];
        shipStat.Critical = stats[Stat.Critical];
        shipStat.Explosion = stats[Stat.Critical];
        shipStat.Health = stats[Stat.Health];
        shipStat.Range = stats[Stat.Range];
        shipStat.Speed = stats[Stat.Speed];
        shipStat.CurHP = curHp;
        shipStat.Tag = this.tag;
        shipStat.Team = team;
        shipStat.Index = index;
        shipStat.NickName = nickName;

        shipStat.Cannon.Add(cannon);
        shipStat.Skill = (int)skill;

        shipStat.TurtleShip = this.turtleShip;
        shipStat.Colleague = this.interceptor;

        return shipStat;
    }

    public void InitStat(ShipStat _stats)
    {
        stats[Stat.Attack] = _stats.Attack;
        stats[Stat.AttackSpeed] = _stats.AttackSpeed;
        stats[Stat.CannonCount] = _stats.CannonCount;
        stats[Stat.Critical] = _stats.Critical;
        stats[Stat.Explosion] = _stats.Explosion;
        stats[Stat.Health] = _stats.Health;
        stats[Stat.Range] = _stats.Range;
        stats[Stat.Speed] = _stats.Speed;

        curHp = _stats.CurHP;
        this.tag = _stats.Tag;

        cannon = _stats.Cannon.ToArray();
        skill = (SkillType)_stats.Skill;

        this.team = _stats.Team;
        this.index = _stats.Index;

        this.nickName = _stats.NickName;

        this.turtleShip = _stats.TurtleShip;
        this.interceptor = _stats.Colleague;

        OnRangeChanged?.Invoke(AttackRange);
        cannonController.SetActiveCannon((int)CannonCount);
        sailController.SetActiveSail((int)stats[Stat.Speed]);
        shipHealthController.RejoinHealth(Health, curHp);
        shipHealthController.SetMaxHealth(Health);
        nickNameUI.SetNickName(nickName, team, realtimeView.OwnerId);
        SetTurtle();

        if (realtimeView.IsMine)
        {
            int prevIndex = ShipSpawner.i.myIndex;
            ShipSpawner.i.myIndex = _stats.Index;
            if (prevIndex != ShipSpawner.i.myIndex)
            {
                realtimeView.RPC("RPCSetOrderNum", RpcTarget.All, ShipSpawner.i.myIndex);
            }
        }


        if (transform.position.x > 500 && realtimeView.IsMine)
        {
            skillManager.Init(skill);
        }
    }

    public void SetNickName(string nickName, string team, int index)
    {
        this.nickName = nickName;
        this.team = team;
        this.index = index;
        nickNameUI.SetNickName(nickName, team, realtimeView.OwnerId);
    }

    public void ApplyIgnite()
    {
        igniteTime += 3f;
        igniteTime = Mathf.Clamp(igniteTime, 0f, 3f);
    }

    public void ApplySlow()
    {
        slowTime += 3f;
        slowTime = Mathf.Clamp(slowTime, 0f, 3f);
    }

    public void ApplyFaint()
    {
        faintTime += 3f;
        faintTime = Mathf.Clamp(faintTime, 0f, 3f);
    }

    public void ApplySilence()
    {
        silenceTime += 3f;
        silenceTime = Mathf.Clamp(silenceTime, 0f, 3f);
    }

    [ContextMenu("ASDFASDFASDF")]
    public void ASDFASDFASDF()
    {
        Debug.Log(stats[Stat.Attack]);
        Debug.Log(stats[Stat.AttackSpeed]);
        Debug.Log(stats[Stat.CannonCount]);
        Debug.Log(stats[Stat.Critical]);
        Debug.Log(stats[Stat.Explosion]);
        Debug.Log(stats[Stat.Health]);
        Debug.Log(stats[Stat.Range]);
        Debug.Log(stats[Stat.Speed]);

        Debug.Log(curHp);
        Debug.Log(this.tag);

        Debug.Log(cannon);
        Debug.Log(skill);

        Debug.Log(this.team);
        Debug.Log(this.index);

        Debug.Log(this.nickName);

        Debug.Log(this.turtleShip);
        Debug.Log(this.interceptor);
    }
}