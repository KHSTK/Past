using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Character
{
    public CurrentRoomStateSO currentRoomStateSO;
    public EnemyDatabase enemyDataBase;
    private EnemyData enemyData;
    private Animator animator;
    private Sprite sprite;
    public EnemyConfigSO enemyConfig;
    [Header("广播")]
    public SetEnemyUIEventSO setEnemyUIEvent;
    public ObjectEventSO PlayerStartEvent;
    public ObjectEventSO EnemyDeadEvent;
    public ObjectEventSO BossDeadEvent;
    public ObjectEventSO EnemyEndEvent;
    public IntEventSO EnemyDamageEvent;
    public IntEventSO PlayerDamage;
    private int time;
    void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void Init()
    {
        enemyData = currentRoomStateSO.currentRoomData.enemyData;
        UpdataVisual();
        Initialize(enemyData.maxHp);
        time = 0;
        UpdateTime();
        PlayerStartEvent.RaiseEvent(this, this);
    }
    public void UpdataVisual()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = enemyDataBase.GetEnemySprite(enemyData.enemyID);
        animator.runtimeAnimatorController = enemyDataBase.GetEnemyAnimatorController(enemyData.enemyID);
        sprite = GetComponentInChildren<SpriteRenderer>().sprite;
    }
    public void GetDamageContext(DamageContext ctx)
    {
        //DOTO:使用敌人效果
        enemyData.enemyEffect?.ApplyEffect(ctx);
        TakeDamage(ctx.currentDamage);
        PlayerDamage.RaiseEvent(ctx.currentDamage, this);
    }
    public override void TakeDamage(int damage)
    {
        if (!isAlive) return;
        ReduceHP(damage);
        UpdateTime();
        transform.DOShakePosition(0.3f);
        if (HP <= 0)
        {
            StartCoroutine(EnemyDead());
        }
        else
        {
            animator.Play("Hit");
            Debug.Log("启动携程");
            StartCoroutine(AttackCoroutine("Hit"));
        }
        Debug.Log("敌人受伤值:" + damage + "剩余生命值" + HP);
    }
    public IEnumerator AttackCoroutine(string name)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f
            && animator.GetCurrentAnimatorStateInfo(0).IsName(name));
        StartAttack();
    }
    public void UpdateState()
    {
        EnemyState enemyState = new EnemyState
        {
            enemyName = enemyData.enemyName,
            currentHp = HP.ToString(),
            currentDamage = enemyData.damage.ToString(),
            currentTime = time.ToString(),
            enemySprite = sprite
        };
        setEnemyUIEvent.RaiseEvent(enemyState, this);
    }
    public void UpdateTime()
    {
        if (time <= 0)
        {
            time = enemyData.time;
        }
        else
        {
            time--;
        }
        UpdateState();
    }
    public void StartAttack()
    {
        if (time <= 0 && isAlive)
        {
            Attack();
        }
        UpdateState();
        Debug.Log("敌人攻击时间:" + time);
        EnemyEndEvent.RaiseEvent(this, this);
    }
    public void Attack()
    {
        transform.DOMoveX(transform.position.x + 1, 0.2f).onComplete = () =>
        {
            transform.DOMoveX(transform.position.x - 1, 0.2f).onComplete = () =>
            {
                StartAttack();
            };
            Debug.Log("敌人攻击:" + enemyData.damage);
            EnemyDamageEvent.RaiseEvent(enemyData.damage, this);
        };
        UpdateTime();
    }
    public IEnumerator EnemyDead()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Enemy Dead");
        if (enemyData.enemyType == EnemyType.Boss)
        {
            BossDeadEvent.RaiseEvent(this, this);
        }
        else
        {
            EnemyDeadEvent.RaiseEvent(this, this);
        }
    }

}
