using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RealTime;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    firstSkill,
    secondSkill,
    thirdSkill,
    forthSkill,
    busy,
}

public class AttackerBase : RealTimeMonoBehaviour
{
    [SerializeField] protected RuntimePlatform runtimePlatform;
    [SerializeField] protected ExpBar expBar;
    [SerializeField] protected SkillSelectPanel skillSelectPanel;
    [SerializeField] Button[] skillBtns;

    protected State state = State.firstSkill;
    protected State prevState = State.firstSkill;

    protected int level = 0;
    protected float attackInc = 0.05f;

    protected Dictionary<Enemy, Action> skillDB;
    protected Dictionary<Enemy, float> skillcoolDB;
    protected Dictionary<Enemy, Button> btnDB;
    protected Action[] Skills;

    Coroutine co1;
    Coroutine co2;
    Coroutine co3;
    protected Coroutine coolDownCo;

    RealTimeEventManager realTimeEventManager;
    PlayerInfo playerInfo;

    protected virtual void InitSkillDB() { }

    protected virtual void HandleUpdate() { }

    void Awake()
    {
        if (runtimePlatform == Application.platform)
        {
            skillDB = new Dictionary<Enemy, Action>();
            skillcoolDB = new Dictionary<Enemy, float>();
            btnDB = new Dictionary<Enemy, Button>();
            Skills = new Action[4];
            realTimeEventManager = RealTimeEventManager.Instance;
            playerInfo = PlayerInfo.Instance;

            realTimeEventManager.OnGCEndEvent += EventGCEnd;
            realTimeEventManager.OnDisconnectRoomEvent += EventGCEnd;

            playerInfo.OnAttackCntInc += EventAttackCntInc;
        }
    }

    void Start()
    {
        if (runtimePlatform == Application.platform)
        {
            InitSkillDB();
            InitBtn();
            AddEventSkillPanel();

            co1 = StartCoroutine(ActiveSkillPanel());
            co2 = StartCoroutine(IncreaseExp(0.008f));
            co3 = StartCoroutine(FullExpCheck());
        }
    }

    void Update()
    {
        if (runtimePlatform == Application.platform)
        {
            HandleUpdate();
        }
    }

