using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public BackgroundGridController boundaryGrid;

    public LayerMask RopeLayer;
    public LayerMask LadderLayer;
    public LayerMask UpGroundLayer;
    public LayerMask GroundLayer;

    public Tilemap ropeTilemap;
    public Tilemap ladderTilemap;

    // 계단 상호작용에 필요한 콜라이더 참조
    public TilemapCollider2D floor1SideTopCol;

    public TilemapCollider2D floor2StairTopTilemapCol;

    public TilemapCollider2D floor3TopTilemapCol;
    public CompositeCollider2D floor3TopCompositeCol;

    private List<Collider2D> ignoredColliders = new List<Collider2D>();

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

    public bool IsGrounded(Collider2D playerCol, ref bool wasGrounded, out int groundLayerInfo)
    {
        int combinedLayerMask = GroundLayer | UpGroundLayer;
        float extraHeight = 0.05f;
        Vector2 boxSize = new Vector2(playerCol.bounds.size.x * 0.9f, extraHeight);
        Vector2 boxCenter = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y - extraHeight / 2);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, combinedLayerMask);
        wasGrounded = hits.Length > 0;

        // 초기화
        groundLayerInfo = -1;

        if (wasGrounded && hits.Length > 0)
        {
            // 첫 번째 충돌한 콜라이더의 레이어 정보를 반환
            groundLayerInfo = hits[0].gameObject.layer;

            // 충돌한 레이어 정보를 디버그 로그로 출력
            //Debug.Log("Grounded on Layer: " + LayerMask.LayerToName(groundLayerInfo));
        }

        return hits.Length > 0;
    }


    public void CheckAndUpdateConnectInteraction(Collider2D playerCol, Tilemap connectTilemap)
    {
        // 현재 플레이어 위치를 기반으로 밧줄 또는 사다리 타일의 셀 좌표를 얻습니다.
        Vector3 playerPosition = playerCol.transform.position;
        Vector3Int cellPosition = connectTilemap.WorldToCell(playerPosition);

        // 해당 셀에 밧줄 또는 사다리 타일이 있는지 확인합니다.
        if (connectTilemap.HasTile(cellPosition))
        {
            // 타일의 중심 월드 좌표를 계산합니다.
            Vector3 tileCenter = connectTilemap.GetCellCenterWorld(cellPosition);

            // 플레이어의 x좌표를 타일의 중심 x좌표로 설정합니다.
            playerCol.transform.position = new Vector3(tileCenter.x, playerCol.transform.position.y, playerCol.transform.position.z);
        }
    }

    public bool HasTileBelow(Collider2D playerCol, Tilemap tilemap)
    {
        // 플레이어 콜라이더의 하단 중심 위치 아래에 위치하는 포인트를 계산합니다.
        // 이 포인트는 플레이어의 발 바로 아래에 위치하게 됩니다.
        Vector3 footPosition = playerCol.bounds.center - new Vector3(0, playerCol.bounds.extents.y + 0.1f, 0);

        // 이 포인트를 타일맵의 셀 좌표로 변환합니다.
        Vector3Int cellPosition = tilemap.WorldToCell(footPosition);

        // 해당 셀 좌표에 타일이 존재하는지 확인합니다.
        return tilemap.HasTile(cellPosition);
    }


    public bool HasTileAbove(Collider2D playerCol, Tilemap tilemap)
    {
        // 플레이어 콜라이더의 상단 중심 위치 위에 위치하는 포인트를 계산합니다.
        // 이 포인트는 플레이어의 머리 바로 위에 위치하게 됩니다.
        Vector3 headPosition = playerCol.bounds.center + new Vector3(0, playerCol.bounds.extents.y + 0.1f, 0);

        // 이 포인트를 타일맵의 셀 좌표로 변환합니다.
        Vector3Int cellPosition = tilemap.WorldToCell(headPosition);

        // 해당 셀 좌표에 타일이 존재하는지 확인합니다.
        return tilemap.HasTile(cellPosition);
    }

    // 특정 영역의 콜라이더를 반환하는 함수
    public Collider2D[] GetSideColliders()
    {
        // 여기서는 예시로 floor2Side를 반환합니다.
        // 실제 구현에서는 필요한 콜라이더들을 배열로 반환합니다.
        return new Collider2D[] { floor1SideTopCol };
    }

    // 특정 타일이 있는지 확인하는 함수
    public bool IsSpecificTile(Tilemap tilemap, Vector3Int tilePos)
    {
        // 주어진 타일맵에서 특정 셀 좌표에 타일이 존재하는지 확인합니다.
        return tilemap.HasTile(tilePos);
    }

    public void AddIgnoredCollider(Collider2D collider)
    {
        if (!ignoredColliders.Contains(collider))
        {
            ignoredColliders.Add(collider);
        }
    }

    public void RemoveIgnoredCollider(Collider2D collider)
    {
        if (ignoredColliders.Contains(collider))
        {
            ignoredColliders.Remove(collider);
        }
    }

    public List<Collider2D> GetIgnoredColliders()
    {
        return ignoredColliders;
    }

    public void ClearIgnoredColliders()
    {
        ignoredColliders.Clear();
    }

    public Collider2D GetPhysicalColliderForStair(string stairTriggerName)
    {
        // 여기에 트리거 이름에 따라 실제 물리 콜라이더를 반환하는 로직을 구현하세요.
        // 예: "Floor2 TriggerStairTop" 트리거에 대응하는 "Floor2 StairTop" 물리 콜라이더 등
        switch (stairTriggerName)
        {
            case "Floor2 TriggerStairTop":
                return floor2StairTopTilemapCol;
            // 추가적인 계단에 대한 매핑이 필요한 경우 여기에 추가
            default:
                return null;
        }
    }

    public bool IsPlayerOnFloor1Top(Transform playerPos, float raycastDistance)
    {
        // 플레이어의 위치에서 바로 아래로 Raycast를 발사
        RaycastHit2D hit = Physics2D.Raycast(playerPos.position, Vector2.down, raycastDistance, GroundLayer);

        if (hit.collider != null)
        {
            // Raycast가 GroundLayer에 닿았고, 해당 오브젝트의 이름이 "Floor1 Top"인지 확인
            return (hit.collider.gameObject.name == "Floor1 Top" || hit.collider.gameObject.name == "Floor1 SideTop");
        }

        return false;
    }

    public bool IsPlayerOnFloor2Top(Transform playerPos, float raycastDistance)
    {
        // 플레이어의 위치에서 바로 아래로 Raycast를 발사
        RaycastHit2D hit = Physics2D.Raycast(playerPos.position, Vector2.down, raycastDistance, UpGroundLayer);

        if (hit.collider != null)
        {
            // Raycast가 GroundLayer에 닿았고, 해당 오브젝트의 이름이 "Floor1 Top"인지 확인
            return (hit.collider.gameObject.name == "Floor2 TriggerStairTop");
        }

        return false;
    }

    public void PrintIgnoredColliders()
    {
        var ignoredColliders = GetIgnoredColliders();
        if (ignoredColliders.Count == 0)
        {
            Debug.Log("무시된 콜라이더가 없습니다.");
        }
        else
        {
            foreach (var collider in ignoredColliders)
            {
                if (collider != null)
                {
                    Debug.Log("무시된 콜라이더: " + collider.GetComponent<Collider2D>().name);
                }
            }
        }
    }
}
