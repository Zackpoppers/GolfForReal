using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor.Toolbars;

public class GameManager : MonoBehaviour
{
    public Transform[] playerPositions;
    private PlayerManager[] playerManagers;
    public GameObject playerPrefab;
    public Transform playerParent;

    private CardManager cardManager;

    public int currentPlayerTurn;
    public int globalTurnCount;
    private int playerCount;
    public GameObject cameraObject;
    public GameObject deckAndDiscardPileParent;
    public bool rotating = false;
    public bool switchingCards = false;

    public GameObject endGameScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerNumberText;
    public bool gameOver = false;

    private float cameraRotateDuration = 0.75f;
    private const float _cardSwapDuration = 0.15f;
    public float cardSwapDuration => _cardSwapDuration; // Getter for the cardSwapDuration

    void Start()
    {
        /*        Settings settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();
                playerCount = settings.playerCount;*/
        // For ben :) ^^^^
        //dont need lol, just use static

        playerCount = MainMenu.PlayerCount;
        cardManager = GameObject.FindGameObjectWithTag("CardManager").GetComponent<CardManager>();
        GeneratePlayers(playerCount);
        cardManager.DrawAndDiscardCard();
        playerNumberText.text = $"Player {currentPlayerTurn + 1}";

    }

    /// <summary>
    /// Generates players that automatically start with cards and get set up for the game to begin
    /// </summary>
    /// <param name="amount">Amount of players to generate</param>
    public void GeneratePlayers(int amount)
    {
        playerManagers = new PlayerManager[amount];
        for (int i = 0; i < amount && i < 4; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab, playerPositions[i].position, playerPositions[i].rotation);
            newPlayer.name = $"Player{i+1}";
            newPlayer.transform.parent = playerParent;
            playerManagers[i] = newPlayer.GetComponent<PlayerManager>();
        }
    }

    /// <summary>
    /// Rotates camera and readies the game for the next players turn
    /// </summary>
    public void NextTurn()
    {
        if (currentPlayerTurn == playerCount - 1) globalTurnCount++;
        if (globalTurnCount == 6)
        {
            EndGame();
            return;
        }
        currentPlayerTurn += 1;
        currentPlayerTurn %= playerCount;
        playerNumberText.text = $"Player {currentPlayerTurn + 1}";
        playerManagers[currentPlayerTurn].EnlargeCards(true);

        if (!cardManager.deckCardDrawn && // Did not draw card
            !cardManager.discardSwitchedWithCardInHand) // Did not switch in-hand card with discarded card (They flipped a card without drawing)
        {
            cardManager.DrawAndDiscardCard(animate:true, goToNextTurn:true);
        }

        cardManager.SetDeckDrawable(true);
        cardManager.discardSwitchedWithCardInHand = false;


        if (!switchingCards) StartCoroutine(RotateOverTime(cameraRotateDuration));
    }

    /// <summary>
    /// Swaps the position of two objects
    /// </summary>
    /// <param name="obj1">First object to swap positions with the other</param>
    /// <param name="obj2">Second object to swap positions with the other</param>
    /// <param name="duration">The length of time in seconds that the swap will take to finish after being started</param>
    /// <returns></returns>
    public IEnumerator SwapPositions(GameObject obj1, GameObject obj2, float duration)
    {
        switchingCards = true;
        // Get the starting positions of both objects
        Vector3 startPos1 = obj1.transform.position;
        Vector3 startPos2 = obj2.transform.position;

        float elapsedTime = 0f;

        // Lerp the positions over the specified duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Smoothly interpolate between the positions
            obj1.transform.position = Vector3.Lerp(startPos1, startPos2, t);
            obj2.transform.position = Vector3.Lerp(startPos2, startPos1, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to final positions to ensure accuracy
        obj1.transform.position = startPos2;
        obj2.transform.position = startPos1;
        switchingCards = false;
        StartCoroutine(RotateOverTime(cameraRotateDuration));
    }

    /// <summary>
    /// Moves an object to a specified position over a given duration.
    /// </summary>
    /// <param name="obj">The object to move</param>
    /// <param name="targetPosition">The position to move the object to</param>
    /// <param name="duration">The length of time in seconds that the movement will take to finish</param>
    /// <returns></returns>
    public IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition, float duration, bool nextTurn)
    {
        switchingCards = true;
        // Get the starting position of the object
        Vector3 startPosition = obj.transform.position;

        float elapsedTime = 0f;

        // Lerp the position over the specified duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Smoothly interpolate between the start and target positions
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position to ensure accuracy
        obj.transform.position = targetPosition;
        switchingCards = false;
        if(nextTurn) StartCoroutine(RotateOverTime(cameraRotateDuration));
    }

    // Generated by ChatGPT
    private IEnumerator RotateOverTime(float duration, float startDelay = 0.2f)
    {
        rotating = true;

        // Wait for {start delay time before rotating}
        while (startDelay >= 0)
        {
            startDelay -= Time.deltaTime;
        }

        // The target rotation angle for the camera and the pile
        int targetRotation = (currentPlayerTurn * 90);
        if (playerCount == 4)
        {
            targetRotation = (int)cameraObject.transform.rotation.eulerAngles.z + 90;
        }
        Debug.Log($"Rotation target: {targetRotation} from {cameraObject.transform.rotation.z}");

        // Start from the current rotation angles
        float startCameraRotation = cameraObject.transform.rotation.eulerAngles.z;
        float startPileRotation = deckAndDiscardPileParent.transform.rotation.eulerAngles.z;

        // Time passed during the rotation
        float elapsedTime = 0f;

        // Gradually rotate until the target rotation is reached
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Normalized time (0 to 1)

            // Interpolate the rotation between the start and target values
            cameraObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startCameraRotation, targetRotation, t));
            deckAndDiscardPileParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startPileRotation, targetRotation, t));

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the final rotation is exactly the target rotation
        cameraObject.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        deckAndDiscardPileParent.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        rotating = false;
    }

    /// <summary>
    /// Calculates scores of each player, displays the end menu and changes "gameOver" to true
    /// </summary>
    private void EndGame()
    {
        gameOver = true;

        string scoreString = "";
        for (int i = 0; i < playerManagers.Length; i++)
        {
            string formattedPlayerScore = $"Player {i + 1} score: {playerManagers[i].CalculateScore()}";
            scoreString += $"{formattedPlayerScore}\n";
        }
        scoreText.text = scoreString;
        endGameScreen.SetActive(true);
        deckAndDiscardPileParent.SetActive(false);
    }
}