using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 伤害处理器接口
/// </summary>
public interface IDamageProcessor
{
    int ExecutionOrder { get; }          // 执行顺序（0-100-200...）
    IEnumerator Execute(DamageContext ctx); // 协程支持延迟
}
