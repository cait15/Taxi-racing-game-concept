using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [Header("GameObjects ")]
    [SerializeField] private GameObject WinningScreen;
    [SerializeField] private GameObject LosingScreen;
    [SerializeField] private GameObject RestartButton;
    [SerializeField] private GameObject Car;
    
    private bool level1;
    private bool level2;
    private bool level3;

    private void Awake()
    {
        level3 = SceneManager.GetActiveScene().name.Contains("3");
        level2 = SceneManager.GetActiveScene().name.Contains("2");
        level1 = SceneManager.GetActiveScene().name.Contains("1");
    }

    private void Start()
    {
        PlayBackTrack();
    }

    private void PlayBackTrack()
    {
        if ( level2)
        {
            SFXManager.instance.PlayRequestedSound("Track2", isLoop: true);
//            Debug.Log(level3);
        }
        else if (level1)
        {
            SFXManager.instance.PlayRequestedSound("Track1", isLoop: true);
        }

        if (level3)
        {
            SFXManager.instance.PlayRequestedSound("Track3", isLoop: true);
        }
    }

    public void NextLevel()
    {
        if ( level2)
        {
           
            SceneManager.LoadScene("AD-DIALOGUE");
        }
        else if (level1)
        {
            SceneManager.LoadScene("B-DIALOGUE");
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("Homescreen");
    }
    private void DisplayWinningScreen()
    {
        Car.SetActive(false);
        CarController.instance.StopAudio();
       SFXManager.instance.StopSound();
        WinningScreen.SetActive(true);
        RestartButton.SetActive(false);
        SFXManager.instance.PlayRequestedSound("WinScreen", isLoop: false);
        
    }

    public void DisplayLosingScreen()
    {
        Car.SetActive(false);
        CarController.instance.StopAudio();
        SFXManager.instance.StopSound();
        LosingScreen.SetActive(true);
        RestartButton.SetActive(false);
        SFXManager.instance.PlayRequestedSound("LoseScreen", isLoop: false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnEnable()
    {
        StackManager.OnWinGameCondition += DisplayWinningScreen;
        RaceAndLapManager.OnWinGameCondition += DisplayWinningScreen;
        RaceAndLapManager.OnLoseGameCondition += DisplayLosingScreen;
        TimerScript.OnLoseGameCondition += DisplayLosingScreen;
    }

    private void OnDisable()
    {
        RaceAndLapManager.OnWinGameCondition -= DisplayWinningScreen;
        RaceAndLapManager.OnLoseGameCondition -= DisplayLosingScreen;
        TimerScript.OnLoseGameCondition -= DisplayLosingScreen;
        StackManager.OnWinGameCondition -= DisplayWinningScreen;
    }
}
