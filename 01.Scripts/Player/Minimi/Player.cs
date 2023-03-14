using System;
using System.Collections;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

[RequireComponent(typeof(betterjump))]
[RequireComponent(typeof(RealTimeView))]
[RequireComponent(typeof(SkillDash))]
[RequireComponent(typeof(SkillStealth))]
[RequireComponent(typeof(SkillHealing))]
[RequireComponent(typeof(SkillInvincibility))]
[RequireComponent(typeof(SkillFlash))]
public class Player : PlayerMove
{
    [Header("반드시 스킬 순서대로 넣기 (Dash, Stealth, Healing, Invincibility, Flash)")]
    [SerializeField] SkillBase[] skills;
    [SerializeField] GameObject arrow;
    [SerializeField] HPBar hPBar;
    public bool isGameStarted = false;

    protected Button skillBtn;
    protected RealTimeEventManager realTimeEventManager;
    protected GameObject controllerObj;
    protected PlayerInfo playerInfo;
    protected GameObject speedBar;
    protected float syncTimer = 0.1f;
    protected float time = 0f;
    private bool isInvincibility;
    private Coroutine CoPlayerSync;

    public bool IsInvincibility { get => isInvincibility; set => isInvincibility = value; }
    public int GetSkillCount { get => skills.Length; }
    public bool GetSkillEnableState { get => skillBtn.gameObject.activeSelf; }

    void Start()
    {
        if (realtimeView.IsMine)
        {
            Init(); // 내 캐릭터만 초기화 됨
        }
        PlayerAdd();
    }

    void FixedUpdate()
    {
        if (isGameStarted)
            Move();
    }

    void Update()
    {
        if (isGameStarted)
        {
            time += Time.deltaTime;
            float x = (Speed - originalSpeed) / (maxSpeed - originalSpeed) * 0.94f;
            speedBar.transform.localScale = new Vector3(x, 0.75f, 1f);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "plate")
        {
            jumpCount = 2;
        }
    }

    void OnDisable()
    {
        if (realtimeView.IsMine && isGameStarted)
        {
            if (skillBtn != null)
                skillBtn.gameObject.SetActive(false);
        }
        if (realtimeView.IsMine)
        {
            realTimeEventManager.OnGCStartEvent -= SetArrow;

        }
    }

    private void Init()
    {
        playerInfo = PlayerInfo.Instance;
        realTimeEventManager = RealTimeEventManager.Instance;
        arrow.SetActive(true);
        realTimeEventManager.OnGCStartEvent += SetArrow;

        SetController();
    }

    protected void PlayerAdd()
    {
        var playerinfo = PlayerInfo.Instance;
        playerinfo.AddPlayerGameObject(realtimeView.OwnerId, this.gameObject);
        if (RealTimeNetwork.IsMasterClient)
        {
            foreach (var player in playerinfo.playerGameobjectList)
            {
                var pos = playerinfo.startPosition[playerinfo.GetIndex(player.Key)];
                var rot = Quaternion.identity;
                var p = player.Value.GetComponent<Player>();
                if (p != null)
                    p.SetStartPoint(pos);
            }
        }
    }

    void SetController()
    {
        var control = Instantiate(ResourceDataManager.minimiControlCanvas).GetComponent<MinimiControlCanvas>();
        controllerObj = control.gameObject;
        controller = control.controller;
        jumpBtn = control.jumpBtn;
        skillBtn = control.skillBtn;
        speedBar = control.speedBar;

        skillBtn.gameObject.SetActive(false);
        jumpBtn.onClick.AddListener(OnJumpBtnClick);

        CoPlayerSync = StartCoroutine(PositionSync());
    }

    void SetArrow(IMessage _packet)
    {
        isGameStarted = true;
    }

    public void SetAttacker(ulong _sessionId)
    {
        realtimeView.RPC("RPCSetAttacker", RpcTarget.All, _sessionId);
    }

    public void SetPlayerHp(float _damage)
    {
        if (!isInvincibility && realtimeView.IsMine)
            realtimeView.RPC("RPCSetHP", RpcTarget.All, hPBar.GetHP() - _damage);
    }

