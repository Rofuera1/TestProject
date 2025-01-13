using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleCell : MonoBehaviour, IPointerDownHandler
{
    public Location Location;
    public SingleItem CurrentItem {get; private set;}
    public bool isEmpty => CurrentItem == null;

    private OwnerManager owner;

    public void Init(OwnerManager Owner, SingleItem item)
    {
        owner = Owner;
        Location = owner.Location;

        if (item)
        {
            ReceiveItem(item);
            CurrentItem.ChangeOwner(Location);
            CurrentItem.ChangeLocation(Location);

            item.transform.parent = transform;
            item.transform.localPosition = Vector3.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputHandler.OnSingleCellPressed(this);
    }

    public void ReceiveItem(SingleItem item)
    {
        if (CurrentItem)
            Debug.LogError("Trying to assign item at cell that is already filled");
         
        CurrentItem = item;

        owner.AddItem(item);
    }

    public void RemoveItem()
    {
        owner.RemoveItem(CurrentItem);

        CurrentItem = null;
    }
}
