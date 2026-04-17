using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [SerializeField] Transform player;
    [SerializeField] TMP_Text distanceText;

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
        float distance = new Vector3(player.position.x, 0f, player.position.z).magnitude;

        distanceText.text = $"{distance:0} m";

        if (distance > 1000f)
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
