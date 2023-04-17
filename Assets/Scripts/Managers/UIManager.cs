using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : GameStateObserver
{
    const int RANKING_SLOTS_COUNT = 5;
    const int NAME_CHARACTERS_COUNT = 3;

    [Header("Panels")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject rankPanel;
    [SerializeField] private GameObject indicationsPanel;
    [SerializeField] private GameObject namePanel;

    [Space, Header("Name")]
    [SerializeField] private GameObject nameButton;
    [SerializeField] private TMP_InputField nameInput;

    [Space, Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI rankSlotLabel;

    public static string PlayerName { get; private set; }

    public void ValidateName() 
    {
        nameButton.SetActive(nameInput.text.Length >= NAME_CHARACTERS_COUNT);
    }

    public override void UpdateState(GameState newState)
    {
        TurnOffPanels();

        base.UpdateState(newState);
    }

    private void TurnOffPanels()
    {
        scorePanel.SetActive(false);
        rankPanel.SetActive(false);
        indicationsPanel.SetActive(false);
        namePanel.SetActive(false);
    }

    protected override void Home()
    {
        namePanel.SetActive(true);
    }

    protected override void Waiting()
    {
        indicationsPanel.SetActive(true);

        if (GameManager.Instance.Mode == GameMode.Web) 
        {
            rankPanel.SetActive(true);

            if(PlayerName == null) PlayerName = nameInput.text;
        }
    }

    protected override void Playing()
    {
        scorePanel.SetActive(true);
    }

    protected override void Restarting()
    {
        scorePanel.SetActive(true);
        indicationsPanel.SetActive(true);
    }

    public void UpdateScore(int currentScore)
    {
        scoreLabel.text = currentScore.ToString("000000");
    }

    public void UpdateRanking(List<RankSlotData> rankingInfo)
    {
        for (int i = 0; i < RANKING_SLOTS_COUNT; i++)
        {
            var label = rankSlotLabel;
            int rank = i + 1;
            string resultText;

            if(rankingInfo == null) 
            {
                resultText = rank + ". | --- | 000000";
                label.text = resultText;
                label.color = Color.gray;
                continue;
            }

            if (i > 0)
            {
                label = Instantiate(rankSlotLabel);
                label.transform.SetParent(rankSlotLabel.transform.parent);
            }

            if (i >= rankingInfo.Count)
            {
                resultText = rank + ". | --- | 000000";
                label.color = Color.gray;
            }
            else
            {
                var info = rankingInfo[i];
                resultText = string.Format("{0}. | {1} | {2}", rank, info.Name, info.Score.ToString("000000"));
                if (i > 0) label.color = Color.black;
            }

            label.text = resultText;
        }
    }

    public IEnumerator ResetRanking()
    {
        while (rankSlotLabel.transform.parent.childCount > 1)
        {
            Destroy(rankSlotLabel.transform.parent.GetChild(1).gameObject);
            yield return null;
        }
    }
}

