using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private PlayerStateSO playerSO;
    [Header("广播")]
    [SerializeField] private ObjectEventSO UpdatePlayerStateUI;
    [SerializeField] private ObjectEventSO PlayerDeadEvent;
    void Awake()
    {
        Init();
    }
    public void Init()
    {
        SetPlayerState();
    }
    public void SetPlayerState()
    {
        UpdatePlayerStateUI.RaiseEvent(this, this);
    }
    public override void TakeDamage(int damage)
    {
        playerSO.AddHp(-damage);
        transform.DOShakePosition(0.3f);
        Debug.Log("玩家受到伤害:" + damage);
        if (playerSO.currentHp <= 0)
        {
            Debug.Log("Player Dead");
            StartCoroutine(Dead());
        }
        SetPlayerState();
    }
    private IEnumerator Dead()
    {
        yield return new WaitForSeconds(1f);
        PlayerDeadEvent.RaiseEvent(this, this);
    }
}
