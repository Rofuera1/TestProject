using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private static Mover Instance;

    public OwnerManager PlayerPlace;
    public OwnerManager MerchantPlace;
    public Transform BufferForDragging;

    private void Awake()
    {
        Instance = this;
    }

    public static void SetObjectToBufferParent(Transform obj)
    {
        obj.parent = Instance.BufferForDragging;
    }

    public static void LerpItemToParent(SingleItem item, Location owner)
    {
        Instance.lerpItemToParent(item, owner);
    }

    public static void LerpItemToCell(SingleItem item, SingleCell cell)
    {
        Instance.lerpItemToCell(item, cell);
    }

    private void lerpItemToParent(SingleItem item, Location owner)
    {
        SingleCell newParent = getParentEmptyCell(owner);

        StartCoroutine(lerpItemToLocation(item, newParent));

        newParent.ReceiveItem(item);
        item.ChangeLocation(owner);
    }

    private void lerpItemToCell(SingleItem item, SingleCell cell)
    {
        StartCoroutine(lerpItemToLocation(item, cell));
    }

    private IEnumerator lerpItemToLocation(SingleItem item, SingleCell cell)
    {
        SetObjectToBufferParent(item.transform);

        float TIME_TO_LERP = 0.2f;
        StartCoroutine(Coroutines.LerpGameObjectFromToPosition(item.transform, cell.transform.position, TIME_TO_LERP, EasingFunction.EaseOutCirc));

        yield return new WaitForSeconds(TIME_TO_LERP);

        item.transform.parent = cell.transform;
    }

    private static SingleCell getParentEmptyCell(Location parentLocation)
    {
        SingleCell cellResult = null;
        if (parentLocation == Location.Player)
            cellResult = Instance.PlayerPlace.FindEmptyCell();
        else if (parentLocation == Location.Merchant)
            cellResult = Instance.MerchantPlace.FindEmptyCell();

        return cellResult;
    }
}
