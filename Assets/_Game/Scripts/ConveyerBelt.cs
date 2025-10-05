using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Renderer beltRenderer;
    [SerializeField] private float scrollSpeed = 0.1f;

    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;

    [SerializeField] private bool Active = true;

    private Material mat;

    public Transform StartTransform => startTransform;
    public Transform EndTransform => endTransform;
    public bool IsActive => Active;

    void Awake()
    {
        mat = beltRenderer.material;
    }
    void Update()
    {
        if (!Active) return;

        Vector2 offset = mat.mainTextureOffset;
        offset.y += scrollSpeed * Time.deltaTime;
        offset.y %= 1;
        mat.mainTextureOffset = offset;
    }
}
