using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class BallControler : MonoBehaviour
{
    public float setSpeed = 10;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI timeText;
    public GameObject BackgroundMusicObject;
    public GameObject startObject;
    public TextMeshProUGUI startCountText;
    public GameObject pauseObject;
    public GameObject gameoverObject;
    public TextMeshProUGUI timeResultText;
    public TextMeshProUGUI highscoreText;
    public TMP_InputField playerNameField;

    private AudioSource BackgroundMusic;
    private Rigidbody rb;

    private float speed;
    private float movementX;
    private float movementY;
    private int count;
    private int pickupNumber;
    private float playerTime;
    private int secondsPlayed;
    private int startCount;
    private float startTime;
    private bool gameStarted;
    private bool gamePaused;
    private bool gameOver;
    // scoreName is used to save a new highscore, if it happens
    private string scorePlayerName;

    // Start is called before the first frame update
    void Start()
    {
        BackgroundMusic = BackgroundMusicObject.GetComponent<AudioSource>();
        // We wait for the game to start to start the music
        BackgroundMusic.Stop();

        rb = GetComponent<Rigidbody>();

        // With speed = 0, the ball can't move (before the end of the start count)
        speed = 0;
        count = 0;
        pickupNumber = GameObject.FindGameObjectsWithTag("PickUp").Length;
        secondsPlayed = 0;
        playerTime = 0.0f;
        startCount = 3;
        startTime = 0;
        SetTimeText();
        SetStartCountText();

        gameStarted = false;
        gamePaused = false;
        gameOver = false;
        scorePlayerName = "";

        SetCountText();

        startObject.SetActive(true);
        pauseObject.SetActive(false);
        gameoverObject.SetActive(false);
        playerNameField.gameObject.SetActive(false);
        playerNameField.onEndEdit.AddListener(SavePlayerName);

        Time.timeScale = 1;

        // Uncomment to delete the saved data
        //PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        // At the begining of the game, there is a count before that the player is able to play
        if (gameStarted == false)
        {
            // Every 2s, we update the start count
            if (Time.timeSinceLevelLoad > 1 + startTime)
            {
                startTime += 1;
                startCount--;
                if (startCount == 0)
                {
                    gameStarted = true;
                    startObject.SetActive(false);
                    startTime = Time.timeSinceLevelLoad;
                    // Setting up the speed allow the player to move the ball
                    speed = setSpeed;
                    // Start the music
                    BackgroundMusic.Play();
                }
                else
                {
                    SetStartCountText();
                }
            }
        }
        
        // When the player push "escape"
        if (Keyboard.current.escapeKey.wasPressedThisFrame && gameOver == false)
        {
            // if the game is not paused, it pauses
            if (gamePaused == false)
            {
                Time.timeScale = 0;
                pauseObject.SetActive(true);
                gamePaused = true;
            }
            // if the game is paused, it resumes
            else
            {
                Time.timeScale = 1;
                pauseObject.SetActive(false);
                gamePaused = false;
            }
        }

        // We display the time in second on the screen
        if (Time.timeSinceLevelLoad - startTime > secondsPlayed + 1)
        {
            secondsPlayed++;
            SetTimeText();
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void SetTimeText()
    {
        timeText.text = secondsPlayed.ToString() + "s";
    }

    private void SetStartCountText()
    {
        startCountText.text = startCount.ToString();
    }

    private void SetCountText()
    {
        countText.text = count.ToString() + " / " + pickupNumber.ToString();

        // If it was the last pickup, the game is over
        if (count >= pickupNumber)
        //if (count >= 1) //for the test
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        gameOver = true;
        // The game over music is played when the gameoverObject is activated, so we stop the level music
        BackgroundMusic.Stop();
        playerTime = Time.timeSinceLevelLoad - startTime;
        // Pause the game, making the ball stop
        Time.timeScale = 0;
        gameoverObject.SetActive(true);
        timeResultText.text = "Time :   " + playerTime.ToString() + " s";

        // We store the 5 best times for each level. If it's a new one, we have to save it
        // The times are saved under the name "Level*x*Time*y*" and the player names under the name "Level*x*Player*y*"
        string scoreNameBase = "Level" + SceneManager.GetActiveScene().buildIndex.ToString();
        string scoreTimeNameBase = scoreNameBase + "Time";
        string scorePlayerNameBase = scoreNameBase + "Player";
        for (int i = 1; i <= 5; i++)
        {
            scorePlayerName = scorePlayerNameBase + i.ToString();
            string scoreTimeName = scoreTimeNameBase + i.ToString();
            if (PlayerPrefs.GetFloat(scoreTimeName, 9999) > playerTime)
            {
                highscoreText.text = "NEW HIGHSCORE !\nPosition : " + i.ToString();
                // We shift the precedent highscores
                for (int k = 5; k > i; k--)
                {
                    PlayerPrefs.SetFloat(scoreTimeNameBase + k.ToString(),
                                         PlayerPrefs.GetFloat(scoreTimeNameBase + (k - 1).ToString(), 9999));
                    PlayerPrefs.SetString(scorePlayerNameBase + k.ToString(),
                                          PlayerPrefs.GetString(scorePlayerNameBase + (k - 1).ToString(), "-"));
                }
                PlayerPrefs.SetFloat(scoreTimeName, playerTime);
                // We first save a default name, then, when the player enter his name, we save it (if he doesn't, we keep the default name)
                PlayerPrefs.SetString(scorePlayerName, "Unknown_Ball");
                // We enable the input field to allow the player to enter his name
                PlayerPrefs.Save();
                playerNameField.gameObject.SetActive(true);
                break;
            }
        }
    }
    
    // To save the player name if he did a new highscore
    private void SavePlayerName(string playerName)
    {
        Debug.Log("SavePlayerName : " + playerName);
        PlayerPrefs.SetString(scorePlayerName, playerName);
        PlayerPrefs.Save();
        playerNameField.interactable = false;
    }
}
