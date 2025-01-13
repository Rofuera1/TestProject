using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public UI m_UI;
    public Wallet m_Wallet;
    public TradeContainer m_Trader;

    private void Start()
    {
        m_Wallet.OnBalanceChanged += m_UI.UpdateWalletText;

        m_Trader.OnCostChanged += m_UI.UpdateTradeText;
        m_Trader.OnTradeFailed += m_UI.EffectUnsuccesfullTrade;
    }
}
