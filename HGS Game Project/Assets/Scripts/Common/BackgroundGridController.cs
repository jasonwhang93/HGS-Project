using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundGridController : MonoBehaviour
{
    private float limitMinX, limitMaxX; // ��輱 ��ġ ����
    private Tilemap boundaryTilemap;

    // Start is called before the first frame update
    void Start()
    {
        boundaryTilemap = GetComponent<Tilemap>();
        CalculateWorldBoundary();
    }

    private void CalculateWorldBoundary()
    {
        if (boundaryTilemap != null)
        {
            // Local bounds ���
            Bounds localBounds = boundaryTilemap.localBounds;
            Vector3Int minCell = Vector3Int.FloorToInt(localBounds.min);
            Vector3Int maxCell = Vector3Int.CeilToInt(localBounds.max);

            // World bounds ���
            Vector3 minWorld = boundaryTilemap.CellToWorld(minCell);
            Vector3 maxWorld = boundaryTilemap.CellToWorld(maxCell);

            limitMinX = minWorld.x;
            limitMaxX = maxWorld.x;

            // UniversalCameraFollow ������Ʈ ã�� �� ī�޶� ��� ����
            UniversalCameraFollow cameraFollow = Camera.main.GetComponent<UniversalCameraFollow>();

            // ī�޶� ��� ����
            if (cameraFollow != null)
            {
                cameraFollow.SetCameraLimits(limitMinX, limitMaxX, minWorld.y, maxWorld.y);
            }
        }
    }
}