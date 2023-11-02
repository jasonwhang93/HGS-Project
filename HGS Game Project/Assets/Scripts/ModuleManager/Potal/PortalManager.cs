using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    public string connectSceneName; // 전환될 씬의 이름
    public int requiredQuestID; // 씬 전환에 필요한 퀘스트 ID

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (PlayerData.GetQuestStatus(requiredQuestID) == QuestStatus.InProgress)
                {
                    SceneManager.LoadScene(connectSceneName);
                }
                else
                {
                    Debug.Log("입장키를 위한 퀘스트를 받지 않은 상태입니다.");
                }
            }
        }
    }
}
