using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public AudioClip backgroundMusicClip; // ��� ���� Ŭ��
    public bool isMapClearFailed = false;

    // Start is called before the first frame update
    void Start()
    {
        // AudioManager �ν��Ͻ��� �ִ��� Ȯ��
        if (AudioManager.instance != null)
        {
            // ��� ���� ���
            AudioManager.instance.PlayMusic(backgroundMusicClip);
        }
        else
        {
            Debug.LogWarning("AudioManager not found.");
        }
    }

    private void Update()
    {
        // ������ ������ ���� ó��
        if (PlayerData.isMap1Cleared && !isMapClearFailed || PlayerData.isMap2Cleared && !isMapClearFailed)
        {
            SceneManager.LoadScene("MainVillage");
        }
        if(isMapClearFailed)
        {
            SceneManager.LoadScene("MainVillage");
        }
    }
}
