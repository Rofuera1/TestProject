using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    public TradeContainer Trade;

    public void OnTrade()
    {
        Trade.TryTrade();
    }
}
