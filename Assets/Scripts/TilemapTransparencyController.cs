using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTransparencyController : MonoBehaviour
{
    private Tilemap tilemap;
    private float opaqueAlpha = 1f;
    private float transparentAlpha = 0.25f;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTilemapTransparency(transparentAlpha);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTilemapTransparency(opaqueAlpha);
        }
    }

    void SetTilemapTransparency(float alpha)
    {
        Color color = tilemap.color;
        color.a = alpha;
        tilemap.color = color;
    }
}
