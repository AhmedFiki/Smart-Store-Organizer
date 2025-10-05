using UnityEngine;
public interface IDraggable
{
    Transform transform { get; }
    Collider Collider { get; }
    bool IsDragging { get; }

    void OnDragStart();
    void OnDrag(Vector3 position);
    void OnDragEnd();
}