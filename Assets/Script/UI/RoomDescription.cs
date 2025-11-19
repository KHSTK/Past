using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomDescription : MonoBehaviour
{
    private UIDOMove uIDOMove;
    public GameObject roomDescriptionPanel;
    public CurrentRoomStateSO currentRoomState;
    [Header("广播")]
    public ObjectEventSO EntryRoomEvent;
    [Header("UI组件")]
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomDescription;
    public Button entryButton;
    public Button existButton;
    public Image roomImage;
    public Image roomTitleImage;
    public List<Sprite> roomImageList;
    public List<Sprite> roomTitleImageList;
    private bool isAnimating;
    private void Start()
    {
        uIDOMove = GetComponent<UIDOMove>();
        roomDescriptionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1720, -20);
        roomDescriptionPanel.SetActive(false);
    }
    public void GetRoomData()
    {
        //根据房间类型显示房间信息
        Debug.Log("房间类型：" + currentRoomState.currentRoomData.roomType);
        roomName.text = currentRoomState.currentRoomData.GetRoomName();
        roomDescription.text = currentRoomState.currentRoomData.GetRoomDescription();
        switch (currentRoomState.currentRoomData.roomType)
        {
            case RoomType.Normal:
            case RoomType.Elite:
            case RoomType.Boss:
                roomImage.sprite = roomImageList[0];
                roomTitleImage.sprite = roomTitleImageList[0];
                break;
            case RoomType.Shop:
                roomImage.sprite = roomImageList[1];
                roomTitleImage.sprite = roomTitleImageList[1];
                break;
            case RoomType.Event:
                roomImage.sprite = roomImageList[2];
                roomTitleImage.sprite = roomTitleImageList[2];
                break;
            case RoomType.Rest:
                roomImage.sprite = roomImageList[3];
                roomTitleImage.sprite = roomTitleImageList[3];
                break;
            default:
                break;
        }
        //初始点击事件
        entryButton.onClick.RemoveAllListeners();
        //注册点击事件
        ShowRoomDescription();
        entryButton.onClick.AddListener(() =>
        {
            //进入房间
            AudioManager.Instance.PlaySFX("Click");
            EntryRoomEvent.RaiseEvent(this, this);
            HideRoomDescription();
        });
        existButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("Click");
            HideRoomDescription();
        });
    }
    public async void ShowRoomDescription()
    {
        if (isAnimating) return;
        isAnimating = true;
        roomDescriptionPanel.SetActive(true);
        await uIDOMove.RightMoveTo(roomDescriptionPanel.GetComponent<RectTransform>(), -600, 0.5f);
        isAnimating = false;
    }
    public async void HideRoomDescription()
    {
        await uIDOMove.LeftMoveTo(roomDescriptionPanel.GetComponent<RectTransform>(), -1740, 0.5f);
        roomDescriptionPanel.SetActive(false);
    }
}
