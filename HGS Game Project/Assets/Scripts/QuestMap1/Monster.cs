using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 몬스터와 플레이어 간의 충돌 처리
        if (collision.gameObject.CompareTag("Player"))
        {
            // MonsterSpawner의 ReducePlayerLife 메서드를 호출
            MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>();
            if (spawner != null)
            {
                spawner.ReducePlayerLife();
            }

            // 몬스터를 파괴합니다.
            Destroy(gameObject);
        }
    }
}
