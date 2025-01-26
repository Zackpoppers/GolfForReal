using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] playerPositions;
    public GameObject playerPrefab;
    public Transform playerParent;

    private CardManager cardManager;

    void Start()
    {
        cardManager = GameObject.FindGameObjectWithTag("CardManager").GetComponent<CardManager>();
        GeneratePlayers(4);
        cardManager.DrawAndDiscardCard();
    }

    public void GeneratePlayers(int amount)
    {
        for (int i = 0; i < amount && i < 4; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab, playerPositions[i].position, playerPositions[i].rotation);
            newPlayer.name = $"Player{i+1}";
            newPlayer.transform.parent = playerParent;
        }
    }
}