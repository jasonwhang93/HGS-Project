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

    // ��� ��ȣ�ۿ뿡 �ʿ��� �ݶ��̴� ����
    public TilemapCollider2D floor1SideTopCol;

    public TilemapCollider2D floor2StairTopTilemapCol;

    public TilemapCollider2D floor3TopTilemapCol;
    public CompositeCollider2D floor3TopCompositeCol;

    private List<Collider2D> ignoredColliders = new List<Collider2D>();

    // ��������� �ڽ� ������Ʈ�� �˻��Ͽ� Ư�� �̸��� ���� ������Ʈ�� ã�� �Լ�
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

        // �ʱ�ȭ
        groundLayerInfo = -1;

        if (wasGrounded && hits.Length > 0)
        {
            // ù ��° �浹�� �ݶ��̴��� ���̾� ������ ��ȯ
            groundLayerInfo = hits[0].gameObject.layer;

            // �浹�� ���̾� ������ ����� �α׷� ���
            //Debug.Log("Grounded on Layer: " + LayerMask.LayerToName(groundLayerInfo));
        }

        return hits.Length > 0;
    }


    public void CheckAndUpdateConnectInteraction(Collider2D playerCol, Tilemap connectTilemap)
    {
        // ���� �÷��̾� ��ġ�� ������� ���� �Ǵ� ��ٸ� Ÿ���� �� ��ǥ�� ����ϴ�.
        Vector3 playerPosition = playerCol.transform.position;
        Vector3Int cellPosition = connectTilemap.WorldToCell(playerPosition);

        // �ش� ���� ���� �Ǵ� ��ٸ� Ÿ���� �ִ��� Ȯ���մϴ�.
        if (connectTilemap.HasTile(cellPosition))
        {
            // Ÿ���� �߽� ���� ��ǥ�� ����մϴ�.
            Vector3 tileCenter = connectTilemap.GetCellCenterWorld(cellPosition);

            // �÷��̾��� x��ǥ�� Ÿ���� �߽� x��ǥ�� �����մϴ�.
            playerCol.transform.position = new Vector3(tileCenter.x, playerCol.transform.position.y, playerCol.transform.position.z);
        }
    }

    public bool HasTileBelow(Collider2D playerCol, Tilemap tilemap)
    {
        // �÷��̾� �ݶ��̴��� �ϴ� �߽� ��ġ �Ʒ��� ��ġ�ϴ� ����Ʈ�� ����մϴ�.
        // �� ����Ʈ�� �÷��̾��� �� �ٷ� �Ʒ��� ��ġ�ϰ� �˴ϴ�.
        Vector3 footPosition = playerCol.bounds.center - new Vector3(0, playerCol.bounds.extents.y + 0.1f, 0);

        // �� ����Ʈ�� Ÿ�ϸ��� �� ��ǥ�� ��ȯ�մϴ�.
        Vector3Int cellPosition = tilemap.WorldToCell(footPosition);

        // �ش� �� ��ǥ�� Ÿ���� �����ϴ��� Ȯ���մϴ�.
        return tilemap.HasTile(cellPosition);
    }


    public bool HasTileAbove(Collider2D playerCol, Tilemap tilemap)
    {
        // �÷��̾� �ݶ��̴��� ��� �߽� ��ġ ���� ��ġ�ϴ� ����Ʈ�� ����մϴ�.
        // �� ����Ʈ�� �÷��̾��� �Ӹ� �ٷ� ���� ��ġ�ϰ� �˴ϴ�.
        Vector3 headPosition = playerCol.bounds.center + new Vector3(0, playerCol.bounds.extents.y + 0.1f, 0);

        // �� ����Ʈ�� Ÿ�ϸ��� �� ��ǥ�� ��ȯ�մϴ�.
        Vector3Int cellPosition = tilemap.WorldToCell(headPosition);

        // �ش� �� ��ǥ�� Ÿ���� �����ϴ��� Ȯ���մϴ�.
        return tilemap.HasTile(cellPosition);
    }

    // Ư�� ������ �ݶ��̴��� ��ȯ�ϴ� �Լ�
    public Collider2D[] GetSideColliders()
    {
        // ���⼭�� ���÷� floor2Side�� ��ȯ�մϴ�.
        // ���� ���������� �ʿ��� �ݶ��̴����� �迭�� ��ȯ�մϴ�.
        return new Collider2D[] { floor1SideTopCol };
    }

    // Ư�� Ÿ���� �ִ��� Ȯ���ϴ� �Լ�
    public bool IsSpecificTile(Tilemap tilemap, Vector3Int tilePos)
    {
        // �־��� Ÿ�ϸʿ��� Ư�� �� ��ǥ�� Ÿ���� �����ϴ��� Ȯ���մϴ�.
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
        // ���⿡ Ʈ���� �̸��� ���� ���� ���� �ݶ��̴��� ��ȯ�ϴ� ������ �����ϼ���.
        // ��: "Floor2 TriggerStairTop" Ʈ���ſ� �����ϴ� "Floor2 StairTop" ���� �ݶ��̴� ��
        switch (stairTriggerName)
        {
            case "Floor2 TriggerStairTop":
                return floor2StairTopTilemapCol;
            // �߰����� ��ܿ� ���� ������ �ʿ��� ��� ���⿡ �߰�
            default:
                return null;
        }
    }

    public bool IsPlayerOnFloor1Top(Transform playerPos, float raycastDistance)
    {
        // �÷��̾��� ��ġ���� �ٷ� �Ʒ��� Raycast�� �߻�
        RaycastHit2D hit = Physics2D.Raycast(playerPos.position, Vector2.down, raycastDistance, GroundLayer);

        if (hit.collider != null)
        {
            // Raycast�� GroundLayer�� ��Ұ�, �ش� ������Ʈ�� �̸��� "Floor1 Top"���� Ȯ��
            return (hit.collider.gameObject.name == "Floor1 Top" || hit.collider.gameObject.name == "Floor1 SideTop");
        }

        return false;
    }

    public bool IsPlayerOnFloor2Top(Transform playerPos, float raycastDistance)
    {
        // �÷��̾��� ��ġ���� �ٷ� �Ʒ��� Raycast�� �߻�
        RaycastHit2D hit = Physics2D.Raycast(playerPos.position, Vector2.down, raycastDistance, UpGroundLayer);

        if (hit.collider != null)
        {
            // Raycast�� GroundLayer�� ��Ұ�, �ش� ������Ʈ�� �̸��� "Floor1 Top"���� Ȯ��
            return (hit.collider.gameObject.name == "Floor2 TriggerStairTop");
        }

        return false;
    }

    public void PrintIgnoredColliders()
    {
        var ignoredColliders = GetIgnoredColliders();
        if (ignoredColliders.Count == 0)
        {
            Debug.Log("���õ� �ݶ��̴��� �����ϴ�.");
        }
        else
        {
            foreach (var collider in ignoredColliders)
            {
                if (collider != null)
                {
                    Debug.Log("���õ� �ݶ��̴�: " + collider.GetComponent<Collider2D>().name);
                }
            }
        }
    }
}
