using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UpdateState : MonoBehaviour
{
    public ScenesLoadManager scenesLoadManager;
    public DialogueController dialogueController;
    public CurrentRoomStateSO currentRoomStateSO;
    public PlayerStateSO playerState;
    public List<GameObject> enemyStateUI = new List<GameObject>();
    public Animator timeIcon;
    public Image enemyHead;
    public List<GameObject> playerStateUI = new List<GameObject>();
    public GameObject BattelCanvas;
    public GameObject TipsPanel;
    public GameObject ShopCanvas;
    public GameObject ShopPanel;
    public GameObject RestCanvas;
    public GameObject RestPanel;
    public GameObject EventCanvas;
    public List<GameObject> overPanel;
    public TextMeshProUGUI discardNumber;
    public Button discardButton;
    private TextMeshProUGUI enemyHp;
    private TextMeshProUGUI enemyDamage;
    private TextMeshProUGUI enemyTime;
    private TextMeshProUGUI enemyName;
    private TextMeshProUGUI playerHp;
    private TextMeshProUGUI playerCoin;
    private TextMeshProUGUI playerCritical;
    void Awake()
    {
        enemyHp = enemyStateUI[0].GetComponent<TextMeshProUGUI>();
        enemyDamage = enemyStateUI[1].GetComponent<TextMeshProUGUI>();
        enemyTime = enemyStateUI[2].GetComponent<TextMeshProUGUI>();
        enemyName = enemyStateUI[3].GetComponent<TextMeshProUGUI>();


        playerHp = playerStateUI[0].GetComponent<TextMeshProUGUI>();
        playerCoin = playerStateUI[1].GetComponent<TextMeshProUGUI>();
        playerCritical = playerStateUI[2].GetComponent<TextMeshProUGUI>();
    }

    public void UpdateEnemyStateUI(EnemyState enemyState)
    {
        enemyHp.text = enemyState.currentHp;
        enemyDamage.text = enemyState.currentDamage;
        enemyTime.text = enemyState.currentTime;
        enemyName.text = enemyState.enemyName;
        Debug.Log("未更新Icon" + enemyHead.sprite);
        enemyHead.sprite = enemyState.enemySprite;
        Debug.Log("更新Icon" + enemyHead.sprite);
        timeIcon.Play("Time");
        UpdatePlayerStateUI();
    }
    public void UpdatePlayerStateUI()
    {
        playerHp.text = playerState.currentHp.ToString();
        playerCoin.text = playerState.currentCoin.ToString();
        playerCritical.text = playerState.currentCritical + "%";
    }
    public void OpenCanvas()
    {
        if (currentRoomStateSO.currentRoomData == null)
        {
            return;
        }
        switch (currentRoomStateSO.currentRoomData.roomType)
        {
            case RoomType.Normal:
            case RoomType.Elite:
            case RoomType.Boss:
                Debug.Log("打开战斗界面");
                BattelCanvas.SetActive(true);
                TipsPanel.SetActive(true);
                break;
            case RoomType.Shop:
                ShopCanvas.SetActive(true);
                break;
            case RoomType.Rest:
                RestCanvas.SetActive(true);
                break;
            case RoomType.Event:
                EventCanvas.SetActive(true);
                break;
            default:
                break;
        }
    }
    public void EnemyHpChange(int HP)
    {
        enemyHp.text = HP.ToString();
    }
    public void GameWinPanel()
    {
        //overPanel[0].transform.localScale = new Vector3(0, 0, 0);
        overPanel[0].SetActive(true);
        //overPanel[0].transform.DOScale(1, 0.5f);
    }
    public void GameLosePanel()
    {
        overPanel[1].SetActive(true);
    }
    public void GameOverPanel()
    {
        overPanel[2].SetActive(true);
    }
    public void UpdateDiscardNumber(int number)
    {
        discardNumber.text = "(" + number.ToString() + ")";
        if (number <= 0)
        {
            discardButton.interactable = false;
        }
        else
        {
            discardButton.interactable = true;
        }
    }
    public void StartShopDialogue()
    {
        dialogueController.StartDialogue("Shop");
    }
    public void StartRestDialogue()
    {
        dialogueController.StartDialogue("Rest");
    }
    public void OpenPanel(string dialogueName)
    {
        if (dialogueName == "Shop")
        {
            ShopPanel.SetActive(true);
        };
        if (dialogueName == "Rest")
        {
            RestPanel.SetActive(false);
            RestCanvas.SetActive(false);
            scenesLoadManager.ReturnToMap();
        }

    }
}
