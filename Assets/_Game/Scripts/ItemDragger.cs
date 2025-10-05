using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum StorageAreaType
{
    Shelf,
    Cooler
}
public class ItemDragger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask itemsLayer;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private Transform dragTransform;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private float nameTextYOffset = 2;
    [SerializeField] private float nameTextFadeDuration = 0.1f;
    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.5f;

    private Camera cam;
    private DropZone currentZone;
    private IDraggable draggedItem;
    private Plane dragPlane;
    private Vector3 offset;
    private Vector3 dragStartPos;
    private Vector3 dragPos;
    private bool isMoving = false;
    private IEnumerator itemMoveEnumerator;
    private IEnumerator nameTextFadeEnum;
    private int activeFingerId = -1;

    private UnityEvent<DropZone> onZoneEntered = new UnityEvent<DropZone>();
    private UnityEvent<DropZone> onZoneExited = new UnityEvent<DropZone>();
    private UnityEvent<IDraggable> onItemDragStarted = new UnityEvent<IDraggable>();
    private UnityEvent<IDraggable, DropZone> onItemDragEnded = new UnityEvent<IDraggable, DropZone>();

    public UnityEvent<IDraggable> OnItemDragStarted => onItemDragStarted;
    public UnityEvent<IDraggable, DropZone> OnItemDragEnded => onItemDragEnded;
    public UnityEvent<DropZone> OnZoneEntered => onZoneEntered;
    public UnityEvent<DropZone> OnZoneExited => onZoneExited;

    void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleDragStart();

        if (draggedItem != null && Input.GetMouseButton(0))
            HandleDragging();

        if (Input.GetMouseButtonUp(0) && draggedItem != null)
            HandleDragRelease();
    }
    private void HandleDragStart()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, itemsLayer))
        {
            if (hit.transform.TryGetComponent<IDraggable>(out var draggable))
            {
                draggedItem = draggable;
                draggable.OnDragStart();

                dragPlane = new Plane(draggedItem.transform.right, draggable.transform.position);
                dragPlane.Raycast(ray, out float distance);

                offset = draggable.transform.position - ray.GetPoint(distance);
                OnDragStart(draggable);
            }
        }
    }
    private void HandleDragging()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float distance))
        {
            dragPos = ray.GetPoint(distance) + offset;
            dragTransform.position = dragPos;

            nameText.transform.position = dragPos + new Vector3(0, nameTextYOffset, 0);

            draggedItem.OnDrag(dragPos);

            if (currentZone == null && !isMoving)
                draggedItem.transform.position = ray.GetPoint(distance) + offset;
        }

        //Check drop zones
        if (Physics.Raycast(ray, out RaycastHit zoneHit, float.MaxValue, dropZoneLayer))
        {
            if (zoneHit.transform.TryGetComponent<DropZone>(out var hitZone))
            {
                if (currentZone != hitZone)
                {
                    if (currentZone != null)
                        OnZoneExit(currentZone);

                    OnZoneEnter(hitZone);
                }
            }
        }
        else
        {
            if (currentZone != null)
                OnZoneExit(currentZone);
        }
    }
    private void HandleDragRelease()
    {
        draggedItem.OnDragEnd();

        OnDragEnd(draggedItem, currentZone);

        draggedItem = null;
    }
    private void OnDragStart(IDraggable item)
    {
        DraggableItem draggableItem = item as DraggableItem;

        if (draggableItem?.ItemData != null)
        {
            nameText.text = draggableItem.ItemData.LocalizedName.GetLocalizedString();
            FadeNameText(true);
        }

        item.Collider.enabled = false;
        onItemDragStarted?.Invoke(item);
    }
    private void OnDragEnd(IDraggable item, DropZone zone)
    {
        FadeNameText(false);

        item.Collider.enabled = true;
        onItemDragEnded?.Invoke(item, zone);
    }
    private void OnZoneEnter(DropZone zone)
    {
        if (zone.HasItem)
            return;

        currentZone = zone;
        onZoneEntered?.Invoke(currentZone);

        if (itemMoveEnumerator != null)
            StopCoroutine(itemMoveEnumerator);

        itemMoveEnumerator = MoveTo(draggedItem.transform, draggedItem.transform.position, zone.DropPoint, moveDuration);
        StartCoroutine(itemMoveEnumerator);
    }
    private void OnZoneExit(DropZone zone)
    {
        if (itemMoveEnumerator != null)
            StopCoroutine(itemMoveEnumerator);

        onZoneExited?.Invoke(currentZone);

        itemMoveEnumerator = MoveTo(draggedItem.transform, draggedItem.transform.position, dragTransform, moveDuration);
        StartCoroutine(itemMoveEnumerator);

        currentZone = null;
    }
    public IEnumerator MoveTo(Transform target, Vector3 start, Transform endTransform, float duration)
    {
        float time = 0f;
        isMoving = true;

        while (time < duration)
        {
            float t = time / duration;
            target.position = Vector3.Lerp(start, endTransform.position, t);

            time += Time.deltaTime;
            yield return null;
        }

        target.position = endTransform.position;
        isMoving = false;
    }
    private void FadeNameText(bool fadeIn = true)
    {
        if (nameTextFadeEnum != null)
            StopCoroutine(nameTextFadeEnum);

        nameTextFadeEnum = FadeTextAlpha(nameText.color.a, fadeIn ? 1 : 0, nameTextFadeDuration);
        StartCoroutine(nameTextFadeEnum);
    }
    private IEnumerator FadeTextAlpha(float startAlpha, float endAlpha, float duration)
    {
        if (nameText == null)
            yield break;

        Color color = nameText.color;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            nameText.color = new Color(color.r, color.g, color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        nameText.color = new Color(color.r, color.g, color.b, endAlpha);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 pointOnPlane = dragPlane.normal * -dragPlane.distance;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, dragPlane.normal);

        Gizmos.matrix = Matrix4x4.TRS(pointOnPlane, rotation, new Vector3(10, 10, 0.01f));
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}