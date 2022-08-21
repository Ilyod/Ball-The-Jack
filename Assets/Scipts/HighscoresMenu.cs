using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoresMenu : MonoBehaviour
{
    public TextMeshProUGUI[] TimesTexts;
    public TextMeshProUGUI[] PlayersTexts;
    public TextMeshProUGUI LevelText;
    public GameObject PrecedentButton;
    public GameObject NextButton;

    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 1;
        displayScores();

        PrecedentButton.SetActive(false);
        NextButton.SetActive(true);
    }

    private void displayScores()
    {
        LevelText.text = "Level " + currentLevel.ToString();
        // The 5 best times for each level are saved
        // The times are saved under the name "Level*x*Time*y*" and the player names under the name "Level*x*Player*y*"
        string scoreNameBase = "Level" + currentLevel.ToString();
        string scoreTimeNameBase = scoreNameBase + "Time";
        string scorePlayerNameBase = scoreNameBase + "Player";
        for (int i = 1; i <= 5; i++)
        {
            string scorePlayerName = scorePlayerNameBase + i.ToString();
            string scoreTimeName = scoreTimeNameBase + i.ToString();

            TimesTexts[i - 1].text = PlayerPrefs.GetFloat(scoreTimeName, 0).ToString() + " s";
            PlayersTexts[i - 1].text = PlayerPrefs.GetString(scorePlayerName, "-");
        }
    }

    public void OnPressPrecedentButton()
    {
        currentLevel--;
        NextButton.SetActive(true);
        displayScores();
        if (currentLevel <= 1)
        {
            PrecedentButton.SetActive(false);
        }
    }

    public void OnPressNextButton()
    {
        currentLevel++;
        PrecedentButton.SetActive(true);
        displayScores();
        if (currentLevel >= 3)
        {
            NextButton.SetActive(false);
        }
    }

    public void DeleteLevelScore()
    {
        string scoreNameBase = "Level" + currentLevel.ToString();
        string scoreTimeNameBase = scoreNameBase + "Time";
        string scorePlayerNameBase = scoreNameBase + "Player";
        for (int i = 1; i <= 5; i++)
        {
            string scorePlayerName = scorePlayerNameBase + i.ToString();
            string scoreTimeName = scoreTimeNameBase + i.ToString();

            PlayerPrefs.DeleteKey(scoreTimeName);
            PlayerPrefs.DeleteKey(scorePlayerName);
        }
        displayScores();
    }
}
