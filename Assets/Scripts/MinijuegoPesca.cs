using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MinijuegoPesca : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform indicator;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text attemptsText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject fishSprite;
    [SerializeField] private Button btnCast;
    [SerializeField] private Button btnVolver;

    [Header("Config")]
    [SerializeField] private float speed = 200f;
    [SerializeField] private float greenZoneSize = 80f;
    [SerializeField] private int maxAttempts = 3;

    private float _barHeight;
    private float _indicatorY;
    private float _direction = 1f;
    private int _score;
    private int _attempts;
    private bool _playing;

    private void Start()
    {
        if (indicator == null)     indicator     = GameObject.Find("Indicator")?.GetComponent<RectTransform>();
        if (greenZone == null)     greenZone     = GameObject.Find("GreenZone")?.GetComponent<RectTransform>();
        if (barBackground == null) barBackground = GameObject.Find("BarBackground")?.GetComponent<RectTransform>();
        if (scoreText == null)     scoreText     = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        if (attemptsText == null)  attemptsText  = GameObject.Find("AttemptsText")?.GetComponent<TMP_Text>();
        if (resultText == null)    resultText    = GameObject.Find("ResultText")?.GetComponent<TMP_Text>();
        if (fishSprite == null)    fishSprite    = GameObject.Find("FishSprite");
        if (btnCast == null)       btnCast       = GameObject.Find("BtnCast")?.GetComponent<Button>();
        if (btnVolver == null)     btnVolver     = GameObject.Find("BtnVolver")?.GetComponent<Button>();

        _barHeight = barBackground.rect.height;
        _indicatorY = 0f;
        _attempts = maxAttempts;
        _score = 0;
        _playing = true;

        RandomizeGreenZone();

        if (fishSprite != null) fishSprite.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);
        if (btnCast != null) btnCast.onClick.AddListener(OnCast);
        if (btnVolver != null) btnVolver.onClick.AddListener(() => SceneManager.LoadScene("MinijuegosMenuScene"));

        UpdateUI();
    }

    private void Update()
    {
        if (!_playing) return;

        _indicatorY += speed * _direction * Time.deltaTime;
        float half = _barHeight / 2f;
        if (_indicatorY >= half)  { _indicatorY = half;  _direction = -1f; }
        if (_indicatorY <= -half) { _indicatorY = -half; _direction =  1f; }

        if (indicator != null)
            indicator.anchoredPosition = new Vector2(0, _indicatorY);

        if (Input.GetKeyDown(KeyCode.Space)) OnCast();
    }

    private void OnCast()
    {
        if (!_playing) return;

        float greenMin = greenZone.anchoredPosition.y - greenZoneSize / 2f;
        float greenMax = greenZone.anchoredPosition.y + greenZoneSize / 2f;

        if (_indicatorY >= greenMin && _indicatorY <= greenMax)
        {
            _score += 10;
            StartCoroutine(ShowFish());
        }
        else
        {
            _attempts--;
        }

        if (_attempts <= 0) EndGame();
        else { RandomizeGreenZone(); UpdateUI(); }
    }

    private System.Collections.IEnumerator ShowFish()
    {
        if (fishSprite != null) fishSprite.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (fishSprite != null) fishSprite.SetActive(false);
    }

    private void RandomizeGreenZone()
    {
        if (greenZone == null) return;
        float half = _barHeight / 2f - greenZoneSize / 2f;
        float randomY = Random.Range(-half, half);
        greenZone.anchoredPosition = new Vector2(0, randomY);
        greenZone.sizeDelta = new Vector2(greenZone.sizeDelta.x, greenZoneSize);
    }

    private void EndGame()
    {
        _playing = false;
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = $"¡Fin! Pescaste {_score / 10} peces";
        }
        var petNeeds = FindObjectOfType<PetNeeds>();
        if (petNeeds != null && _score > 0)
        {
            petNeeds.RecompensaMinijuego((_score / 10) * 15);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)    scoreText.text    = $"Peces: {_score / 10}";
        if (attemptsText != null) attemptsText.text = $"Intentos: {_attempts}/{maxAttempts}";
    }
}
