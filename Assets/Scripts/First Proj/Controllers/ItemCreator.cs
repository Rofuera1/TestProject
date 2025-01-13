using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreator : MonoBehaviour
{
    public ItemParams[] ItemInfos;
    public SingleItem ItemPrefab;
    protected static Dictionary<int, ItemParams> itemsInfosDictionary;

    private void Awake()
    {
        loadDictionary();
    }

    private void loadDictionary()
    {
        itemsInfosDictionary = new Dictionary<int, ItemParams>();

        foreach (ItemParams item in ItemInfos)
            if (!itemsInfosDictionary.ContainsKey(item.ID))
                itemsInfosDictionary.Add(item.ID, item);
    }

    public SingleItem[] ReturnPrefabByID(ItemInfo[] infos)
    {
        SingleItem[] items = new SingleItem[infos.Length];
        for (int i = 0; i < items.Length; i++)
            items[i] = ReturnPrefabByID(infos[i]);
        return items;
    }

    public SingleItem ReturnPrefabByID(ItemInfo info)
    {
        if (!itemsInfosDictionary.ContainsKey(info.ID))
        {
            Debug.LogError("Non-existing ID: " + info.ID);
            return ItemPrefab;
        }
        ItemParams param = itemsInfosDictionary[info.ID];

        SingleItem item = Instantiate(ItemPrefab); // Лучше было бы пул сделать, но тут и так всего намешано ради маленького тестового, так что просто знайте - я не тупой, а просто стараюсь быстрее, и так уже затянул((
        item.GetComponent<UnityEngine.UI.Image>().sprite = param.Sprite;
        item.Init(param.CostFromPlayer, param.CostFromMerchant, info.ID);

        return item;
    }
}
