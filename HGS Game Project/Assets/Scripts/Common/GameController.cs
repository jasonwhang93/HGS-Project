using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    // GameData의 필드를 여기로 이동
    public static bool isPause = false;

    private void Awake()
    {
        // 싱글턴 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 객체가 파괴되지 않도록 설정

            // PlayerData 초기화
            PlayerData.ResetData();
        }
        else
        {
            Destroy(gameObject); // 이미 GameController 인스턴스가 존재하면 새로 생성된 인스턴스 파괴
        }
    }
}
