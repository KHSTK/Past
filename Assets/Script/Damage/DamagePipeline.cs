using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamagePipeline : MonoBehaviour
{
    [Header("广播")]
    public ObjectEventSO EnemyStartEvent;
    public DamageEventSO PlayerDamageEvent;
    [SerializeField] private List<IDamageProcessor> _processors = new();//
    void Awake()
    {
        _processors = GetComponentsInChildren<IDamageProcessor>()
        .OrderBy(p => p.ExecutionOrder)
        .ToList();
    }

    public void AddProcessor(IDamageProcessor processor)
    {
        _processors.Add(processor);
        _processors = _processors.OrderBy(p => p.ExecutionOrder).ToList();
    }

    public IEnumerator RunPipeline(DamageContext ctx)
    {
        foreach (var processor in _processors)
        {
            yield return processor.Execute(ctx);
            // 步骤间间隔
            yield return new WaitForSeconds(0.2f);
        }
        foreach (var card in ctx.playedCards)
        {
            card.Attack();
        }
        PlayerDamageEvent.RaiseEvent(ctx, this);
        yield return new WaitForSeconds(0.7f);
        EnemyStartEvent.RaiseEvent(this, this);
        //ctx.IsFinal = true;

        //yield return FinalizeDamage(ctx);
    }
}
