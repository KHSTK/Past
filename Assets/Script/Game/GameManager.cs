using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public CurrentRoomStateSO currentRoomStateSO;
    public MapGenerator mapGenerator;
    public UpdateState updateState;
    public PlayerStateSO playerStateSO;
    public GameObject Bar;
    public void InitPlayer()
    {
        playerStateSO.currentHp = playerStateSO.maxHp;
        playerStateSO.currentCoin = 10;
        playerStateSO.currentCritical = 5;
    }
    public async void GameWin()
    {
        await loadingScreen.ShowAsync(LoadingScreenType.Normal);
        updateState.GameWinPanel();
        await loadingScreen.HideAsync(LoadingScreenType.Normal);
    }
    public void GameLose()
    {
        updateState.GameLosePanel();
        currentRoomStateSO.ClearCurrentRoom();
    }
    public void GameOver()
    {
        updateState.GameOverPanel();
        currentRoomStateSO.ClearCurrentRoom();
    }
    public void CreatMap()
    {
        mapGenerator.gameObject.SetActive(true);
        mapGenerator.ReGenerateRoom();
    }
    public void OpenBar()
    {
        Bar.SetActive(true);
    }
    public void CloseBar()
    {
        Bar.SetActive(false);
    }
}
