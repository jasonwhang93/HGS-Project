using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    // GameData�� �ʵ带 ����� �̵�
    public static bool isPause = false;

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ��ü�� �ı����� �ʵ��� ����

            // PlayerData �ʱ�ȭ
            PlayerData.ResetData();
        }
        else
        {
            Destroy(gameObject); // �̹� GameController �ν��Ͻ��� �����ϸ� ���� ������ �ν��Ͻ� �ı�
        }
    }
}
