using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TradeContainer : OwnerManager
{
    public int CartCostDelta { get { return _cartCostDelta; } private set { _cartCostDelta = value; OnCostChanged?.Invoke(value); } }
    private int _cartCostDelta;
    private int _cartCostForMerchant;

    public Wallet PlayerWallet;
    public Wallet MerchantWallet;

    public Action<int> OnCostChanged;
    public Action OnTradeFailed;
    public Action OnTradeSuccess;

    public override void AddItem(SingleItem item)
    {
        base.AddItem(item);
        CartCostDelta += relativeCostForPerson(item);
        _cartCostForMerchant += relativeCostForMerchant(item);
    }

    public override void RemoveItem(SingleItem item)
    {
        base.RemoveItem(item);
        CartCostDelta -= relativeCostForPerson(item);
        _cartCostForMerchant -= relativeCostForMerchant(item);
    }

    private int relativeCostForPerson(SingleItem item)
    {
        return (item.Owner == Location.Merchant ? item.CostFromMerchant : -item.CostFromPlayer);
    }

    private int relativeCostForMerchant(SingleItem item)
    {
        return (item.Owner == Location.Player ? item.CostFromPlayer : -item.CostFromMerchant);
    }

    public void TryTrade()
    {
        if (!PlayerWallet.IsPurchasePossible(_cartCostDelta) || !MerchantWallet.IsPurchasePossible(_cartCostForMerchant))
        {
            OnTradeFailed?.Invoke();
            return;
        }

        if(_cartCostDelta > 0)
        {
            PlayerWallet.TryPurchase(_cartCostDelta);
            MerchantWallet.AddMoney(_cartCostDelta);
        }
        else
        {
            MerchantWallet.TryPurchase(_cartCostForMerchant);
            PlayerWallet.AddMoney(_cartCostForMerchant);
        }

        ReassignItemsToNewOwners();

        OnTradeSuccess?.Invoke();
    }

    private void ReassignItemsToNewOwners()
    {
        foreach(SingleCell cell in cells)
        {
            if (cell.isEmpty) continue;
            SingleItem item = cell.CurrentItem;

            Location newOwner = calculateNewOwner(item);

            cell.RemoveItem();
            item.ChangeOwner(newOwner);
            Mover.LerpItemToParent(item, newOwner);
        }
    }

    private Location calculateNewOwner(SingleItem item)
    {
        return 1 - item.Owner;
    }
}
