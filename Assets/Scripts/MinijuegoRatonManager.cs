using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MinijuegoRatonManager : MonoBehaviour
{
    [Header("UI Referencias")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private RectTransform ratonRT;

    [Header("Configuración")]
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private float marginX = 520f;
    [SerializeField] private float marginY = 280f;

    private int   _score;
    private float _timeLeft;
    private bool  _playing;
    private float _nextMoveAt;
    private float _moveInterval;

    private void Start() => BeginGame();

    private void Update()
    {
        if (!_playing) return;

        _timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(Mathf.Max(0f, _timeLeft)).ToString();

        // Dificultad progresiva: el ratón se mueve más rápido con el tiempo
        _moveInterval = Mathf.Lerp(0.35f, 1.2f, _timeLeft / gameDuration);

        if (Time.time >= _nextMoveAt)
            MoveRaton();

        if (_timeLeft <= 0f)
            EndGame();
    }

    public void OnRatonClicked()
    {
        if (!_playing) return;
        _score++;
        UpdateScoreUI();
        MoveRaton();
    }

    public void Reintentar() => BeginGame();

    public void Salir() => SceneManager.LoadScene("MinijuegosMenuScene");

    // ─────────────────────────────────────────────────────────

    private void BeginGame()
    {
        _score        = 0;
        _timeLeft     = gameDuration;
        _moveInterval = 1.2f;
        _playing      = true;
        endPanel.SetActive(false);
        UpdateScoreUI();
        MoveRaton();
    }

    private void MoveRaton()
    {
        ratonRT.anchoredPosition = new Vector2(
            Random.Range(-marginX, marginX),
            Random.Range(-marginY, marginY)
        );
        _nextMoveAt = Time.time + _moveInterval;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Ratones: " + _score;
    }

    private void EndGame()
    {
        _playing = false;
        endPanel.SetActive(true);
        finalScoreText.text = "Atrapaste " + _score + " ratones!";
    }
}
