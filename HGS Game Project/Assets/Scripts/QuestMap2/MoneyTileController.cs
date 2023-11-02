using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MoneyTileController : MonoBehaviour
{
    public Text coinText;
    private int coin = 0;

    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어의 머리 위 위치를 기준으로 cellPosition을 계산합니다.
            Vector3 headPosition = collision.bounds.center + new Vector3(0, collision.bounds.extents.y, 0);
            Vector3Int cellPosition = tilemap.WorldToCell(headPosition);

            // 플레이어의 발 아래 위치를 기준으로 cellPosition을 계산합니다.
            Vector3 footosition = collision.bounds.center - new Vector3(0, collision.bounds.extents.y, 0);
            Vector3Int cellPosition1 = tilemap.WorldToCell(footosition);

            // 플레이어의 오른쪽면의 오른쪽 위치를 기준으로 cellPosition을 계산합니다.
            Vector3 rightSideosition = collision.bounds.center + new Vector3(collision.bounds.extents.x, 0, 0);
            Vector3Int cellPosition2 = tilemap.WorldToCell(rightSideosition);

            // 플레이어의 왼쪽면의 왼쪽 위치를 기준으로 cellPosition을 계산합니다.
            Vector3 leftSideosition = collision.bounds.center - new Vector3(collision.bounds.extents.x, 0, 0);
            Vector3Int cellPosition3 = tilemap.WorldToCell(leftSideosition);

            if(tilemap.GetTile(cellPosition) != null)
            {
                tilemap.SetTile(cellPosition, null);

                // 플레이어의 점수를 1점 올립니다.
                coin++;
                coinText.text = coin.ToString();
            }
            if (tilemap.GetTile(cellPosition1) != null)
            {
                tilemap.SetTile(cellPosition1, null);

                // 플레이어의 점수를 1점 올립니다.
                coin++;
                coinText.text = coin.ToString();
            }
            if (tilemap.GetTile(cellPosition2) != null)
            {
                tilemap.SetTile(cellPosition2, null);

                // 플레이어의 점수를 1점 올립니다.
                coin++;
                coinText.text = coin.ToString();
            }
            if (tilemap.GetTile(cellPosition3) != null)
            {
                tilemap.SetTile(cellPosition3, null);

                // 플레이어의 점수를 1점 올립니다.
                coin++;
                coinText.text = coin.ToString();
            }
        }
    }
}
