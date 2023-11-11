using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject spawnArea; // 몬스터 스폰 영역 (MonsterSpawnBox 오브젝트)
    private Transform parentSpawnTransform;
    private Camera mainCamera;

    public float spawnRate = 2f; // 몬스터 스폰 간격
    public float nextSpawnTime = 1f; // 다음 몬스터 스폰 시간

    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 생성된 몬스터 목록

    public Text currentScoreText; // 점수 텍스트 UI
    public Text maxScoreText;
    public Image heart1, heart2, heart3; // 남은 목숨을 나타내는 하트 이미지 UI

    private bool isGameEnded = false;

    public MapController mapController;

    private void Start()
    {
        parentSpawnTransform = spawnArea.GetComponent<Transform>();
        mainCamera = Camera.main;

        Debug.Log("최고 점수: " + PlayerData.playerMaxScore);

        InitGame();

        // 초기 UI 설정
        UpdateUI();
    }

    private void Update()
    {
        if (!isGameEnded && Time.time >= nextSpawnTime)
        {
            int numberOfMonstersToSpawn = Random.Range(1, 3);

            for (int i = 0; i < numberOfMonstersToSpawn; i++)
            {
                SpawnMonster();
            }

            nextSpawnTime = Time.time + 1f / spawnRate;
        }

        // 플레이어의 남은 목숨 확인
        if (!isGameEnded)
        {
            CheckPlayerRemainingHearts();
        }

        // UI를 업데이트하는 함수 호출
        UpdateUI();
    }

    private void InitGame()
    {
        isGameEnded = false;
        PlayerData.playerCurrentScore = 0;
        PlayerData.playerRemainHeart = 3;
    }


    private void SpawnMonster()
    {
        GameObject[] monsterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Monster");
        int randomIndex = Random.Range(0, monsterPrefabs.Length);
        GameObject monsterPrefab = monsterPrefabs[randomIndex];

        Vector3 spawnPosition = GetRandomSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            GameObject newObject = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            newObject.transform.SetParent(parentSpawnTransform);

            spawnedMonsters.Add(newObject);

            // 몬스터를 카메라 아래 범위를 벗어나면 파괴하도록 설정
            StartCoroutine(DestroyWhenOutOfCameraBounds(newObject));
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnArea.GetComponent<Collider2D>().bounds.min.x, spawnArea.GetComponent<Collider2D>().bounds.max.x),
                spawnArea.GetComponent<Collider2D>().bounds.max.y,
                0f
            );

            bool overlap = false;

            // 스폰된 몬스터들과 충돌 검사
            foreach (GameObject monster in spawnedMonsters)
            {
                // Null 체크를 추가합니다.
                if (monster != null)
                {
                    Collider2D monsterCollider = monster.GetComponent<Collider2D>();

                    if (monsterCollider != null)
                    {
                        if (monsterCollider.bounds.Contains(spawnPosition))
                        {
                            overlap = true;
                            break;
                        }
                    }
                }
            }

            if (!overlap)
            {
                return spawnPosition;
            }

            attempts++;
        }

        return Vector3.zero;
    }

    private IEnumerator DestroyWhenOutOfCameraBounds(GameObject obj)
    {
        while (true)
        {
            // obj가 null이 아니고 아직 파괴되지 않았을 때
            if (obj != null && obj.transform.position.y < mainCamera.transform.position.y - mainCamera.orthographicSize)
            {
                spawnedMonsters.Remove(obj);
                Destroy(obj);
                PlayerData.playerCurrentScore++;
                yield break;
            }
            yield return null;
        }
    }

    public void ReducePlayerLife()
    {
        PlayerData.playerRemainHeart--;
        UpdateUI();
        CheckPlayerRemainingHearts();
    }


    private void CheckPlayerRemainingHearts()
    {
        if (PlayerData.playerRemainHeart <= 0)
        {
            isGameEnded = true;
            EndGame();
        }
    }


    private void EndGame()
    {
        // 게임 종료 처리
        // 스폰된 몬스터들을 모두 파괴
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }

        // 스폰된 몬스터 목록 초기화
        spawnedMonsters.Clear();

        if (PlayerData.playerMaxScore <= PlayerData.playerCurrentScore)
        {
            PlayerData.playerMaxScore = PlayerData.playerCurrentScore;
        }

        // AudioManager 인스턴스가 있는지 확인 후 음악 정지
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }

        PlayerData.isMap1Cleared = true;
    }

    // UI 업데이트 함수
    private void UpdateUI()
    {
        // 점수 표시
        currentScoreText.text = PlayerData.playerCurrentScore.ToString();
        maxScoreText.text = PlayerData.playerMaxScore.ToString();

        // 남은 목숨 표시 (하트 이미지 활성화/비활성화)
        heart1.enabled = PlayerData.playerRemainHeart >= 1;
        heart2.enabled = PlayerData.playerRemainHeart >= 2;
        heart3.enabled = PlayerData.playerRemainHeart >= 3;
    }
}
