using System;
using RealTime;
using UnityEngine;

public class AttackerPC : AttackerBase
{
    [SerializeField] LineRenderer lr;

    float skill_increase = 0.02f;

    Vector3 startPos;
    Vector3 endPos;

    protected override void InitSkillDB()
    {
        AddSkillDB(Enemy.Thunder, SpawnThunder);
        AddSkillDB(Enemy.FollowingThunder, SpawnFollowingThunder);
        AddSkillDB(Enemy.CircleThunder, SpawnCircleThunder);

        AddSkillDB(Enemy.Bombing, SpawnBombing);
        AddSkillDB(Enemy.ExplosionBarrel, SpawnExplosionBarrel);
        AddSkillDB(Enemy.RollingBarrel, SpawnRollingBarrel);

        AddSkillDB(Enemy.Turret, SpawnTurret);
        AddSkillDB(Enemy.Mine, SpawnMine);
        AddSkillDB(Enemy.BombBot, SpawnBombBot);

        AddSkillDB(Enemy.Sniper, SpawnSniper);
        AddSkillDB(Enemy.Meteor, SpawnMeteor);
        AddSkillDB(Enemy.Charlse, SpawnCharlse);


        AddSkillCoolDB(Enemy.Thunder, 1f);
        AddSkillCoolDB(Enemy.FollowingThunder, 6f);
        AddSkillCoolDB(Enemy.ExplosionBarrel, 1f);

        AddSkillCoolDB(Enemy.Bombing, 4f);
        AddSkillCoolDB(Enemy.CircleThunder, 3f);
        AddSkillCoolDB(Enemy.RollingBarrel, 3f);

        AddSkillCoolDB(Enemy.Turret, 4f);
        AddSkillCoolDB(Enemy.Mine, 4f);
        AddSkillCoolDB(Enemy.BombBot, 4f);

        AddSkillCoolDB(Enemy.Sniper, 10f);
        AddSkillCoolDB(Enemy.Meteor, 10f);
        AddSkillCoolDB(Enemy.Charlse, 10f);
    }

    protected override void HandleUpdate()
    {
        // Debug.Log(state);
        if (state == State.firstSkill)
        {
            Skills[0]?.Invoke();
        }
        else if (state == State.secondSkill)
        {
            Skills[1]?.Invoke();
        }
        else if (state == State.thirdSkill)
        {
            Skills[2]?.Invoke();
        }
        else if (state == State.forthSkill)
        {
            Skills[3]?.Invoke();
        }
        else if (state == State.busy)
            SetLineStartPoint();

        if (skillSelectPanel.gameObject.activeSelf || state == State.busy)
        {
            lr.enabled = false;
        }
    }

    private void SetLineStartPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
            {
                startPos = hit.point;
                lr.SetPosition(0, hit.point);
            }
        }

    }

    private void SpawnThunder()
    {
        DetectTouchEnd(Enemy.Thunder, SpawnEnemy);
    }

    private void SpawnCircleThunder()
    {
        DetectTouchEnd(Enemy.CircleThunder, SpawnEnemy, default, 0.5f);
    }

    private void SpawnCharlse()
    {
        DetectTouchEnd(Enemy.Charlse, SpawnEnemy, 90f);
    }

    private void SpawnExplosionBarrel()
    {
        DetectTouchEnd(Enemy.ExplosionBarrel, SpawnEnemy);
    }

    private void SpawnRollingBarrel()
    {
        DetectDrag(skillcoolDB[Enemy.RollingBarrel]);
    }

    private void SpawnTurret()
    {
        DetectTouchEnd(Enemy.Turret, SpawnEnemy);
    }

    private void SpawnMine()
    {
        DetectTouchEnd(Enemy.Mine, SpawnEnemy);
    }

    private void SpawnBombing()
    {
        DetectTouchEnd(Enemy.Bombing, SpawnEnemy);
    }

    private void SpawnSniper()
    {
        DetectTouchSniping();
    }

    private void SpawnBombBot()
    {
        DetectTouchEnd(Enemy.BombBot, SpawnEnemy, default, 0.8f);
    }

    private void SpawnFollowingThunder()
    {
        DetectTouchBegin(Enemy.FollowingThunder, SpawnFollowingThunder);
    }

    private void SpawnMeteor()
    {
        DetectTouchBegin(Enemy.Meteor, SpawnEnemy);
    }

    private void DetectTouchSniping()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.playerLayer))
            {
                hit.collider.GetComponent<Player>().SetPlayerHpByAttacker(0.2f);
                EnemySpawner.Instance.SpawnEnemy(Enemy.Sniper, Input.mousePosition, Quaternion.identity, true);
                OnSkillUsed(Enemy.Sniper);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
            {
                EnemySpawner.Instance.SpawnEnemy(Enemy.Sniper, Input.mousePosition, Quaternion.identity, true);
                OnSkillUsed(Enemy.Sniper);
            }
        }
    }

    private void DetectTouchBegin(Enemy _enemy, Action<Ray, Enemy, float, float> callback, float _rotValue = 0f, float _offSet = 0.2f)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            callback?.Invoke(ray, _enemy, _rotValue, _offSet);
        }
    }

    private void DetectTouchEnd(Enemy _enemy, Action<Ray, Enemy, float, float> callback, float _rotValue = 0f, float _offSet = 0.2f)
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            callback?.Invoke(ray, _enemy, _rotValue, _offSet);
        }
    }

    private void DetectDrag(float _coolTime)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
            {
                startPos = hit.point;
                lr.SetPosition(0, hit.point + Vector3.up);
                lr.enabled = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
            {
                lr.enabled = true;
                lr.SetPosition(1, hit.point + Vector3.up);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lr.enabled = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
            {
                endPos = hit.point;
                var dir = endPos - startPos;
                var deg = Mathf.Rad2Deg * Mathf.Atan2(dir.z, dir.x);
                EnemySpawner.Instance.SpawnEnemy(Enemy.RollingBarrel, startPos + new Vector3(0f, 10f, 0f), Quaternion.identity * Quaternion.Euler(0f, -deg, 0f));
                OnSkillUsed(Enemy.RollingBarrel);
            }
        }
    }

    private void SpawnEnemy(Ray _ray, Enemy _enemy, float _rotValue = 0f, float _offSet = 0.2f)
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
        {
            EnemySpawner.Instance.SpawnEnemy(_enemy, hit.point + new Vector3(0f, _offSet, 0f), Quaternion.identity * Quaternion.Euler(_rotValue, 0f, 0f));
            OnSkillUsed(_enemy);
        }
    }

    private void SpawnFollowingThunder(Ray _ray, Enemy _enemy, float _rotValue = 0f, float _offSet = 0.2f)
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit, Mathf.Infinity, GlobalSettings.i.plateLayer))
        {
            EnemySpawner.Instance.SpawnEnemy(_enemy, hit.point + new Vector3(0f, _offSet, 0f), Quaternion.identity * Quaternion.Euler(_rotValue, 0f, 0f));
            // RealTimeNetwork.Instantiate("Enemy/FollowingThunder", hit.point + new Vector3(0f, _offSet, 0f), Quaternion.identity * Quaternion.Euler(_rotValue, 0f, 0f));
            OnSkillUsed(_enemy);
        }
    }

    private void OnSkillUsed(Enemy _enemy)
    {
        expBar.SetExpbar(expBar.GetExp() + skill_increase);
        coolDownCo = StartCoroutine(SkillCoolDown(skillcoolDB[_enemy]));
        StartCoroutine(DisableBtn(btnDB[_enemy], skillcoolDB[_enemy]));
    }
}
