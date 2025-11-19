using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScenesLoadManager : MonoBehaviour
{
    public Player player;
    public MapGenerator mapGenerator;
    public LoadingScreen loadingScreen;
    public CurrentRoomStateSO currentRoomStateSO;
    public static ScenesLoadManager Instance { get; private set; }
    [Header("场景引用")]
    public AssetReference menuScene;
    public AssetReference mapScenes;
    public AssetReference enemyScenes;
    public AssetReference eventScenes;
    public AssetReference RestScenes;
    public AssetReference shopScenes;
    //当前加载的场景实例
    private SceneInstance currentMapScene;
    private SceneInstance currentRoomScene;
    [Header("广播")]
    public ObjectEventSO LoadOverEvent;
    public ObjectEventSO ReturnEvent;
    public ObjectEventSO CreatMapEvent;
    [Header("转场间隔")]
    public int loadTime = 1;
    private bool isCreatOver = false;
    private bool isNewGame = false;
    private LoadingScreenType loadingScreenType = LoadingScreenType.Normal;
    async void Awake()
    {
        // 初始化时加载地图场景
        //LoadMapScene();
        await LoadRoomScene(menuScene);
    }
    public void NewGameEvent()
    {
        isNewGame = true;
        LoadMapScene();
    }
    public void NewGameFalse()
    {
        isNewGame = false;
    }
    public async void LoadMapScene()
    {
        if (isNewGame)
        {
            _ = loadingScreen.ShowAsync(LoadingScreenType.NewGame);
        }
        else
        {
            await loadingScreen.ShowAsync(LoadingScreenType.Player);
            AudioManager.Instance.PlayMusic("walking");
        }
        try
        {
            if (currentRoomScene.Scene.isLoaded)
            {
                await Addressables.UnloadSceneAsync(currentRoomScene).Task;
            }

            currentMapScene = await Addressables.LoadSceneAsync(mapScenes,
                LoadSceneMode.Additive).Task;

            SceneManager.SetActiveScene(currentMapScene.Scene);
            player.gameObject.SetActive(false);
            mapGenerator.gameObject.SetActive(true);
        }
        finally
        {
            if (isNewGame)
            {
                while (isNewGame)
                {
                    await Task.Delay(100);
                }
            }
            await Task.Delay(loadTime);
            if (!isCreatOver)
            {
                Debug.Log("将要触发CreatMapEvent");
                CreatMapEvent.RaiseEvent(this, this);
                isCreatOver = true;
            }
            await loadingScreen.HideAsync(LoadingScreenType.Player);
            AudioManager.Instance.PlayMusic("BGM1");
        }
    }
    public async void GetRoomType()
    {
        switch (currentRoomStateSO.currentRoomData.roomType)
        {
            case RoomType.Normal:
            case RoomType.Elite:
            case RoomType.Boss:
                loadingScreenType = LoadingScreenType.Enemy;
                await LoadRoomScene(enemyScenes);
                player.gameObject.SetActive(true);
                break;
            case RoomType.Rest:
                loadingScreenType = LoadingScreenType.Mouse;
                await LoadRoomScene(RestScenes);
                break;
            case RoomType.Event:
                loadingScreenType = LoadingScreenType.Player;
                await LoadRoomScene(eventScenes);
                break;
            case RoomType.Shop:
                loadingScreenType = LoadingScreenType.Player;
                await LoadRoomScene(shopScenes);
                break;
        }
    }
    // 加载房间场景
    private async Task LoadRoomScene(AssetReference roomScene)
    {
        await loadingScreen.ShowAsync(loadingScreenType);
        try
        {
            if (currentMapScene.Scene.isLoaded)
            {
                await Addressables.UnloadSceneAsync(currentMapScene).Task;
            }
            else if (currentRoomScene.Scene.isLoaded)
            {
                await Addressables.UnloadSceneAsync(currentRoomScene).Task;
            }
            currentRoomScene = await Addressables.LoadSceneAsync(roomScene,
                LoadSceneMode.Additive).Task;

            SceneManager.SetActiveScene(currentRoomScene.Scene);
            mapGenerator.gameObject.SetActive(false);
            if (roomScene != menuScene)
            {
                LoadOverEvent.RaiseEvent(this, this);
            }
        }
        finally
        {
            await Task.Delay(loadTime);
            await loadingScreen.HideAsync(loadingScreenType);
        }
    }
    // 返回
    public void ReturnToMap()
    {
        Debug.Log("返回地图");
        LoadMapScene();
        ReturnEvent.RaiseEvent(this, this);
        mapGenerator.GetCurrentRoom();
        mapGenerator.UpdateRoomStates();
    }
    public async void ReturnToMenu()
    {
        Debug.Log("返回菜单");
        loadingScreenType = LoadingScreenType.Normal;
        isCreatOver = false;
        ReturnEvent.RaiseEvent(this, this);
        await LoadRoomScene(menuScene);
    }
}

