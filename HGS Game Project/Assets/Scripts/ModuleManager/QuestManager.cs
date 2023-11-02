using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestUIController questUIController;  // QuestUIController 참조

    public QuestContent levelOneQuestContentPrefab;  // LevelOne QuestContent 프리팹
    public QuestContent levelTwoQuestContentPrefab;  // LevelTwo QuestContent 프리팹

    private int selectedQuestID = 0;

    private void Start()
    {
        // 저장된 퀘스트 데이터를 가져옵니다.
        Quest savedQuest1 = PlayerData.GetSavedQuest(1);
        Quest savedQuest2 = PlayerData.GetSavedQuest(2);

        if (savedQuest1 == null)
            CreateQuestInstance(levelOneQuestContentPrefab.GetQuest());
        else
            CreateQuestInstance(savedQuest1); // 수정된 부분

        if (savedQuest2 == null)
            CreateQuestInstance(levelTwoQuestContentPrefab.GetQuest());
        else
            CreateQuestInstance(savedQuest2); // 수정된 부분

        EventManager.instance.AddEvent("Quest :: UIToggle", (p) =>
        {
            ToggleQuestUI();
        });

        // TODO: 각 QuestContent의 인스턴스에 콜백을 설정하도록 변경
        // levelOneQuestContentPrefab.onQuestClickedCallback = OnQuestContentClicked;
        // levelTwoQuestContentPrefab.onQuestClickedCallback = OnQuestContentClicked;
    }

    private void Update()
    {
        CheckQuestIsCompleted();
    }

    private void OnQuestContentClicked(QuestContent clickedContent)
    {
        Debug.Log($"Clicked on quest: {clickedContent.GetQuest().questID}");

        // 필요한 추가 처리를 여기에 넣습니다.
        selectedQuestID = clickedContent.GetQuest().questID;
    }

    // 모든 창을 닫는 함수를 호출하는 함수
    public void OnCancelDialogButtonClicked()
    {
        questUIController.CloseAllWindows();
        selectedQuestID = 0;
    }

    public void OnNextButtonClicked()
    {
        if (selectedQuestID == 1)
        {
            questUIController.ShowLevel0neQuestDescriptDialogUI();
        }
        else if (selectedQuestID == 2)
        {
            questUIController.ShowLevelTwoQuestDescriptDialogUI();
        }
        else
        {
            // 아무런 반응도 하지 않는다.
        }
    }

    public void OnAccecptButtonClicked()
    {
        // 퀘스트를 가져오기 위한 참조 변수
        Quest selectedQuest = null;

        // 선택된 퀘스트ID에 따라 해당 퀘스트 객체를 가져옵니다.
        if (selectedQuestID == 1)
        {
            selectedQuest = levelOneQuestContentPrefab.GetQuest();
        }
        else if (selectedQuestID == 2)
        {
            selectedQuest = levelTwoQuestContentPrefab.GetQuest();
        }

        // 선택된 퀘스트가 있을 경우, 그 퀘스트의 상태를 '수락'으로 변경합니다.
        if (selectedQuest != null)
        {
            selectedQuest.AcceptQuest();
            OnCancelDialogButtonClicked();

            // 퀘스트 리스트 UI를 업데이트합니다.
            questUIController.UpdateQuestList();
        }
    }

    public void OnPrevButtonClicked()
    {
        if (levelOneQuestContentPrefab.GetQuest().status == QuestStatus.NotAccepted ||
            levelTwoQuestContentPrefab.GetQuest().status == QuestStatus.NotAccepted)
        {
            questUIController.ShowAcceptableQuestsUI();
        }
    }

    public void OnCompleteButtonClick()
    {
        // 모든 퀘스트를 순회
        Quest[] allQuests = { levelOneQuestContentPrefab.GetQuest(), levelTwoQuestContentPrefab.GetQuest() };
        foreach (Quest quest in allQuests)
        {
            // 퀘스트 상태가 Completed이면
            if (quest.status == QuestStatus.Completed)
            {
                // 플레이어에게 보상 지급
                PlayerData.playerMoney += quest.rewardMoney;

                // 퀘스트 상태를 NotAccepted로 변경
                quest.status = QuestStatus.NotAccepted;
            }
        }

        // UI 갱신
        questUIController.UpdateQuestList();
        OnCancelDialogButtonClicked();
    }

    public void OnRejectButtonClick()
    {
        // 퀘스트를 가져오기 위한 참조 변수
        Quest selectedQuest = null;

        // 선택된 퀘스트ID에 따라 해당 퀘스트 객체를 가져옵니다.
        if (selectedQuestID == 1)
        {
            selectedQuest = levelOneQuestContentPrefab.GetQuest();
        }
        else if (selectedQuestID == 2)
        {
            selectedQuest = levelTwoQuestContentPrefab.GetQuest();
        }

        // 선택된 퀘스트가 있을 경우, 그 퀘스트의 상태를 '수락'으로 변경합니다.
        if (selectedQuest != null)
        {
            selectedQuest.NotAcceptQuest();

            OnCancelDialogButtonClicked();

            // 퀘스트 리스트 UI를 업데이트합니다.
            questUIController.UpdateQuestList();
        }
    }

    public void CheckQuestIsCompleted()
    {
        if (PlayerData.isMap1Cleared && levelOneQuestContentPrefab.GetQuest().status == QuestStatus.InProgress)
        {
            Debug.Log("1");
            levelOneQuestContentPrefab.GetQuest().CompleteQuest();
            questUIController.UpdateQuestList();
            PlayerData.isMap1Cleared = false; // 맵1 클리어 상태 초기화
        }

        if (PlayerData.isMap2Cleared && levelTwoQuestContentPrefab.GetQuest().status == QuestStatus.InProgress)
        {
            Debug.Log("2");
            levelTwoQuestContentPrefab.GetQuest().CompleteQuest();
            questUIController.UpdateQuestList();
            PlayerData.isMap2Cleared = false; // 맵2 클리어 상태 초기화
        }
    }

    public void CreateQuestInstance(Quest quest)
    {
        QuestContent questContentPrefab;

        // 퀘스트 ID에 따라 적절한 QuestContent 프리팹 선택
        if (quest.questID == 1)
        {
            questContentPrefab = levelOneQuestContentPrefab;
        }
        else
        {
            questContentPrefab = levelTwoQuestContentPrefab;
        }

        QuestContent questContentInstance = Instantiate(questContentPrefab);

        // 인스턴스에 콜백 설정
        questContentInstance.onQuestClickedCallback = OnQuestContentClicked;

        switch (quest.status)
        {
            case QuestStatus.NotAccepted:
                questUIController.AddQuestToAcceptableList(questContentInstance);
                break;
            case QuestStatus.Accepted:
            case QuestStatus.InProgress:
                questUIController.AddQuestToInProgressList(questContentInstance);
                break;
            case QuestStatus.Completed:
                questUIController.AddQuestToCompleteList(questContentInstance);
                break;
        }

        PlayerData.SaveQuest(quest);
    }

    // QuestUIController의 창들을 토글하는 함수
    public void ToggleQuestUI(object param = null)
    {
        bool hasNotAcceptedQuests = false;
        bool hasAcceptedOrInProgressQuests = false;

        Quest[] allQuests = { levelOneQuestContentPrefab.GetQuest(), levelTwoQuestContentPrefab.GetQuest() };

        // Check the status of all quests
        foreach (Quest quest in allQuests)
        {
            if (quest.status == QuestStatus.NotAccepted)
            {
                hasNotAcceptedQuests = true;
            }
            else if (quest.status == QuestStatus.Accepted || quest.status == QuestStatus.InProgress || quest.status == QuestStatus.Completed)
            {
                hasAcceptedOrInProgressQuests = true;
            }
        }

        // Use QuestUIController methods based on quest status
        if (hasAcceptedOrInProgressQuests)
        {
            questUIController.ShowInProgressOrCompleteQuestsUI();
        }
        else if (hasNotAcceptedQuests)
        {
            questUIController.ShowAcceptableQuestsUI();
        }
        else
        {
            // Optionally, handle the case where all quests are completed or no quests are available
        }
    }
}
