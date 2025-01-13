using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Location
{
    Player = 0,
    Merchant = 1,
    Trading = 2
}

public class SingleItem : MonoBehaviour
{
    public Location CurrentlyAt { get; private set; }
    public Location Owner { get; private set; }
    public int CostFromPlayer { get; private set; }
    public int CostFromMerchant { get; private set; }
    public int ID { get; private set; }

    public void Init(int costFromPlayer, int costFromMerchant, int id)
    {
        CostFromPlayer = costFromPlayer;
        CostFromMerchant = costFromMerchant;
        ID = id;
    }

    public void ChangeOwner(Location cell)
    {
        if (cell == Location.Trading)
            Debug.LogWarning("Trying to set trading as owner");
        Owner = cell;
    }

    public void ChangeLocation(Location location)
    {
        CurrentlyAt = location;
    }
}
