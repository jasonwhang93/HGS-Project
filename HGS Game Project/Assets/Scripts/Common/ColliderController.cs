using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColliderController : MonoBehaviour
{
    public TilemapController tilemapController; // TilemapController의 참조가 필요합니다.

    // 플레이어가 사다리나 밧줄을 사용하여 특정 플랫폼으로 이동할 때 콜라이더 무시
    public void IgnorePlatformCollider(Collider2D playerCollider, Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
    }

    // 플레이어가 플랫폼에 착지한 후 콜라이더를 다시 활성화
    public void ResetPlatformCollider(Collider2D playerCollider, Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    // 플레이어가 특정 위치에 있을 때 콜라이더 무시
    public void IgnoreSideColliders(Collider2D playerCollider)
    {
        // 여기서는 Floor2 Side의 콜라이더를 무시합니다.
        Collider2D[] sideColliders = tilemapController.GetSideColliders(); // GetSideColliders는 구현해야 할 메소드입니다.

        foreach (var collider in sideColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, true);
        }
    }

    // 플레이어가 특정 타일맵의 특정 영역 내에 있을 경우 주어진 콜라이더를 무시하는 메소드
    public void IgnoreColliderIfInArea(Collider2D playerCollider, Vector3 playerWorldPosition, Tilemap someTilemap, Vector3Int areaCellPosition, 
        Collider2D colliderToIgnore)
    {
        // playerWorldPosition을 Tilemap의 셀 좌표로 변환
        Vector3Int playerCellPosition = someTilemap.WorldToCell(playerWorldPosition);

        // 플레이어가 지정된 영역 내에 있는지 확인
        if (playerCellPosition == areaCellPosition)
        {
            Physics2D.IgnoreCollision(playerCollider, colliderToIgnore, true);
        }
    }


    // 특정 콜라이더를 무시하는 메소드
    public void IgnoreCollider(Collider2D playerCol, Collider2D colliderToIgnore)
    {
        Physics2D.IgnoreCollision(playerCol, colliderToIgnore, true);
        tilemapController.AddIgnoredCollider(colliderToIgnore); // 무시된 콜라이더 추가
    }

    // 여러 콜라이더를 무시하는 메소드
    public void IgnoreColliders(Collider2D playerCol, Collider2D[] collidersToIgnore)
    {
        foreach (var collider in collidersToIgnore)
        {
            IgnoreCollider(playerCol, collider);
        }
    }

    // 무시된 콜라이더를 다시 활성화하고 리스트에서 제거하는 메소드
    public void ResetIgnoredCollider(Collider2D playerCol, Collider2D colliderToReset)
    {
        List<Collider2D> ignoredColliders = tilemapController.GetIgnoredColliders();

        // 무시된 콜라이더 목록에서 특정 콜라이더를 찾아 해제합니다.
        if (ignoredColliders.Contains(colliderToReset))
        {
            Physics2D.IgnoreCollision(playerCol, colliderToReset, false);
            tilemapController.RemoveIgnoredCollider(colliderToReset);
        }
    }

    // 사다리나 밧줄 등 특정 레이어를 향해 아래로 이동할 때 해당 레이어의 콜라이더를 무시하는 메소드
    public void IgnoreColliderOnDownward(Collider2D playerCol, string targetLayer)
    {
        int layerMask = 1 << LayerMask.NameToLayer(targetLayer);

        playerCol.enabled = false;

        Vector2 rayStart = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.5f, layerMask);

        playerCol.enabled = true;

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            IgnoreCollider(playerCol, hit.collider);
        }
    }

    // 플레이어가 점프 중일 때 특정 콜라이더를 무시하는 메소드
    public void IgnoreColliderDuringJump(Collider2D playerCollider, Collider2D[] collidersToIgnore)
    {
        foreach (var collider in collidersToIgnore)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, true);
        }
    }

    // 플레이어가 착지 후 특정 콜라이더 무시 해제 메소드
    public void ResetMultipleIgnoredColliders(Collider2D playerCollider, Collider2D[] collidersToReset)
    {
        foreach (var collider in collidersToReset)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, false);
        }
    }

    // 현재 무시되고 있는 콜라이더들을 출력하는 메소드
    public void PrintIgnoredColliders()
    {
        Debug.Log("Print");

        foreach (var collider in tilemapController.GetIgnoredColliders())
        {
            Debug.Log("Ignored Collider: " + collider.name);
        }
    }
}
