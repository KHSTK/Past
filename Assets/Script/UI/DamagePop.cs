using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DamagePop : MonoBehaviour
{
    public GameObject comboText;
    public GameObject damageText;
    public TextPoolTool textPoolTool;
    // 获取伤害上下文
    public void GetDamageContext(DamageContext currentContext)
    {
        // 设置文本
        SetText(currentContext);
    }

    // 设置文本激活状态
    public void SetTextActive(DamageContext damageContext)
    {
        // 如果damageContext中的playedCards数量为0
        if (damageContext.playedCards.Count == 0)
        {
            // 设置文本不激活
            SetTextActivefalse();
        }
        else
        {
            // 设置comboText激活
            comboText.SetActive(true);
            // 设置damageText激活
            damageText.SetActive(true);
        }
    }
    // 设置文本为不可见
    public void SetTextActivefalse()
    {
        // 设置comboText为不可见
        comboText.SetActive(false);
        // 设置damageText为不可见
        damageText.SetActive(false);
    }
    // 设置文本
    public void SetText(DamageContext damageContext)
    {
        // 设置文本激活
        SetTextActive(damageContext);
        // 设置comboText的文本
        comboText.GetComponent<TextMeshProUGUI>().text = damageContext.comboName;
        // 设置damageText的文本
        damageText.GetComponent<TextMeshProUGUI>().text = damageContext.baseDamage.ToString();
    }
    // 创建伤害弹窗
    public void CreateDamagePopup(DamagePopup damagePopup)
    {
        // 从对象池中获取一个弹窗对象
        var popup = textPoolTool.GetGameObjectFromPool();
        popup.GetComponent<TextMeshPro>().fontSize = 5;
        // 设置弹窗的位置
        popup.transform.position = damagePopup.position;
        // 设置弹窗的文本
        popup.GetComponent<TextMeshPro>().text = "+" + damagePopup.popupText;
        // 设置伤害文本的文本
        damageText.GetComponent<TextMeshProUGUI>().text = damagePopup.damageContext.currentDamage.ToString();
        popup.GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 1f);
        popup.GetComponent<TextMeshPro>().DOFade(0, 1.3f).onComplete = () =>
        {
            textPoolTool.ReleaseGameObjectToPool(popup);
        };

    }
    // 创建最终伤害弹窗
    public void CreateFinallyDamagePopup(int FinallyDamage)
    {
        // 从对象池中获取一个弹窗对象
        var popup = textPoolTool.GetGameObjectFromPool();
        // 设置弹窗的位置
        popup.transform.position = new Vector3(0.5f, 0.5f, 0);
        popup.GetComponent<TextMeshPro>().fontSize = 9;
        popup.GetComponent<TextMeshPro>().color = Color.red;
        // 设置弹窗的文本
        popup.GetComponent<TextMeshPro>().text = "-" + FinallyDamage.ToString();
        popup.GetComponent<TextMeshPro>().DOFade(0, 1.3f).onComplete = () =>
        {
            textPoolTool.ReleaseGameObjectToPool(popup);
        };
    }
    public void CreatEnemyDamagePopup(int EnemyDamage)
    {
        var popup = textPoolTool.GetGameObjectFromPool();
        popup.transform.position = new Vector3(6f, -1f, 0);
        popup.GetComponent<TextMeshPro>().fontSize = 9;
        popup.GetComponent<TextMeshPro>().color = Color.red;
        popup.GetComponent<TextMeshPro>().text = "-" + EnemyDamage.ToString();
        popup.GetComponent<TextMeshPro>().DOFade(0, 1.3f).onComplete = () =>
        {
            textPoolTool.ReleaseGameObjectToPool(popup);
        };
    }
}
