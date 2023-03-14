using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using UnityEngine;

public class Dokdo : MonoBehaviour
{
    public IslandBase Base;
    [SerializeField] IslandBase[] Bases;
    [SerializeField] float health;
    [SerializeField] IslandOwner owner;
    [SerializeField] MeshFilter mesh;
    [SerializeField] MeshFilter shadowMesh;
    [SerializeField] HPBar hPBar;
    [SerializeField] SpriteRenderer ping;
    [SerializeField] IslandColliderMaker colliderMaker;
    [SerializeField] IslandText islandText;
    [SerializeField] DokdoCannon[] cannons;

    RealTimeEventManager realTimeEventManager;
    public float maxHealth;
    public int key;
    public float Health { get { return health; } set { health = value; } }
    public IslandOwner Owner { get => owner; private set => owner = value; }
    public ulong? OwnerID { get; private set; }
    public Action<IslandType, float, Stat, SkillType, CannonType, bool> OnChangeOwner = null;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        maxHealth = 500f;
        AddEvent();
        SetOwner();
    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCDokDoStat += InitStat;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCDokDoStat -= InitStat;
    }

    private void InitStat(IMessage obj)
    {
        G_C_Dokdo_Stat packet = (G_C_Dokdo_Stat)obj;
        hPBar.ApplyDamage(maxHealth - packet.Hp, maxHealth);
        SetOwner(packet.OwnerID);
    }

    public void Init(int _key, float _hp, ulong? _session_id, float difficulty = 1f)
    {
        this.health = _hp;
        this.key = _key;

        hPBar.ApplyDamage((maxHealth - _hp), maxHealth);
        SetOwner(_session_id);

        colliderMaker?.MakeCollider();
    }

    public void ApplyDamage(float _damage, ulong _session_id)
    {
        health -= _damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health <= 0)
        {
            health = maxHealth;
            hPBar.ApplyDamage(-maxHealth, maxHealth);
            SetOwner(_session_id);
        }
        else
        {
            hPBar.ApplyDamage(_damage, maxHealth);
        }
    }

    public void Shot(float _degree, Vector3 _pos, int _index)
    {
        cannons[_index].Shot(_degree, _pos);
    }

    public IslandOwner GetTeam()
    {
        return Owner;
    }

    void SetOwner(ulong? _session_id = null)
    {
        // 소유권자 설정
        // 1. 널 값일 때 - 중립
        // 2. 클라이언트 세션 아이디와 공격자 아이디가 같을 때 - 내꺼
        // 3. 클라이언트 팀과 공격자 팀이 같은 때 - 아군
        // 4. 클라이언트 팀과 공격자 팀이 다를 때 - 적군

        var prevOwner = this.Owner;
        OwnerID = _session_id;
        if (!_session_id.HasValue)
        {
            Owner = IslandOwner.Neutrality;
            this.gameObject.tag = "DokdoNeutrality";
            ping.color = Color.yellow;
        }
        else if (RealTime.RealTimeNetwork.SessionId == _session_id.Value) // 내꺼
        {
            Owner = IslandOwner.Mine;
            this.gameObject.tag = "DokdoMine";
            ping.color = Color.blue;
        }
        else if (ShipSpawner.i.GetTeam(RealTime.RealTimeNetwork.SessionId) == ShipSpawner.i.GetTeam(_session_id.Value)) // 아군
        {
            Owner = IslandOwner.Friendly;
            this.gameObject.tag = "DokdoFriendly";
            ping.color = Color.green;
        }
        else if (ShipSpawner.i.GetTeam(RealTime.RealTimeNetwork.SessionId) != ShipSpawner.i.GetTeam(_session_id.Value)) // 적군
        {
            Owner = IslandOwner.Enemy;
            this.gameObject.tag = "DokdoEnemy";
            ping.color = Color.red;
        }

        SetBuff(prevOwner);

        hPBar.SetHpbarColor(Owner);
    }

    private void SetBuff(IslandOwner _prevOwner)
    {
        if ((_prevOwner == IslandOwner.Neutrality || _prevOwner == IslandOwner.Enemy) && (Owner == IslandOwner.Mine || Owner == IslandOwner.Friendly))
        {
            OnChangeOwner?.Invoke(IslandType.AllStat, 0, Stat.None, SkillType.None, CannonType.None, true);
            DisplayText();
        }
        else if ((_prevOwner == IslandOwner.Mine || _prevOwner == IslandOwner.Friendly) && Owner == IslandOwner.Enemy)
        {
            OnChangeOwner?.Invoke(IslandType.AllStat, 0, Stat.None, SkillType.None, CannonType.None, false);
        }
    }

    private void DisplayText()
    {
        islandText.DisplayText("AllStat");
    }
}