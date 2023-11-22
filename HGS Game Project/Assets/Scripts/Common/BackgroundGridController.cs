using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundGridController : MonoBehaviour
{
    private float limitMinX, limitMaxX; // 경계선 위치 변수
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
            // Local bounds 계산
            Bounds localBounds = boundaryTilemap.localBounds;
            Vector3Int minCell = Vector3Int.FloorToInt(localBounds.min);
            Vector3Int maxCell = Vector3Int.CeilToInt(localBounds.max);

            // World bounds 계산
            Vector3 minWorld = boundaryTilemap.CellToWorld(minCell);
            Vector3 maxWorld = boundaryTilemap.CellToWorld(maxCell);

            limitMinX = minWorld.x;
            limitMaxX = maxWorld.x;

            // UniversalCameraFollow 컴포넌트 찾기 및 카메라 경계 설정
            UniversalCameraFollow cameraFollow = Camera.main.GetComponent<UniversalCameraFollow>();

            // 카메라 경계 설정
            if (cameraFollow != null)
            {
                cameraFollow.SetCameraLimits(limitMinX, limitMaxX, minWorld.y, maxWorld.y);
            }
        }
    }
}