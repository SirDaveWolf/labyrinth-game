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

    private List<bool> ShowPlayerCard;

    // Start is called before the first frame update
    void Start()
    {
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
        return CollectableCards[(int)playerObjective.Collectable];
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
