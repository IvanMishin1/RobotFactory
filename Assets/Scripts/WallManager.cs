using UnityEngine;
using UnityEngine.Tilemaps;

public class WallManager : MonoBehaviour
{
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase wallTile;

    private RectInt _wallRect;
    public RectInt wallRect
    {
        get => _wallRect;
        set
        {
            if (_wallRect == value) return;
            _wallRect = value;
            DrawOutline();
        }
    }

    private void DrawOutline()
    {
        if (wallTilemap == null || wallTile == null) return;
        wallTilemap.ClearAllTiles();

        int xMin = _wallRect.xMin;
        int xMax = _wallRect.xMin + _wallRect.width - 1;
        int yMin = _wallRect.yMin;
        int yMax = _wallRect.yMin + _wallRect.height - 1;

        for (int x = xMin; x <= xMax; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, yMin, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(x, yMax, 0), wallTile);
        }
        for (int y = yMin + 1; y < yMax; y++)
        {
            wallTilemap.SetTile(new Vector3Int(xMin, y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(xMax, y, 0), wallTile);
        }
    }
}