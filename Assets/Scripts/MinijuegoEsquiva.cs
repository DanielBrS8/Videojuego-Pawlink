using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MinijuegoEsquiva : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform catRT;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button btnJump;
    [SerializeField] private Button btnVolver;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private RectTransform ground;

    [Header("Config")]
    [SerializeField] private float jumpForce = 600f;
    [SerializeField] private float gravity = -1200f;
    [SerializeField] private float obstacleSpeed = 300f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxLives = 3;

    private float _catVelocityY;
    private float _groundY;
    private bool _isGrounded = true;
    private bool _playing;
    private int _score;
    private int _lives;
    private float _speedMultiplier = 1f;
    private List<RectTransform> _obstacles = new();
    private float _spawnTimer;

    private void Start()
    {
        if (catRT == null)      catRT      = GameObject.Find("Cat")?.GetComponent<RectTransform>();
        if (scoreText == null)  scoreText  = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        if (livesText == null)  livesText  = GameObject.Find("LivesText")?.GetComponent<TMP_Text>();
        if (resultText == null) resultText = GameObject.Find("ResultText")?.GetComponent<TMP_Text>();
        if (btnJump == null)    btnJump    = GameObject.Find("BtnJump")?.GetComponent<Button>();
        if (btnVolver == null)  btnVolver  = GameObject.Find("BtnVolver")?.GetComponent<Button>();
        if (ground == null)     ground     = GameObject.Find("Ground")?.GetComponent<RectTransform>();

        _groundY = catRT != null ? catRT.anchoredPosition.y : -200f;
        _lives = maxLives;
        _score = 0;
        _playing = true;
        _spawnTimer = spawnInterval;

        if (resultText != null) resultText.gameObject.SetActive(false);
        if (btnJump != null)    btnJump.onClick.AddListener(Jump);
        if (btnVolver != null)  btnVolver.onClick.AddListener(() => SceneManager.LoadScene("MinijuegosMenuScene"));

        UpdateUI();
    }

    private void Update()
    {
        if (!_playing) return;

        if (Input.GetKeyDown(KeyCode.Space)) Jump();

        if (!_isGrounded)
        {
            _catVelocityY += gravity * Time.deltaTime;
            Vector2 pos = catRT.anchoredPosition;
            pos.y += _catVelocityY * Time.deltaTime;
            if (pos.y <= _groundY)
            {
                pos.y = _groundY;
                _catVelocityY = 0f;
                _isGrounded = true;
            }
            catRT.anchoredPosition = pos;
        }

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnObstacle();
            _spawnTimer = spawnInterval / _speedMultiplier;
        }

        float speed = obstacleSpeed * _speedMultiplier * Time.deltaTime;
        for (int i = _obstacles.Count - 1; i >= 0; i--)
        {
            if (_obstacles[i] == null) { _obstacles.RemoveAt(i); continue; }
            Vector2 pos = _obstacles[i].anchoredPosition;
            pos.x -= speed;
            _obstacles[i].anchoredPosition = pos;

            float dx = Mathf.Abs(pos.x - catRT.anchoredPosition.x);
            float dy = Mathf.Abs(_obstacles[i].anchoredPosition.y - catRT.anchoredPosition.y);
            if (dx < 35f && dy < 40f)
            {
                Destroy(_obstacles[i].gameObject);
                _obstacles.RemoveAt(i);
                HitObstacle();
                continue;
            }

            if (pos.x < -800f)
            {
                Destroy(_obstacles[i].gameObject);
                _obstacles.RemoveAt(i);
                _score++;
                _speedMultiplier += 0.05f;
                UpdateUI();
            }
        }
    }

    private void Jump()
    {
        if (!_playing || !_isGrounded) return;
        _isGrounded = false;
        _catVelocityY = jumpForce;
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefab == null || ground == null) return;
        GameObject obs = Instantiate(obstaclePrefab, ground.parent);
        RectTransform rt = obs.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(750f, _groundY);
        rt.sizeDelta = new Vector2(40f, 50f);
        _obstacles.Add(rt);
    }

    private void HitObstacle()
    {
        _lives--;
        UpdateUI();
        if (_lives <= 0) EndGame();
    }

    private void EndGame()
    {
        _playing = false;
        foreach (var obs in _obstacles)
            if (obs != null) Destroy(obs.gameObject);
        _obstacles.Clear();

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = $"¡Game Over!\nEsquivaste {_score} obstáculos";
        }

        var petNeeds = FindObjectOfType<PetNeeds>();
        if (petNeeds != null && _score > 0)
        {
            petNeeds.RecompensaMinijuego(_score * 10);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)  scoreText.text  = $"Esquivados: {_score}";
        if (livesText != null)  livesText.text  = $"Vidas: {_lives}/{maxLives}";
    }
}
