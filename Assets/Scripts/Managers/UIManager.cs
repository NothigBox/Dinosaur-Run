using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : GameStateObserver
{
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
        rankPanel.SetActive(true);
        indicationsPanel.SetActive(true);

        PlayerName = nameInput.text;
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
}

