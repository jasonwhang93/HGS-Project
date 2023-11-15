using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public LayerMask RopeLayer;
    public LayerMask LadderLayer;
    public LayerMask UpGroundLayer;
    public LayerMask GroundLayer;
    private Collider2D ignoredCollider = null; // 무시된 collider를 저장할 변수

    public Collider2D floor2Side;
    public TilemapCollider2D floor2TopTilemapCol;
    public CompositeCollider2D floor2TopCompositeCol;
    public TilemapCollider2D floor2StairTopTilemapCol;
    public CompositeCollider2D floor2StairTopCompositeCol;

    public Tilemap floor2StairTopTilemap;
    public Vector3Int tilePosition; // 수정하고자 하는 타일의 위치

    private Tilemap ropeTilemap;
    private Tilemap ladderTilemap;

    private List<Collider2D> ignoredColliders = new List<Collider2D>();

    void Awake()
    {
        ropeTilemap = FindDeepChild(this.transform, "Rope").GetComponent<Tilemap>();
        ladderTilemap = FindDeepChild(this.transform, "Ladder").GetComponent<Tilemap>();
    }

    // 재귀적으로 자식 오브젝트를 검색하여 특정 이름을 가진 오브젝트를 찾는 함수
    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            Transform found = FindDeepChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }

    // 겹쳐진 영역인지 확인
    public bool IsOverlappingArea(Vector3Int tilePos)
    {
        // 플레이어의 현재 위치를 월드 좌표로 변환
        Vector3 playerWorldPos = floor2StairTopTilemap.CellToWorld(tilePos);

        // Floor2 Side 콜라이더 영역을 확인
        if (floor2Side.OverlapPoint(playerWorldPos))
        {
            // Floor2 TopCompositeCol 콜라이더 영역을 확인
            if (floor2TopCompositeCol.OverlapPoint(playerWorldPos))
            {
                // 두 영역이 겹침
                return true;
            }
        }

        // 겹치지 않음
        return false;
    }

    public Vector3Int GetCurrentTilePosition(Transform player)
    {
        // 플레이어의 위치를 월드 좌표에서 셀 좌표로 변환
        Vector3 worldPosition = player.transform.position;
        Vector3Int cellPosition = floor2StairTopTilemap.WorldToCell(worldPosition);

        return cellPosition;
    }

    public Tilemap GetRopeTilemap()
    {
        return ropeTilemap;
    }

    public Tilemap GetLadderTilemap()
    {
        return ladderTilemap;
    }

    // 이 메서드는 특정 collider를 무시합니다.
    public void IgnoreCollider(Collider2D playerCol, Collider2D colliderToIgnore)
    {
        if (!ignoredColliders.Contains(colliderToIgnore))
        {
            ignoredColliders.Add(colliderToIgnore);
            Physics2D.IgnoreCollision(playerCol, colliderToIgnore, true);
        }
    }

    public void IgnoreColliders(Collider2D playerCol, Collider2D[] collidersToIgnore)
    {
        foreach (var collider in collidersToIgnore)
        {
            IgnoreCollider(playerCol, collider);
        }
    }

    public void ResetIgnoredCollider(Collider2D playerCol)
    {
        foreach (var ignoredCollider in ignoredColliders)
        {
            Physics2D.IgnoreCollision(playerCol, ignoredCollider, false);
        }
        ignoredColliders.Clear();
    }

    public void ResetIgnoredColliderAtSpecificTile(Collider2D playerCol, Vector3Int tilePos)
    {
        if (tilePos == new Vector3Int(11, -8, 0) || tilePos == new Vector3Int(10, -8, 0) || 
            tilePos == new Vector3Int(-3, -8, 0) || tilePos == new Vector3Int(-4, -8, 0))
        {
            ResetIgnoredCollider(playerCol);
        }
    }

    public void IgnoreColliderOnDownward(Collider2D playerCol, string targetLayer)
    {
        // LayerMask 설정
        int layerMask = 1 << LayerMask.NameToLayer(targetLayer);

        // Player의 Collider를 일시적으로 비활성화
        playerCol.enabled = false;

        // PlayerController의 하단에서 아래로 Raycast 발사 (UpGround 레이어만 대상으로 함)
        Vector2 rayStart = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.5f, layerMask);

        // Player의 Collider를 다시 활성화
        playerCol.enabled = true;

        // UpGround 레이어에 해당하는 콜라이더를 찾았을 때만 처리
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            Debug.Log("Raycast Hit: " + hit.collider.name);
            IgnoreCollider(playerCol, hit.collider);
        }
    }

    public bool IsGrounded(Collider2D playerCol, ref bool wasGrounded)
    {
        int combinedLayerMask = GroundLayer | UpGroundLayer;
        float extraHeight = 0.05f;
        Vector2 boxSize = new Vector2(playerCol.bounds.size.x * 0.9f, extraHeight); // 콜라이더의 가로 길이의 일부와 추가 높이 사용
        Vector2 boxCenter = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y - extraHeight / 2); // 콜라이더 하단 중심 위치

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, combinedLayerMask);

        wasGrounded = hits.Length > 0;
        return hits.Length > 0;
    }

    public bool HasTileBelow(Collider2D playerCol, Tilemap tilemap)
    {
        Vector3 footPosition = playerCol.bounds.center + new Vector3(0, -playerCol.bounds.extents.y - 0.1f, 0);  // Just below the feet
        Vector3Int cellPosition = tilemap.WorldToCell(footPosition);
        return tilemap.HasTile(cellPosition);
    }

    public bool HasTileAbove(Collider2D playerCol, Tilemap tilemap)
    {
        Vector3 headPosition = playerCol.bounds.center + new Vector3(0, playerCol.bounds.extents.y + 0.1f, 0);  // Just above the head
        Vector3Int cellPosition = tilemap.WorldToCell(headPosition);
        return tilemap.HasTile(cellPosition);
    }

    public void CheckRopeInteraction(Collider2D playerCol, ref bool isOnRope, ref bool isRope, ref Vector3 ropeTileCenter)
    {
        CheckTileInteraction(playerCol, ref isOnRope, ref isRope, ref ropeTileCenter, RopeLayer, -0.2f);
    }

    public void CheckLadderInteraction(Collider2D playerCol, ref bool isOnLadder, ref bool isLadder, ref Vector3 ladderTileCenter)
    {
        CheckTileInteraction(playerCol, ref isOnLadder, ref isLadder, ref ladderTileCenter, LadderLayer, 0);
    }

    private void CheckTileInteraction(Collider2D playerCol, ref bool isOnTile, ref bool isTile, ref Vector3 tileCenter, LayerMask tileLayer, float offset)
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCol.bounds.center, playerCol.bounds.size, 0f, Vector2.up, 0.02f, tileLayer);

        if (hit.collider != null)
        {
            isOnTile = true;
            var tilemap = hit.collider.GetComponent<Tilemap>();
            Vector3Int cellPosition = tilemap.WorldToCell(hit.point);
            Vector3 actualTileCenter = tilemap.GetCellCenterWorld(cellPosition);
            tileCenter = actualTileCenter + new Vector3(offset, 0, 0);
        }
        else
        {
            isOnTile = false;
            isTile = false;
        }
    }
}
