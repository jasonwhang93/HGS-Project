using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuestContent : MonoBehaviour
{
    // 입력받을 Quest 정보
    [Header("Quest Info")]
    [SerializeField] private int questID;
    [SerializeField] private string questTitle;
    [SerializeField] private int rewardMoney;

    public Button questButton;

    private Quest quest; // 현재 QuestContent에 연결된 Quest 객체

    public delegate void OnQuestClicked(QuestContent clickedQuestContent);
    public OnQuestClicked onQuestClickedCallback;

    private void Awake()
    {
        CreateQuest();  // Quest 객체를 초기화합니다.
        questButton.onClick.AddListener(OnQuestButtonClickedInternal);
    }

    // Quest 객체를 생성하고 초기화하는 메서드
    private void CreateQuest()
    {
        quest = new Quest
        {
            questID = this.questID,
            questTitle = this.questTitle,
            rewardMoney = this.rewardMoney
        };
    }


    public void InitializeQuestContent(Quest newQuest)
    {
        quest = newQuest;
        questButton.onClick.AddListener(OnQuestButtonClicked);
    }

    private void OnQuestButtonClickedInternal()
    {
        OnQuestButtonClicked();  // 기존의 로직을 호출

        onQuestClickedCallback?.Invoke(this);  // 콜백 호출
    }

    public void Setup(Quest newQuest)
    {
        quest = newQuest;

        // 퀘스트 데이터를 UI 요소에 바인딩
        // 예: titleText.text = quest.questTitle;
        // 아이콘, 설명 등도 여기에서 설정할 수 있습니다.
    }

    public Quest GetQuest()
    {
        if (quest == null)
        {
            CreateQuest();
        }
        return quest;
    }

    // 추가적으로, Quest를 수락하는 로직을 버튼에 연결할 수 있습니다.
    public void OnQuestButtonClicked()
    {
        if (quest.status == QuestStatus.NotAccepted)
        {

        }
        // 추가적인 로직 (예: 퀘스트 수락 알림 표시)
    }
}