    public void SetPlayerHpByAttacker(float _damage)
    {
        if (!isInvincibility)
            realtimeView.RPC("RPCSetHP", RpcTarget.All, hPBar.GetHP() - _damage);
    }

    public void SetInvincibility(bool _value)
    {
        isInvincibility = _value;
    }

    public void SetStartPoint(Vector3 _pos)
    {
        var rot = transform.rotation;
        realtimeView.RPC("SetPlayerPosition", RpcTarget.All, realtimeView.OwnerId, _pos.x, _pos.y, _pos.z, rot.x, rot.y, rot.z, rot.w);
    }

    public void SetPlayerSkill(PlayerSkill _playerskill)
    {
        if (realtimeView.IsMine)
        {
            skillBtn.onClick.RemoveAllListeners();
            skillBtn.onClick.AddListener(() =>
            {
                skills[(int)_playerskill].OnSkillBtnClicked(skillBtn);
            });

            var skillName = SkillNameDB.GetMinimiSkillName(_playerskill);
            if (skillName != null)
            {
                Text btn = null;
                if (skillBtn != null)
                {
                    btn = skillBtn.GetComponentInChildren<Text>();
                }

                if (btn != null)
                {
                    btn.text = SkillNameDB.minimiSkillNameDB[_playerskill].ToString();
                }
            }

            if (skillBtn != null)
            {
                skillBtn.gameObject.SetActive(true);
            }
        }
    }

    public void SetPlayerBounce()
    {

    }

    public void sendScore(bool _isDead = false)
    {
        if (CoPlayerSync != null)
        {
            StopCoroutine(CoPlayerSync);
        }

        C_G_Score response = new C_G_Score();
        response.SessionID = realtimeView.OwnerId;
        response.Score = (_isDead) ? 500 * (int)time : GetScore();
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGScore, response);
    }

    public int GetScore()
    {
        return (int)(5000f * hPBar.GetHP()) + (PlayerInfo.Instance.GetScore()) + (200 * Timer.Instance.GetAliveTime());
    }

    protected IEnumerator PositionSync()
    {
        while (true)
        {
            if (realtimeView.IsMine && RealTimeNetwork.IsInRoom && hPBar.GetHP() > 0f)
            {
                var pos = transform.position;
                var rot = transform.rotation;
                realtimeView.RPC("SetPlayerPosition", RpcTarget.Others,
                    realtimeView.OwnerId,
                    pos.x, pos.y, pos.z,
                    rot.x, rot.y, rot.z, rot.w);
            }
            yield return new WaitForSeconds(syncTimer);
        }
    }

    [RealTimeRPC]
    public void SetPlayerPosition(
        ulong _ownerId,
        float _posX, float _posY, float _posZ,
        float _rotX, float _rotY, float _rotZ, float _rotW)
    {
        if (realtimeView.OwnerId == _ownerId)
        {
            Vector3 pos = new Vector3(_posX, _posY, _posZ);
            Quaternion qua = new Quaternion(_rotX, _rotY, _rotZ, _rotW);
            this.transform.DOLocalMove(pos, syncTimer);
            this.transform.DOLocalRotateQuaternion(qua, syncTimer);
        }
    }

    [RealTimeRPC]
    public void RPCSetAttacker(ulong _sessionId)
    {
        PlayerInfo.Instance.attackerID = _sessionId;
        if (realtimeView.IsMine)
        {
            if (CoPlayerSync != null)
            {
                StopCoroutine(CoPlayerSync);
            }

            Destroy(controllerObj);
            Instantiate(Resources.Load("Player/Attacker") as GameObject, Vector3.zero, Quaternion.identity);
            RealTimeNetwork.Instantiate("EnemySpawner", Vector3.zero, Quaternion.identity);
            RealTimeNetwork.Destroy(this.gameObject);
        }
    }

    [RealTimeRPC]
    public void RPCSetHP(float _value)
    {
        hPBar.SetHPbar(_value);
    }
}