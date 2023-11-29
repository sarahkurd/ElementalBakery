using System.Collections;
using UnityEngine;
using TMPro;

public class HintOverlayController : MonoBehaviour
{
    public GameObject[] hintPanels; // References to the hint panels in the Unity Inspector
    //public GameObject riddle;
    public TMP_Text[] hintTexts; // References to the TextMeshPro Text components of the hint panels
    public GameObject halfBroken, chicken; // Reference to the halfBroken
    private bool[] hintsShown; // Track whether the hints have been shown

    private void Start()
    {
        hintsShown = new bool[hintPanels.Length];
        for (int i = 0; i < hintPanels.Length; i++)
        {
            // Hide all the hint panels at the start of the level
            hintPanels[i].SetActive(false);
            hintsShown[i] = false;
        }
    }

    private void Update()
    {
        //if (riddle.activeSelf)
        //{
        //    StartCoroutine(HideRiddleAfterDelay(8.0f));
        //}

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && !hintsShown[0])
        {
            ShowHint(0);
            hintsShown[0] = true;
        }
    
        if (hintsShown[0] && hintsShown[1] && !hintsShown[2])
        {
            HideHint(1);
            ShowHint(2);
            hintsShown[2] = true;
        } 

        if (!hintsShown[1] && hintsShown[0] && halfBroken == null)
        {
            HideHint(0);
            ShowHint(1);
            hintsShown[1] = true;
        }
    }

    public void ShowHint(int hintIndex)
    {
        // Show the hint panel for the specified index
        hintPanels[hintIndex].SetActive(true);
    }

    public void HideHint(int hintIndex)
    {
        // Hide the hint panel for the specified index
        hintPanels[hintIndex].SetActive(false);
    }

    /*
    private IEnumerator HideRiddleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide the riddle panel for the specified index after the specified delay
        riddle.SetActive(false);
    }*/
}