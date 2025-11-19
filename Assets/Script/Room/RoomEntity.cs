using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomEntity : MonoBehaviour, IPointerClickHandler
{

    [Header("浮现动画")]
    public float appearDuration = 0.8f;
    public Ease appearEase = Ease.OutBack; // 使用DOTween的缓动类型
    private Vector3 originalScale;
    Color originalColor;
    [Header("状态颜色")]
    public Color lockedColor = Color.gray;
    public Color attainableColor = Color.green;
    public Color visitedColor = Color.white;
    //房间图标
    private SpriteRenderer spriteRenderer;
    //房间数据
    public RoomData roomData;
    public RoomState roomState;
    public RoomDataBase roomDataBase;
    public GameObject lockedIcon;
    [SerializeField] private Enemy enemyPrefab; // 敌人预制体
    [SerializeField] private GameObject eventPanelPrefab; // 事件预制体
    [SerializeField] private CurrentRoomStateSO currentRoomStateSO; // 当前房间SO
    public List<Vector2Int> linkTo = new();
    [Header("广播")]
    public ObjectEventSO clickRoomEvent;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        originalScale = transform.localScale;
    }
    public void SetColor()
    {
        // 初始状态：透明且缩小
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        transform.localScale = originalScale * 0.3f;
    }
    public void PlayAppearAnimation()
    {
        // 使用DOTween同时进行缩放和淡入
        Sequence appearSequence = DOTween.Sequence();
        appearSequence.Join(transform.DOScale(originalScale, appearDuration).SetEase(appearEase));
        appearSequence.Join(spriteRenderer.DOFade(originalColor.a, appearDuration));
        appearSequence.OnComplete(() =>
        {
            Debug.Log("房间" + roomData.roomType + "出现动画完成");
        });
    }
    public void SetupRoom(RoomData roomData)
    {
        // 保存原始颜色（包括房间状态对应的透明度）
        this.roomData = roomData;
        spriteRenderer.sprite = roomDataBase.GetRoomSprite(roomData.roomType);
        UpdateRoomStateVisual(); // 初始化状态颜色
        InitializeRoomContent(); // 初始化房间内容
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
        SetColor();
    }
    private void InitializeRoomContent()
    {
        switch (roomData.roomType)
        {
            case RoomType.Boss:
                Debug.Log("敌人房间:" + roomData.roomType + "具体敌人" + roomData.enemyData.enemyName);
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                break;
            case RoomType.Event:
                Debug.Log("事件房间" + roomData.roomType + "具体事件" + roomData.eventData.eventName);
                break;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击房间");
        //如果房间状态为可达到
        if (roomState == RoomState.Attainable)
        {
            //设置当前房间
            currentRoomStateSO.SetCurrentRoom(roomData, this);
            //触发点击房间事件
            clickRoomEvent.RaiseEvent(this, this);
        }
    }
    public void UpdateRoomStateVisual()
    {
        switch (roomState)
        {
            case RoomState.Locked:
                spriteRenderer.color = lockedColor;
                lockedIcon.SetActive(true);
                break;
            case RoomState.Attainable:
                spriteRenderer.color = attainableColor;
                lockedIcon.SetActive(false);
                break;
            case RoomState.Visited:
                spriteRenderer.color = visitedColor;
                lockedIcon.SetActive(false);
                break;
        }
    }

}
