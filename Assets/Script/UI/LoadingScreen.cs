using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject Start;
    [SerializeField] private Image blockerImage;
    [SerializeField] private Image barImage;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Image mouseImage;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        // 初始状态
        gameObject.SetActive(false);
    }

    public async Task ShowAsync(LoadingScreenType loadType)
    {
        gameObject.SetActive(true);
        AnimatorFadeIn(loadType);
        await blockerImage.DOFade(1, fadeDuration).AsyncWaitForCompletion();
        while (Start.activeSelf)
        {
            await Task.Delay(100);
        }
    }

    public async Task HideAsync(LoadingScreenType loadType)
    {
        AnimatorFadeOut(loadType);
        await blockerImage.DOFade(0, fadeDuration).AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }
    public void AnimatorFadeIn(LoadingScreenType loadType)
    {
        switch (loadType)
        {
            case LoadingScreenType.NewGame:
                Start.SetActive(true);
                break;
            case LoadingScreenType.Enemy:
                loadingText.text = "你依稀看到一个模糊的影子,是" + "<size=200%><color=#FF0000>谁</color></size>" + "在那?";
                loadingText.DOFade(1, fadeDuration);
                barImage.DOFade(1, fadeDuration);
                enemyImage.DOFade(1, fadeDuration);
                break;
            case LoadingScreenType.Player:
                loadingText.text = "你寻着光亮前进，似乎有什么声音在引导你。";
                loadingText.DOFade(1, fadeDuration);
                barImage.DOFade(1, fadeDuration);
                playerImage.DOFade(1, fadeDuration);
                break;
            case LoadingScreenType.Mouse:
                loadingText.text = ".嘘—向导在对你招手，它似乎有了新的发现…";
                loadingText.DOFade(1, fadeDuration);
                barImage.DOFade(1, fadeDuration);
                mouseImage.DOFade(1, fadeDuration);
                break;
            case LoadingScreenType.Normal:
                loadingText.text = "";
                break;
        }
    }
    public void AnimatorFadeOut(LoadingScreenType loadType)
    {
        switch (loadType)
        {
            case LoadingScreenType.Enemy:
                loadingText.DOFade(0, fadeDuration);
                barImage.DOFade(0, fadeDuration);
                enemyImage.DOFade(0, fadeDuration);
                break;
            case LoadingScreenType.Player:
                loadingText.DOFade(0, fadeDuration);
                barImage.DOFade(0, fadeDuration);
                playerImage.DOFade(0, fadeDuration);
                break;
            case LoadingScreenType.Mouse:
                loadingText.DOFade(0, fadeDuration);
                barImage.DOFade(0, fadeDuration);
                mouseImage.DOFade(0, fadeDuration);
                break;
            default:
                break;
        }
    }
}