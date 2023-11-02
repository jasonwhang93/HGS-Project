using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    public GameObject acceptableQuestUI;
    public GameObject inProgressOrCompleteQuestUI;
    public GameObject level0neQuestDescriptDialogUI;
    public GameObject levelTwoQuestDescriptDialogUI;

    public Transform acceptableQuestsList;
    public Transform inProgressQuestsList;
    public Transform completeQuestsList;

    private List<QuestContent> allQuestContents = new List<QuestContent>();

    public void ShowAcceptableQuestsUI()
    {
        acceptableQuestUI.SetActive(true);
        inProgressOrCompleteQuestUI.SetActive(false);
        level0neQuestDescriptDialogUI.SetActive(false);
        levelTwoQuestDescriptDialogUI.SetActive(false);
    }

    public void ShowInProgressOrCompleteQuestsUI()
    {
        acceptableQuestUI.SetActive(false);
        inProgressOrCompleteQuestUI.SetActive(true);
        level0neQuestDescriptDialogUI.SetActive(false);
        levelTwoQuestDescriptDialogUI.SetActive(false);
    }

    public void ShowLevel0neQuestDescriptDialogUI()
    {
        acceptableQuestUI.SetActive(false);
        inProgressOrCompleteQuestUI.SetActive(false);
        level0neQuestDescriptDialogUI.SetActive(true);
        levelTwoQuestDescriptDialogUI.SetActive(false);
    }

    public void ShowLevelTwoQuestDescriptDialogUI()
    {
        acceptableQuestUI.SetActive(false);
        inProgressOrCompleteQuestUI.SetActive(false);
        level0neQuestDescriptDialogUI.SetActive(false);
        levelTwoQuestDescriptDialogUI.SetActive(true);
    }

    public void CloseAllWindows()
    {
        // 각 UI 창을 닫는 로직을 여기에 추가합니다.
        // 예를 들면, 만약 각 UI 창이 GameObject 형태로 저장되어 있다면:
        acceptableQuestUI.SetActive(false);
        inProgressOrCompleteQuestUI.SetActive(false);
        level0neQuestDescriptDialogUI.SetActive(false);
        levelTwoQuestDescriptDialogUI.SetActive(false);
    }

    public QuestContent AddQuestToAcceptableList(QuestContent questContentInstance)
    {
        questContentInstance.transform.SetParent(acceptableQuestsList);
        allQuestContents.Add(questContentInstance);
        return questContentInstance;
    }

    public QuestContent AddQuestToInProgressList(QuestContent questContentInstance)
    {
        questContentInstance.transform.SetParent(inProgressQuestsList);
        allQuestContents.Add(questContentInstance);
        return questContentInstance;
    }

    public QuestContent AddQuestToCompleteList(QuestContent questContentInstance)
    {
        questContentInstance.transform.SetParent(completeQuestsList);
        allQuestContents.Add(questContentInstance);
        return questContentInstance;
    }

    public void UpdateQuestList()
    {
        foreach (QuestContent questContent in allQuestContents)
        {
            Quest quest = questContent.GetQuest();

            // 퀘스트 상태를 PlayerData에 저장
            PlayerData.SaveQuest(quest);

            switch (quest.status)
            {
                case QuestStatus.NotAccepted:
                    questContent.transform.SetParent(acceptableQuestsList);
                    break;
                case QuestStatus.Accepted:
                    questContent.transform.SetParent(inProgressQuestsList);
                    quest.StartQuest();  // Accepted 상태의 퀘스트를 InProgress로 변경
                    break;
                case QuestStatus.InProgress:
                    questContent.transform.SetParent(inProgressQuestsList);
                    break;
                case QuestStatus.Completed:
                    questContent.transform.SetParent(completeQuestsList);
                    break;
            }
        }
    }

    public void DestroyQuestItems()
    {
        foreach (Transform child in acceptableQuestsList)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in inProgressQuestsList)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in completeQuestsList)
        {
            Destroy(child.gameObject);
        }

        allQuestContents.Clear();
    }
}
