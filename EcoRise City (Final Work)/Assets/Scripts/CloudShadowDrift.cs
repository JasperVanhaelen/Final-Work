using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CloudShadowDrift : MonoBehaviour
{
    [Tooltip("World units per second (X to scroll sideways, Y for slight vertical drift).")]
    public Vector2 speed = new Vector2(-0.1f, 0.0f);

    [Range(0f, 1f)]
    public float alpha = 0.12f;

    [Tooltip("If true, auto-sizes the tiled sprite to be larger than the camera view so edges never show.")]
    public bool autoSizeToCamera = true;

    [Tooltip("How much larger than the camera view to make the tile (only if autoSizeToCamera).")]
    public float oversizeFactor = 3f;

    SpriteRenderer sr;
    Camera cam;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0f, 0f, 0f, alpha);

        if (autoSizeToCamera)
        {
            cam = Camera.main;
            ResizeToCoverCamera();
        }
    }

    void Update()
    {
        // Slow, constant drift
        transform.position += (Vector3)(speed * Time.deltaTime);
        // No wrap needed if your tile is huge; it will never expose edges.
    }

    void OnValidate()
    {
        if (sr != null)
            sr.color = new Color(0f, 0f, 0f, alpha);
    }

    void ResizeToCoverCamera()
    {
        if (cam == null) return;

        // Works with an orthographic camera (typical for city builders)
        float height = 2f * cam.orthographicSize;
        float width  = height * cam.aspect;

        Vector2 targetSize = new Vector2(width, height) * oversizeFactor;

        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = targetSize;
        // Make sure your texture's Wrap Mode is Repeat
    }
}