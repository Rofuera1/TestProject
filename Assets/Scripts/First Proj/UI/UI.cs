using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text TradeCost;
    public Text WalletAmount;
    [Space]
    public ColorPallete ColorsForCartText;

    public void UpdateTradeText(int newAmount)
    {
        if (newAmount == 0) TradeCost.color = ColorsForCartText.ColorNeutral;
        else TradeCost.color = newAmount > 0 ? ColorsForCartText.ColorGood : ColorsForCartText.ColorBad;

        TradeCost.text = "Cart: " + Mathf.Abs(newAmount).ToString();
    }

    public void UpdateWalletText(int newAmount)
    {
        WalletAmount.text = "Player: " + newAmount.ToString();
    }

    private IEnumerator unsucessfullTradeCoroutine;
    public void EffectUnsuccesfullTrade()
    {
        if (unsucessfullTradeCoroutine != null)
        {
            StopCoroutine(unsucessfullTradeCoroutine);
            WalletAmount.rectTransform.anchoredPosition = Vector2.zero;
        }
        
        StartCoroutine(unsucessfullTradeCoroutine = Coroutines.LerpObjectShakeByX(WalletAmount.transform, 10f, 0.1f, EasingFunction.Linear));
    }
}
