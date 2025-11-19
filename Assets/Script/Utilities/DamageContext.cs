using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageContext
{
    public int baseDamage;              // 组合基础伤害
    public ComboType comboType;         // 组合类型
    public string comboName;           // 组合名称
    public int relicDamage;           // 藏品伤害加成
    public int currentDamage;           // 当前总伤害（动态变化）
    public List<CardEntity> playedCards;// 实际打出的卡牌
    public int criticalCount;           // 暴击牌数量
    public CardDeck cardDeck;           // 牌库引用
}
public class DamagePopup
{
    public Vector3 position;
    public string popupText;
    public DamageContext damageContext;
}
