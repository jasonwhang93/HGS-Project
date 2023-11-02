using UnityEngine;

public enum QuestStatus
{
    NotAccepted,
    Accepted,
    InProgress,
    Completed
}

[System.Serializable]
public class Quest
{
    // Quest의 기본 정보
    public int questID;
    public string questTitle;

    // 퀘스트의 보상
    public int rewardMoney;

    // 퀘스트 상태
    public QuestStatus status
    {
        get
        {
            return PlayerData.GetQuestStatus(questID);
        }
        set
        {
            PlayerData.SetQuestStatus(questID, value);
        }
    }

    public void NotAcceptQuest()
    {
        if (status == QuestStatus.NotAccepted)
        {
            status = QuestStatus.NotAccepted;

            // 상태 변경 후 저장
            PlayerData.SaveQuest(this);
        }
    }

    // 퀘스트 상태 변경을 위한 액션 메서드들
    public void AcceptQuest()
    {
        if (status == QuestStatus.NotAccepted)
        {
            status = QuestStatus.Accepted;

            // 상태 변경 후 저장
            PlayerData.SaveQuest(this);
        }
    }

    public void StartQuest()
    {
        if (status == QuestStatus.Accepted)
        {
            status = QuestStatus.InProgress;

            // 상태 변경 후 저장
            PlayerData.SaveQuest(this);
        }
    }

    public void CompleteQuest()
    {
        if (status == QuestStatus.InProgress)
        {
            status = QuestStatus.Completed;

            // 상태 변경 후 저장
            PlayerData.SaveQuest(this);
        }
    }

    public bool IsQuestCompleted()
    {
        return status == QuestStatus.Completed;
    }
}
