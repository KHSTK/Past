using TMPro;
using DG.Tweening;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float charsPerSecond = 20; // 每秒显示字符数
    [SerializeField] private float startDelay = 0.5f;   // 开始前的延迟
    [SerializeField] private Ease easeType = Ease.Linear;

    private TMP_Text _tmpText;
    private string _fullText;
    private Sequence _sequence;

    void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        _fullText = _tmpText.text; // 保存原始文本
        _tmpText.text = "";        // 初始清空文本
    }

    public void StartTypewriter()
    {
        // 清除现有动画
        _sequence?.Kill();

        _tmpText.text = "";
        _tmpText.maxVisibleCharacters = 0;

        // 计算总持续时间
        float duration = _fullText.Length / charsPerSecond;

        _sequence = DOTween.Sequence()
            .AppendInterval(startDelay)
            .Append(
                DOTween.To(
                    () => _tmpText.maxVisibleCharacters,
                    x => _tmpText.maxVisibleCharacters = x,
                    _fullText.Length,
                    duration
                ).SetEase(easeType)
            );
    }

    // 可选：跳过动画直接显示完整文本
    public void SkipAnimation()
    {
        _sequence?.Complete();
        _tmpText.maxVisibleCharacters = _fullText.Length;
    }

    void OnDestroy()
    {
        _sequence?.Kill(); // 清理动画
    }
}