//花色枚举
using System;
using UnityEngine;

public enum CardSuit
{
    Spade,//黑桃
    Club,//梅花
    Heart,//红桃
    Diamond//方块
}
public enum SuitType
{
    Black,
    Red
}
public enum CardRank
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    J = 11,
    Q = 12,
    K = 13,
    A = 14
}
public enum SortingMode
{
    ByRank,
    BySuit
}
public enum ComboType
{
    Single,//独行
    Pair,//二连
    Three,//三连
    TwoPairs,//双二连
    Flush,//同花
    Straight,//顺子
    FullHouse,//葫芦
    Four,//四连
    StraightFlush,//同花顺
    RoyalFlush//同花大顺
}
[Flags]
public enum RoomType
{
    Normal = 1,//普通
    Elite = 2,//精英
    Boss = 4,//首领
    Rest = 8,//休息
    Shop = 16,//商店
    Event = 32//事件
}
public enum RoomState
{
    Attainable,
    Locked,
    Visited
}
public enum EnemyType
{
    Normal,
    Elite,
    Boss
}
public enum EffectType
{
    AddCoin,
    AddCritical,
    AddRelic,
    AddGlobalFlag,
    ModifyPlayerHP,
    UnlockSecretPath,
    ChangeIcon,
}
public enum LoadingScreenType
{
    NewGame,
    Normal,
    Player,
    Enemy,
    Mouse,
}