using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Island : MonoBehaviour
{
    public IslandBase Base;
    [SerializeField] IslandBase[] Bases;
    [SerializeField] float health;
    [SerializeField] IslandOwner owner;
    [SerializeField] MeshFilter mesh;
    [SerializeField] MeshFilter shadowMesh;
    [SerializeField] HPBar hPBar;
    [SerializeField] SpriteRenderer ping;
    [SerializeField] IslandText islandText;
    [SerializeField] IslandCannon cannon;
    [SerializeField] float pingSize;
    [SerializeField] GameObject atom;

    public float maxHealth;
    public int key;
    public float Health { get { return health; } set { health = value; } }
    public IslandOwner Owner { get => owner; private set => owner = value; }
    public ulong? OwnerID { get; private set; }
    public Action<IslandType, float, Stat, SkillType, CannonType, bool> OnChangeOwner = null;

    public void Init(int _key, float _hp, ulong? _session_id, float difficulty = 1f)
    {
        this.health = _hp;
        this.key = _key;
        cannon.key = _key;

        hPBar.ApplyDamage((maxHealth - _hp), maxHealth);
        SetOwner(_session_id);
        ping.transform.localScale = Vector3.one * pingSize;
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

    public void Shot(float _degree, Vector3 _pos)
    {
        cannon.Shot(_degree, _pos);
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
        // Debug.Log($"{_session_id} {RealTime.RealTimeNetwork.SessionId}");
        if (!_session_id.HasValue)
        {
            Owner = IslandOwner.Neutrality;
            this.gameObject.tag = "Neutrality";
            ping.color = Color.yellow;
        }
        else if (RealTime.RealTimeNetwork.SessionId == _session_id.Value) // 내꺼
        {
            Owner = IslandOwner.Mine;
            this.gameObject.tag = "Mine";
            ping.color = Color.green;
        }
        else if (ShipSpawner.i.GetTeam(RealTime.RealTimeNetwork.SessionId) == ShipSpawner.i.GetTeam(_session_id.Value)) // 아군
        {
            Owner = IslandOwner.Friendly;
            this.gameObject.tag = "Friendly";
            ping.color = Color.blue;
        }
        else if (ShipSpawner.i.GetTeam(RealTime.RealTimeNetwork.SessionId) != ShipSpawner.i.GetTeam(_session_id.Value)) // 적군
        {
            Owner = IslandOwner.Enemy;
            this.gameObject.tag = "Enemy";
            ping.color = Color.red;
        }

        SetBuff(prevOwner);

        hPBar.SetHpbarColor(Owner);
        if (atom != null && !CompareTag("Neutrality"))
            atom.SetActive(false);
    }

    private void SetBuff(IslandOwner _prevOwner)
    {
        IslandBase prevBase = Base;
        int index = UnityEngine.Random.Range(0, Bases.Length);
        Base = Bases[index];

        if ((_prevOwner == IslandOwner.Neutrality || _prevOwner == IslandOwner.Enemy) && Owner == IslandOwner.Mine)
        {
            OnChangeOwner?.Invoke(Base.Type, Base.Amount, Base.Stat, Base.Skill, Base.Cannon, true);
            DisplayText();
        }
        else if (_prevOwner == IslandOwner.Mine && Owner == IslandOwner.Enemy)
        {
            OnChangeOwner?.Invoke(prevBase.Type, -prevBase.Amount, prevBase.Stat, prevBase.Skill, prevBase.Cannon, false);
        }
    }

    private void DisplayText()
    {
        if (Base.Type == IslandType.AllStat)
        {
            islandText.DisplayText("AllStat");
        }
        else if (Base.Type == IslandType.StatUpgrade)
        {
            islandText.DisplayText(StringDB.StatName[Base.Stat]);
        }
        else if (Base.Type == IslandType.Skill)
        {
            islandText.DisplayText(StringDB.SkillName[Base.Skill]);
        }
        else if (Base.Type == IslandType.CannonUpgrade)
        {
            islandText.DisplayText(StringDB.CannonName[Base.Cannon]);
        }
    }
}