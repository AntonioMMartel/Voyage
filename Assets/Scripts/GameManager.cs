using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject winMenu;

    [Header("Win Condition")]
    [SerializeField] float winDistance = 1000f;
    private float distanceTravelled = 0f;
    private bool gameEnded = false;

    private void Awake()
    {
        // Singleton 
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded) return;

        // Example: increase distance over time
        distanceTravelled += Time.deltaTime * 10f; // adjust speed as needed

        if (distanceTravelled >= winDistance)
        {
            WinGame();
        }
    }

    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        loseMenu.SetActive(true);
        Time.timeScale = 0f; 
    }

    void WinGame()
    {
        gameEnded = true;
        winMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
