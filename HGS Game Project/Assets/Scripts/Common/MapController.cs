using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public AudioClip backgroundMusicClip; // 배경 음악 클립
    public bool isGameEnd;
    public string sceneName;
    public int questID;

    // Start is called before the first frame update
    void Start()
    {
        // AudioManager 인스턴스가 있는지 확인
        if (AudioManager.instance != null)
        {
            // 배경 음악 재생
            AudioManager.instance.PlayMusic(backgroundMusicClip);
        }
        else
        {
            Debug.LogWarning("AudioManager not found.");
        }
    }

    private void Update()
    {
        // 게임이 끝났을 때의 처리
        if (isGameEnd)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        // 퀘스트의 상태를 클리어로 변경
        PlayerData.isMap1Cleared = true;

        isGameEnd = false;

        // MainVillage 씬 로드
        SceneManager.LoadScene("MainVillage");
    }
}