    void InitBtn()
    {
        skillBtns[0].image.color = Color.red;
        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i;
            skillBtns[index].onClick.AddListener(() => SkillChange((State)index, skillBtns[index]));
            skillBtns[index].gameObject.SetActive(false);
        }
    }

    void AddEventSkillPanel()
    {
        skillSelectPanel.OnSkillSelected += (_skill) =>
        {
            if (level <= 1)
            {
                skillSelectPanel.gameObject.SetActive(false);
                ActivateSkillBtn(level, _skill);
            }

            else if (level == 2)
            {
                skillSelectPanel.gameObject.SetActive(false);
                SpawnObstacle(_skill);
            }
            else
            {
                skillSelectPanel.gameObject.SetActive(false);
                ActivateSkillBtn(level - 1, _skill);
            }
        };
        skillSelectPanel.EnrollSkill += (_skill) =>
        {
            if (level <= 1)
            {
                Skills[level] = skillDB[_skill];
                btnDB.Add(_skill, skillBtns[level]);
            }
            else if (level == 2)
            {

            }
            else
            {
                Skills[level - 1] = skillDB[_skill];
                btnDB.Add(_skill, skillBtns[level - 1]);
            }
        };
    }

    private void SpawnObstacle(Enemy _skill)
    {
        if (_skill == Enemy.Obstacle)
        {
            RealTimeNetwork.Instantiate("Enemy/Rotator", new Vector3(0f, 10f, 0f), Quaternion.Euler(90f, 0f, 0f));
        }
        else if (_skill == Enemy.Obstacle2)
        {
            RealTimeNetwork.Instantiate("Enemy/Rotator2", new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
    }

    void EventGCEnd<T>(T _packet)
    {
        skillSelectPanel.gameObject.SetActive(false);
        if (co2 != null)
            StopCoroutine(co2);
        if (co3 != null)
            StopCoroutine(co3);
        StartCoroutine(SetBusy());
    }

    private void EventAttackCntInc()
    {
        expBar.SetExpbar(expBar.GetExp() + attackInc);
    }

    private void ActivateSkillBtn(int _level, Enemy _skill)
    {
        for (var i = 0; i < skillBtns.Length; i++)
        {
            skillBtns[i].gameObject.SetActive(i <= _level);
        }
        var skillName = SkillNameDB.GetAttackerSkillName(_skill);
        if (skillName != null)
            skillBtns[_level].GetComponentInChildren<Text>().text = skillName;
    }

    private void SkillChange(State _state, Button _skillBtn)
    {
        if (prevState != _state)
        {
            if (coolDownCo != null)
            {
                StopCoroutine(coolDownCo);
            }
            prevState = _state;
            state = _state;
            StartCoroutine(FillButton(_skillBtn));
        }
    }

    protected void AddSkillDB(Enemy _enemy, Action _action)
    {
        if (!skillDB.ContainsKey(_enemy))
        {
            skillDB.Add(_enemy, _action);
        }
    }

    protected void AddSkillCoolDB(Enemy _enemy, float _coolTime)
    {
        if (!skillcoolDB.ContainsKey(_enemy))
        {
            skillcoolDB.Add(_enemy, _coolTime);
        }
    }

    protected IEnumerator DisableBtn(Button _skillBtn, float _coolTime)
    {
        _skillBtn.interactable = false;
        _skillBtn.image.fillAmount = 0f;
        yield return _skillBtn.image.DOFillAmount(1f, _coolTime).SetEase(Ease.Linear).OnComplete(() => { if (_skillBtn.image.fillAmount >= 1f) _skillBtn.interactable = true; });
    }

    protected IEnumerator SkillCoolDown(float _coolTime)
    {
        prevState = state;
        state = State.busy;
        yield return new WaitForSeconds(_coolTime);
        yield return new WaitUntil(() => skillSelectPanel.gameObject.activeSelf == false);
        state = prevState;
    }

    protected IEnumerator IncreaseExp(float _amount)
    {
        while (true)
        {
            expBar.SetExpbar(expBar.GetExp() + _amount);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => skillSelectPanel.gameObject.activeSelf == false);
        }
    }

    IEnumerator FillButton(Button _skillBtn)
    {
        foreach (var btn in skillBtns)
        {
            if (btn != _skillBtn)
            {
                btn.image.color = Color.white;
                if (btn.interactable)
                {
                    btn.interactable = false;
                    btn.image.fillAmount = 0f;
                    yield return btn.image.DOFillAmount(1f, 1f).SetEase(Ease.Linear).OnComplete(() => { if (btn.image.fillAmount >= 1f) btn.interactable = true; });
                }
            }
            else
            {
                btn.image.color = Color.red;
            }
        }
    }

    IEnumerator FullExpCheck()
    {
        while (level < 4)
        {
            if (expBar.expBar.localScale.x >= 1)
            {
                LevelUp();

                expBar.curExp = 0;
                expBar.expBar.transform.DOScaleX(0f, 0.1f);
                yield return new WaitUntil(() => skillSelectPanel.gameObject.activeSelf == false);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ActiveSkillPanel()
    {
        skillSelectPanel.gameObject.SetActive(true);
        skillSelectPanel.SetSkill(level);
        var tmp = state;
        state = State.busy;
        yield return new WaitUntil(() => skillSelectPanel.gameObject.activeSelf == false);
        state = tmp;
    }

    IEnumerator SetBusy()
    {
        while (true)
        {
            state = State.busy;
            yield return null;
        }
    }

    void LevelUp()
    {
        level++;
        StartCoroutine(ActiveSkillPanel());
    }

    void OnDestroy()
    {
        if (runtimePlatform == Application.platform)
        {
            realTimeEventManager.OnGCEndEvent -= EventGCEnd;
            realTimeEventManager.OnDisconnectRoomEvent -= EventGCEnd;
            playerInfo.OnAttackCntInc -= EventAttackCntInc;
        }

    }
}
