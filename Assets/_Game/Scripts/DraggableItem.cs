using UnityEngine;
using UnityEngine.Events;

public class DraggableItem : MonoBehaviour, IDraggable
{
    [SerializeField] private Collider col;
    [SerializeField] private Transform visuals;

    private ItemDataSO itemData;
    private UnityEvent onDragStarted = new UnityEvent();
    private UnityEvent onDragEnded = new UnityEvent();
    private bool isDragging = false;
    private bool isPlaced = false;

    public ItemDataSO ItemData => itemData;
    public Collider Collider => col;
    public UnityEvent OnDragStarted => onDragStarted;
    public UnityEvent OnDragEnded => onDragEnded;
    public bool IsDragging => isDragging;
    public bool IsPlaced
    {
        get { return isPlaced; }
        set { isPlaced = value; }
    }

    public void Initialize(ItemDataSO data)
    {
        itemData = data;
        gameObject.name = data.DisplayName;
    }
    public void OnDragStart()
    {
        isDragging = true;
        onDragStarted?.Invoke();
    }
    public void OnDragEnd()
    {
        isDragging = false;
        onDragEnded?.Invoke();
    }
    public void OnDrag(Vector3 position){}
}
