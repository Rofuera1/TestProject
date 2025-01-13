using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    private static InputHandler Instance;
    public EventSystem EventSystem;
    public UnityEngine.UI.GraphicRaycaster Raycaster;

    private Location ownerOfItemCurrentlyDragging;

    private SingleItem itemCurrentlyDragging;
    private SingleCell cellCurrentlyDragging;

    private void Awake()
    {
        Instance = this;
    }

    public static void OnSingleCellPressed(SingleCell cell)
    {
        Instance.onSingleCellPressed(cell);
    }

    private void onSingleCellPressed(SingleCell cell)
    {
        if (cell.isEmpty)
            return;

        cellCurrentlyDragging = cell;
        itemCurrentlyDragging = cell.CurrentItem;

        ownerOfItemCurrentlyDragging = itemCurrentlyDragging.Owner;

        StartCoroutine(draggingItemCoroutine());
    }

    private IEnumerator draggingItemCoroutine()
    {
        Vector3 ref_vector = Vector3.zero;
        Vector3 targetPosition;

        Mover.SetObjectToBufferParent(itemCurrentlyDragging.transform);

        while(Input.GetKey(KeyCode.Mouse0))
        {
            yield return null;

            targetPosition = Vector3.SmoothDamp(itemCurrentlyDragging.transform.position, Input.mousePosition, ref ref_vector, 0.1f);
            itemCurrentlyDragging.transform.position = targetPosition;
        }

        onStoppedDragging();
    }

    private void onStoppedDragging()
    {
        PointerEventData PointerEventData = new PointerEventData(EventSystem);
        PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        Raycaster.Raycast(PointerEventData, results);

        sortItemMovementAfterDragStop(results);
    }

    private void sortItemMovementAfterDragStop(List<RaycastResult> results)
    {
        bool didDoSomething = false;
        removeItemFromCurrentPlace();

        foreach (RaycastResult res in results)
        {
            // It's slow, but it's the easiest to write ;)
            // Please hire me

            SingleCell cell = res.gameObject.GetComponent<SingleCell>();
            if (!cell || !cell.isEmpty || !isMoveAllowed(cell))
                continue;

            repositionItemToNewPlace(cell);

            didDoSomething = true;
            break;
        }

        if (!didDoSomething)
            Mover.LerpItemToParent(itemCurrentlyDragging, ownerOfItemCurrentlyDragging);
    }

    private bool isMoveAllowed(SingleCell newPlace)
    {
        if (newPlace.Location == Location.Trading)
            return true;
        else if (ownerOfItemCurrentlyDragging != newPlace.Location)
            return false;

        return true;
    }

    private void removeItemFromCurrentPlace()
    {
        cellCurrentlyDragging.RemoveItem();
    }

    private void repositionItemToNewPlace(SingleCell cell)
    {
        cellCurrentlyDragging = cell;
        cellCurrentlyDragging.ReceiveItem(itemCurrentlyDragging);
        itemCurrentlyDragging.ChangeLocation(cell.Location);

        Mover.LerpItemToCell(itemCurrentlyDragging, cell);
    }
}
