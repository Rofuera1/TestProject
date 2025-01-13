using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    private int balance = 0;
    public int Balance { get => balance; set { balance = value; OnBalanceChanged?.Invoke(value); } }

    public Action<int> OnBalanceChanged;
    public Location Location;

    public void Init(int startBalance)
    {
        Balance = startBalance;
    }

    public bool IsPurchasePossible(int cost)
    {
        return cost <= balance;
    }

    public bool TryPurchase(int cost)
    {
        if (cost > balance || cost < 0)
            return false;

        Balance -= cost;

        return true;
    }

    public void AddMoney(int value)
    {
        if (value < 0)
            return;

        Balance += value;
    }
}
