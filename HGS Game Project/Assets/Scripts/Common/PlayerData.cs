using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static int playerMoney = 10000;
    public static int playerCurrentScore = 0;
    public static int playerMaxScore = 0;
    public static int playerRemainHeart = 3;
    public static int playerEarnCoin = 0;

    // 퀘스트 상태를 관리하기 위한 딕셔너리
    private static Dictionary<int, QuestStatus> questStatuses = new Dictionary<int, QuestStatus>();

    public static bool isMap1Cleared = false;
    public static bool isMap2Cleared = false;

    private static Dictionary<int, Quest> savedQuests = new Dictionary<int, Quest>();

    // 모든 데이터를 초기 상태로 리셋하는 함수
    public static void ResetData()
    {
        playerMoney = 10000;
        playerCurrentScore = 0;
        playerMaxScore = 0;
        playerRemainHeart = 3;
        questStatuses.Clear();
        isMap1Cleared = false;
        isMap2Cleared = false;
    }

    public static void PrintData()
    {
        Debug.Log("playerMoney: " + playerMoney);
        Debug.Log("playerMaxScore: " + playerMaxScore);
        Debug.Log("playerEarnCoin: " + playerEarnCoin);
        Debug.Log("isMap1Cleared: " + isMap1Cleared);
        Debug.Log("isMap2Cleared: " + isMap2Cleared);

        Debug.Log("===== Quest Statuses =====");
        foreach (var kvp in questStatuses)
        {
            Debug.Log($"Quest ID: {kvp.Key}, Status: {kvp.Value}");
        }
    }


    public static void SaveQuest(Quest quest)
    {
        if (!savedQuests.ContainsKey(quest.questID))
        {
            savedQuests.Add(quest.questID, quest);
        }
        else
        {
            savedQuests[quest.questID] = quest;
        }
    }

    public static Quest GetSavedQuest(int questID)
    {
        if (savedQuests.TryGetValue(questID, out Quest storedQuest))
        {
            return storedQuest;
        }
        return null; // 없으면 null 반환
    }

    // 퀘스트 상태 설정 함수
    public static void SetQuestStatus(int questID, QuestStatus status)
    {
        if (questStatuses.ContainsKey(questID))
        {
            questStatuses[questID] = status;
        }
        else
        {
            questStatuses.Add(questID, status);
        }
    }

    // 퀘스트 상태 가져오기 함수
    public static QuestStatus GetQuestStatus(int questID)
    {
        if (questStatuses.ContainsKey(questID))
        {
            return questStatuses[questID];
        }
        return QuestStatus.NotAccepted; // 기본값. 원하는 값으로 변경 가능
    }
}
