using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public List<GameObject> PlayerCards;

    public List<GameObject> PlayerBackdrops;

    public List<Texture> CollectableCards;

    public Text CurrentPlayerText;

    public GameObject WinnerPanel;

    private List<bool> ShowPlayerCard;

    // Start is called before the first frame update
    void Start()
    {
        WinnerPanel.SetActive(false);
        for (var id = 0; id < GameRules.PlayerCount; id++)
        {
            PlayerCards[id].GetComponent<RawImage>().texture = CollectableCards.Last();
        }

        var missingPlayers = 4 - GameRules.PlayerCount;

        for (var i = 0; i < missingPlayers; i++)
        {
            PlayerBackdrops[3 - i].SetActive(false);
        }

        ShowPlayerCard = new List<bool>()
        {
            false,
            false,
            false,
            false
        };
    }
    private Texture GetTextureForPlayer(Int32 playerId)
    {
        var playerObjective = GameRules.GetObjectiveForPlayer(playerId);

        if (playerObjective.Objective == Objective.ReturnToStart)
        {
            return CollectableCards[CollectableCards.Count - 2];
        }
        else
        {
            return CollectableCards[(int)playerObjective.Collectable];
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(GameRules.GetCurrentPlayer())
        {
            case 0:
                CurrentPlayerText.text = "Red";
                CurrentPlayerText.color = Color.red;
                break;

            case 1:
                CurrentPlayerText.text = "Green";
                CurrentPlayerText.color = Color.green;
                break;

            case 2:
                CurrentPlayerText.text = "Blue";
                CurrentPlayerText.color = Color.blue;
                break;

            case 3:
                CurrentPlayerText.text = "Yellow";
                CurrentPlayerText.color = Color.yellow;
                break;
        }

        if(GameRules.Winner != null)
        {
            WinnerPanel.SetActive(true);
            switch(GameRules.Winner)
            {
                case 0:
                    WinnerPanel.GetComponentInChildren<Text>().text = "Red player wins!";
                    break;

                case 1:
                    WinnerPanel.GetComponentInChildren<Text>().text = "Green player wins!";
                    break;

                case 2:
                    WinnerPanel.GetComponentInChildren<Text>().text = "Blue player wins!";
                    break;

                case 3:
                    WinnerPanel.GetComponentInChildren<Text>().text = "Yellow player wins!";
                    break;
            }
        }
    }

    public void TogglePlayerCard(int id)
    {
        ShowPlayerCard[id] = !ShowPlayerCard[id];
        if (ShowPlayerCard[id])
        {
            PlayerCards[id].GetComponent<RawImage>().texture = GetTextureForPlayer(id);
        }
        else
        {
            PlayerCards[id].GetComponent<RawImage>().texture = CollectableCards.Last();
        }
    }

    public void HideAllPlayerCards()
    {
        for(var id = 0; id < GameRules.PlayerCount; id++)
        {
            ShowPlayerCard[id] = false;
            PlayerCards[id].GetComponent<RawImage>().texture = CollectableCards.Last();
        }
    }
}
