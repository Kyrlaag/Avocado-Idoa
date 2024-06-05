using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseObjects;
    public GameObject panel;

    private bool isSettingsActive;
    public GameObject settingsObjects;

    private float volume;
    private GameObject[] fallingPlatforms;

    [SerializeField] private bool isMainMenu;

    private void Awake()
    {
        Time.timeScale = 1f;

    }

    private void Start()
    {
        fallingPlatforms = GameObject.FindGameObjectsWithTag("FallingPlatform");

    }
    private void Update()
    {
        if (!isMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.R))
                ReloadLevel();

            if (Input.GetKeyDown(KeyCode.Escape))
                Back();
        }
       
    }


    public void PauseGame()
    {
        SetSliderVolume();

        if (isGamePaused == false)
        {
            Time.timeScale = 0f;
            pauseObjects.SetActive(true);
            panel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseObjects.SetActive(false);
            panel.SetActive(false);
        }
        isGamePaused = !isGamePaused;
    }


    public void ShowSettings()
    {
        pauseObjects.SetActive(false);
        settingsObjects.SetActive(true);
        isSettingsActive = true;
    }


    //ESC works to
    public void Back()
    {
        if (isSettingsActive == true)
        {
            pauseObjects.SetActive(true);
            settingsObjects.SetActive(false);
            isSettingsActive = false;
        }
        else
            PauseGame();


    }


    //R works to
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //can be use for main menu and next level
    public void LoadLevel(string LevelName)
    {
        try
        {
            SceneManager.LoadScene(LevelName);

        }
        catch 
        {

            Debug.Log(LevelName + " not found");
        }
        
    }

    public void VolumeChange(GameObject slider)
    {
        volume = slider.GetComponent<Slider>().value;

        if (slider.name == "Music") 
            FindObjectOfType<AudioManager>().VolumeUpdate(volume, true);
        else
            FindObjectOfType<AudioManager>().VolumeUpdate(volume, false);

    }
    //music dogru calismasi icin theme adli dosya olmasi zorunlu
    public GameObject musicSlider;
    public GameObject sfxSlider;
    public void SetSliderVolume()
    {
        musicSlider.GetComponent<Slider>().value = FindObjectOfType<AudioManager>().GetAudioVolume(true);
        sfxSlider.GetComponent<Slider>().value = FindObjectOfType<AudioManager>().GetAudioVolume(false);
    }

    private GameObject player;
    public void ReloadLevel()
    {
        FindObjectOfType<AudioManager>().Play("Die");
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().ReturnToCheckPoint();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
