using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerManager : MonoBehaviour
{
    public Location Location;
    [SerializeField]
    protected SingleCell[] cells;

    protected List<SingleItem> ownedItems = new List<SingleItem>();

    private void Start()
    {
        foreach (SingleCell cell in cells)
            cell.Init(this, null);
    }

    public virtual void Init(SingleItem[] Items)
    {
        for(int i = 0; i < Items.Length; i++)
        {
            cells[i].Init(this, Items[i]);
        }
    }

    public virtual void AddItem(SingleItem item)
    {
        ownedItems.Add(item);
    }

    public virtual void RemoveItem(SingleItem item)
    {
        if (!ownedItems.Remove(item))
            Debug.Log("Didnt remove shit");
    }

    public virtual SingleCell FindEmptyCell()
    {
        foreach (SingleCell cell in cells)
            if (cell.isEmpty)
                return cell;
        Debug.LogWarning("No empty cell found");
        return null;
    }

    public SingleItem[] ReturnOwnedItems() => ownedItems.ToArray();
}
