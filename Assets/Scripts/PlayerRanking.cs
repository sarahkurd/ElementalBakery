using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRanking : MonoBehaviour
{
    public GameObject PlayerRankingUiCanvas; // UI element to display rank
    public GameObject IncorrectOrderCanvas;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI incorrectlyCookedIngredientsText;
    public TextMeshProUGUI rankingText;
    public TextMeshProUGUI incorrectPlatesDeliveredText;
    
    private const string MasterChefHeader = "Well Done! Amazing! You have achieved ... ";
    private const string GreatChefHeader = "Great skills. You have achieved ... ";
    private const string NoviceChefHeader = "You delivered the order. But there is room for improvement.";
    private const string UnrankedChefHeader = "You don't seem like you know what you're doing. Try again?";

    private void Start()
    {
        PlayerRankingUiCanvas.SetActive(false);
        IncorrectOrderCanvas.SetActive(false);
    }

    public void SetTotalTime(float totalTime)
    {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        totalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetHeaderText(PlayerRank rank)
    {
        switch (rank)
        {
            case PlayerRank.Master:
                headerText.text = MasterChefHeader;
                return;
            case PlayerRank.Great:
                headerText.text = GreatChefHeader;
                return;
            case PlayerRank.Novice:
                headerText.text = NoviceChefHeader;
                return;
            case PlayerRank.Unranked:
                headerText.text = UnrankedChefHeader;
                return;
        }
    }

    public void SetIncorrectIngredientsText(int count)
    {
        incorrectlyCookedIngredientsText.text = count.ToString();
    }
    
    public void SetIncorrectPlatesDelivered(int count)
    {
        incorrectPlatesDeliveredText.text = count.ToString();
    }

    public void SetRankingText(PlayerRank rank)
    {
        rankingText.text = rank.ToString();
    }

    // This method should be called when you want to update the rank text on the UI.
    public void DisplayRank()
    {
        PlayerRankingUiCanvas.SetActive(true);
    }

    public void DisplayIncorrectOrder()
    {
        IncorrectOrderCanvas.SetActive(true);
        StartCoroutine(WaitAndDisplayIncorrectOrder());
    }

    private IEnumerator WaitAndDisplayIncorrectOrder()
    {
        yield return new WaitForSeconds(4f);
        IncorrectOrderCanvas.SetActive(false);
    }

    public void SetYouTookToLong()
    {
        rankingText.text = "You Took To Long. Restart.";
    }
}
