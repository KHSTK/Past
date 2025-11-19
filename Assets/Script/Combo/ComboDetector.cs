using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

public class ComboDetector : MonoBehaviour
{
    public CardDeck cardDeck;
    public TextPoolTool textPoolTool;
    public DamageContext ctx;
    private bool isCalculate;
    private ComboType currentComboType;
    private List<CardEntity> differentCards = new();
    [Header("广播")]
    public DamageEventSO baseDamageEventSO;
    public ObjectEventSO PlayerEndEvent;
    [Header("伤害计算管线")]
    [SerializeField] private DamagePipeline pipeline;
    private void Start()
    {
        // 确保Lua管理器已初始化
        if (!LuaManager.Instance.IsInitialized)
        {
            LuaManager.Instance.OnLuaInitialized += OnLuaInitialized;
        }
    }
    private void OnLuaInitialized()
    {
        Debug.Log("ComboDetector: Lua已初始化");
    }
    #region ComboCheck
    // 检查二连三连四连
    public bool CheckSameNumber(int count)
    {
        // 清空不同牌的列表
        differentCards.Clear();
        // 将选中的牌按牌面值分组
        var groups = cardDeck.selectedCardList.GroupBy(c => c.cardData.rank);
        differentCards = groups.Where(g => g.Count() == 1).Select(g => g.First()).ToList();
        return groups.Any(g => g.Count() == count);
    }
    public bool CheckRoyalFlush()
    {
        differentCards.Clear();

        // 基础条件检查
        if (cardDeck.selectedCardList.Count != 5) return false;

        // 创建目标牌型集合
        var requiredRanks = new HashSet<CardRank>
        {
            CardRank.Ten,
            CardRank.J,
            CardRank.Q,
            CardRank.K,
            CardRank.A
        };
        // 获取当前选中的牌型集合
        var currentRanks = new HashSet<CardRank>(
            cardDeck.selectedCardList.Select(c => c.cardData.rank)
        );
        // 检查牌型匹配
        if (!requiredRanks.SetEquals(currentRanks)) return false;
        // 检查花色一致性
        var suits = cardDeck.selectedCardList.Select(c => c.cardData.suit).Distinct();
        return suits.Count() == 1;
    }
    // 检查双二连
    public bool CheckTwoPairs()
    {
        differentCards.Clear();
        // 仅接受4或5张牌
        if (cardDeck.selectedCardList.Count < 4) return false;
        // 按牌面分组（记录原始卡牌对象）
        var groups = cardDeck.selectedCardList
            .GroupBy(c => c.cardData.rank)
            .Select(g => new
            {
                Key = g.Key,
                Cards = g.ToList(),
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .ToList();

        // 检测有效对子组
        var validPairs = groups.Where(g => g.Count >= 2).Take(2).ToList();

        // 验证条件（必须有两个对子且总对牌数>=4）
        bool isTwoPairs = validPairs.Count >= 2 &&
                         validPairs.Sum(g => g.Count) >= 4 &&
                         !groups.Any(g => g.Count > 2); // 排除三张相同的情况

        if (isTwoPairs)
        {
            // 收集所有不符合条件的卡牌
            differentCards = groups
                .Except(validPairs) // 非对子组
                .SelectMany(g => g.Cards) // 展开所有卡牌
                .Concat( // 处理对子组中多余牌
                    validPairs.SelectMany(g =>
                        g.Cards.Skip(2)) // 每个对子组保留两张
                )
                .ToList();// 将不符合条件的卡牌加入differentCards
            return true;
        }
        return false;
    }
    // 检查 三带二
    public bool CheckFullHouse()
    {
        // 清空differentCards列表
        differentCards.Clear();
        // 将selectedCardList按cardData.rank分组
        var groups = cardDeck.selectedCardList.GroupBy(c => c.cardData.rank);
        var threeOfAKind = groups.FirstOrDefault(g => g.Count() == 3);
        var twoOfAKind = groups.FirstOrDefault(g => g.Count() == 2);
        return threeOfAKind != null && twoOfAKind != null;
    }
    // 检查同花
    public bool CheckFlush()
    {
        // 清空不同花色的牌列表
        differentCards.Clear();
        // 将选中的牌按花色分组
        var groups = cardDeck.selectedCardList.GroupBy(c => c.cardData.suit);
        // 如果有任意一组牌的数量为5，则返回true
        return groups.Any(g => g.Count() == 5);
    }
    // 检查同花顺
    public bool CheckStraight()
    {
        // 清空不同牌的列表
        differentCards.Clear();
        if (cardDeck.selectedCardList.Count < 5) return false;
        // 对牌组进行排序
        var sortedCards = cardDeck.selectedCardList.OrderBy(c => c.cardData.rank).ToList();
        // 遍历排序后的牌组
        for (int i = 0; i < sortedCards.Count - 1; i++)
        {
            // 如果当前牌的值加1不等于下一张牌的值，则返回false
            if (sortedCards[i].cardData.rank + 1 != sortedCards[i + 1].cardData.rank)
            {
                return false;
            }
        }
        // 如果所有牌都连续，则返回true
        return true;
    }

    // 检测数字最高的牌
    public void DetectHighestNumberCard()
    {
        // 获取数字最高的牌
        var sortedCards = cardDeck.selectedCardList.OrderBy(c => c.cardData.rank).ToList();
        // 将除了数字最高的牌以外的牌加入differentCards
        differentCards.Clear();
        // 遍历selectedCardList
        foreach (var card in cardDeck.selectedCardList)
        {
            // 如果当前牌不是数字最高的牌，则将其加入differentCards
            if (card != sortedCards.LastOrDefault())
            {
                differentCards.Add(card);
            }
        }
    }
    #endregion
    public void Detector()
    {
        if (!LuaManager.Instance.IsInitialized)
        {
            Debug.LogError("Lua未初始化，无法检测组合");
            return;
        }
        if (CheckRoyalFlush())
        {
            currentComboType = ComboType.RoyalFlush;
            Setctx(currentComboType);
            Debug.Log("旧梦之核");
            return;
        }
        if (CheckStraight() && CheckFlush())
        {
            currentComboType = ComboType.StraightFlush;
            Setctx(currentComboType);
            Debug.Log("血统");
            return;
        }
        { }
        if (CheckStraight())
        {
            currentComboType = ComboType.Straight;
            Setctx(currentComboType);
            Debug.Log("锁链");
            return;
        }

        if (CheckFlush())
        {
            currentComboType = ComboType.Flush;
            Setctx(currentComboType);
            Debug.Log("血脉");
            return;
        }
        if (CheckFullHouse())
        {
            currentComboType = ComboType.FullHouse;
            Setctx(currentComboType);
            Debug.Log("契约");
            return;
        }
        if (CheckSameNumber(4))
        {
            currentComboType = ComboType.Four;
            Setctx(currentComboType);
            Debug.Log("四重奏");
            return;
        }
        if (CheckTwoPairs())
        {
            currentComboType = ComboType.TwoPairs;
            Setctx(currentComboType);
            Debug.Log("共鸣");
            return;
        }
        if (CheckSameNumber(3))
        {
            currentComboType = ComboType.Three;
            Setctx(currentComboType);
            Debug.Log("三重奏");
            return;
        }
        if (CheckSameNumber(2))
        {
            currentComboType = ComboType.Pair;
            Setctx(currentComboType);
            Debug.Log("双生");
            return;
        }
        DetectHighestNumberCard();
        currentComboType = ComboType.Single;
        Setctx(currentComboType);
        Debug.Log("残影");
    }

    // 排序使得伤害文本按顺序显示
    private void SortCards()
    {
        // 根据cardDeck的sortingMode属性进行排序
        switch (cardDeck.sortingMode)
        {
            // 如果sortingMode为ByRank，则按牌面值进行排序，如果牌面值相同，则按花色进行排序
            case SortingMode.ByRank:
                Debug.Log("ByRank");
                ctx.playedCards = ctx.playedCards
                .OrderBy(c => c.cardData.rank)
                .ThenBy(c => c.cardData.suit)
                .ToList();
                break;
            // 如果sortingMode为BySuit，则按花色进行排序，如果花色相同，则按牌面值进行排序
            case SortingMode.BySuit:
                Debug.Log("BySuit");
                ctx.playedCards = ctx.playedCards
                .OrderBy(c => c.cardData.suit)
                .ThenBy(c => c.cardData.rank)
                .ToList();
                break;
        }
    }
    /// <summary>
    /// 弃无组合牌
    /// </summary>
    public void DiscardCards()
    {
        foreach (var card in differentCards)
        {
            cardDeck.selectedCardList.Remove(card);
            cardDeck.DiscardCard(card);
        }
        differentCards.Clear();
    }
    //鼠标点击出牌按钮计算伤害
    public void OnClickAttackButton()
    {
        if (!isCalculate && cardDeck.selectedCardList.Count != 0)
        {
            isCalculate = true;
            cardDeck.CloseCardClick();
            ctx.playedCards = cardDeck.selectedCardList;
            SortCards();
            PlayerEndEvent.RaiseEvent(this, this);
            StartCoroutine(pipeline.RunPipeline(ctx));
        }
    }
    //开启出牌按钮
    public void OpenAttackButton()
    {
        isCalculate = false;
    }
    //设置ctx传递伤害组合信息
    private void Setctx(ComboType comboType)
    {
        if (!LuaManager.Instance.IsInitialized)
        {
            Debug.LogError("Lua未初始化，使用默认值");
            SetFallbackContext(comboType);
            return;
        }

        LuaTable comboData = LuaManager.Instance.GetTable("ComboConfig", comboType.ToString());

        if (comboData == null)
        {
            Debug.LogWarning($"Lua中未找到组合配置: {comboType}");
            SetFallbackContext(comboType);
            return;
        }

        // 获取伤害和名称
        int damage = comboData.Get<int>("damage");
        string name = comboData.Get<string>("name");

        ctx = new DamageContext
        {
            comboName = name,
            comboType = comboType,
            baseDamage = damage,
            playedCards = cardDeck.selectedCardList,
            criticalCount = cardDeck.selectedCardList.Count(c => c.cardData.isCritical),
            cardDeck = cardDeck
        };

        SortCards();
        //更新UI组合显示
        baseDamageEventSO.RaiseEvent(ctx, this);
    }
    private void SetFallbackContext(ComboType comboType)
    {
        var fallbackData = GetFallbackComboData(comboType);

        ctx = new DamageContext
        {
            comboName = fallbackData.name,
            comboType = comboType,
            baseDamage = fallbackData.damage,
            playedCards = cardDeck.selectedCardList,
            criticalCount = cardDeck.selectedCardList.Count(c => c.cardData.isCritical),
            cardDeck = cardDeck
        };

        SortCards();
        baseDamageEventSO.RaiseEvent(ctx, this);
    }
    private (string name, int damage) GetFallbackComboData(ComboType type)
    {
        // 硬编码的回退值（与原值相同）
        return type switch
        {
            ComboType.RoyalFlush => ("旧梦之核", 2000),
            ComboType.StraightFlush => ("血统", 600),
            ComboType.Straight => ("锁链", 100),
            ComboType.Flush => ("血脉", 125),
            ComboType.FullHouse => ("契约", 175),
            ComboType.Four => ("四重奏", 400),
            ComboType.TwoPairs => ("共鸣", 40),
            ComboType.Three => ("三重奏", 80),
            ComboType.Pair => ("双生", 20),
            ComboType.Single => ("残影", 10),
            _ => ("未知", 10)
        };
    }
}
