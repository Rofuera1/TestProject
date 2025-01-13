using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public ItemCreator Creator;
    [Space]
    public OwnerManager PlayerManager;
    public OwnerManager MerchantManager;
    [Space]
    public Wallet PlayerWallet;
    public Wallet MerchantWallet;
    [Space]
    public OwnerInfo BasicPlayer;
    public OwnerInfo BasicMerhcant;

    private const string PLAYER_SAVE_FILE = "Player.info";
    private const string MERCHANT_SAVE_FILE = "Merchant.info";

    private void Start()
    {
        loadFiles();
    }

    private void OnApplicationQuit()
    {
        saveFiles();
    }

    private void loadFiles()
    {
        OwnerInfo playerInfo = Saver.LoadFile<OwnerInfo>(Application.persistentDataPath + "/" + PLAYER_SAVE_FILE);
        OwnerInfo traderInfo = Saver.LoadFile<OwnerInfo>(Application.persistentDataPath + "/" + MERCHANT_SAVE_FILE);

        if (playerInfo == null) playerInfo = BasicPlayer;
        if (traderInfo == null) traderInfo = BasicMerhcant;

        PlayerManager.Init(Creator.ReturnPrefabByID(playerInfo.Items));
        MerchantManager.Init(Creator.ReturnPrefabByID(traderInfo.Items));

        PlayerWallet.Init(playerInfo.Money);
        MerchantWallet.Init(traderInfo.Money);
    }

    private void saveFiles()
    {
        int playerWalletAmount = PlayerWallet.Balance;
        int merchantWalletAmount = MerchantWallet.Balance;

        ItemInfo[] playerItems = itemsToInfoConvert(PlayerManager.ReturnOwnedItems());
        ItemInfo[] merchantItems = itemsToInfoConvert(MerchantManager.ReturnOwnedItems());

        Saver.SaveFile(new OwnerInfo(playerItems, playerWalletAmount), Application.persistentDataPath + "/" + PLAYER_SAVE_FILE);
        Saver.SaveFile(new OwnerInfo(merchantItems, merchantWalletAmount), Application.persistentDataPath + "/" + MERCHANT_SAVE_FILE);
    }

    private ItemInfo[] itemsToInfoConvert(SingleItem[] items)
    {
        ItemInfo[] result = new ItemInfo[items.Length];

        for (int i = 0; i < result.Length; i++)
            result[i] = new ItemInfo(items[i].ID);

        return result;
    }
}

[System.Serializable]
public class OwnerInfo
{
    [SerializeField]
    public ItemInfo[] Items;
    [SerializeField]
    public int Money;

    public OwnerInfo(ItemInfo[] items, int money)
    {
        Items = items;
        Money = money;
    }
}

[System.Serializable]
public class ItemInfo
{
    [SerializeField]
    public int ID;

    public ItemInfo(int iD)
    {
        ID = iD;
    }
}
