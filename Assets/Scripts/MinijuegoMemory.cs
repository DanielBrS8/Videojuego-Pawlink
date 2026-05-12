using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MinijuegoMemory : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int cols = 4;
    [SerializeField] private int rows = 3;
    [SerializeField] private float cardSize = 120f;
    [SerializeField] private float cardSpacing = 20f;

    [Header("UI")]
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private TMP_Text attemptsText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button btnVolver;

    [Header("Sprites — asignar 6 distintos")]
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private Sprite cardBack;

    private class Card
    {
        public GameObject go;
        public Image img;
        public int pairId;
        public bool isFlipped;
        public bool isMatched;
    }

    private List<Card> _cards = new();
    private Card _firstFlipped;
    private bool _canFlip = true;
    private int _attempts;
    private int _matched;

    private void Start()
    {
        if (gridContainer == null) gridContainer = GameObject.Find("GridContainer")?.GetComponent<RectTransform>();
        if (attemptsText == null)  attemptsText  = GameObject.Find("AttemptsText")?.GetComponent<TMP_Text>();
        if (resultText == null)    resultText     = GameObject.Find("ResultText")?.GetComponent<TMP_Text>();
        if (btnVolver == null)     btnVolver      = GameObject.Find("BtnVolver")?.GetComponent<Button>();

        if (resultText != null) resultText.gameObject.SetActive(false);
        if (btnVolver != null)  btnVolver.onClick.AddListener(() => SceneManager.LoadScene("MinijuegosMenuScene"));

        BuildGrid();
        UpdateUI();
    }

    private void BuildGrid()
    {
        List<int> ids = new();
        for (int i = 0; i < cols * rows / 2; i++) { ids.Add(i); ids.Add(i); }
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        float totalW = cols * cardSize + (cols - 1) * cardSpacing;
        float totalH = rows * cardSize + (rows - 1) * cardSpacing;

        for (int i = 0; i < ids.Count; i++)
        {
            int col = i % cols;
            int row = i / cols;
            float x = -totalW / 2f + col * (cardSize + cardSpacing) + cardSize / 2f;
            float y =  totalH / 2f - row * (cardSize + cardSpacing) - cardSize / 2f;

            GameObject cardGO = new GameObject($"Card_{i}");
            cardGO.transform.SetParent(gridContainer, false);
            RectTransform rt = cardGO.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(cardSize, cardSize);
            rt.anchoredPosition = new Vector2(x, y);

            Image img = cardGO.AddComponent<Image>();
            img.sprite = cardBack;
            img.color = new Color(0.482f, 0.373f, 0.753f, 1f);

            Button btn = cardGO.AddComponent<Button>();
            int idx = i;
            btn.onClick.AddListener(() => OnCardClick(idx));

            Card card = new Card { go = cardGO, img = img, pairId = ids[i] };
            _cards.Add(card);
        }
    }

    private void OnCardClick(int idx)
    {
        Card card = _cards[idx];
        if (!_canFlip || card.isFlipped || card.isMatched) return;

        FlipCard(card, true);

        if (_firstFlipped == null)
        {
            _firstFlipped = card;
        }
        else
        {
            _attempts++;
            UpdateUI();
            if (_firstFlipped.pairId == card.pairId)
            {
                card.isMatched = true;
                _firstFlipped.isMatched = true;
                _firstFlipped = null;
                _matched++;
                if (_matched == cols * rows / 2) StartCoroutine(Win());
            }
            else
            {
                Card first = _firstFlipped;
                _firstFlipped = null;
                _canFlip = false;
                StartCoroutine(FlipBackDelay(first, card));
            }
        }
    }

    private void FlipCard(Card card, bool faceUp)
    {
        card.isFlipped = faceUp;
        if (faceUp && cardSprites != null && card.pairId < cardSprites.Length)
        {
            card.img.sprite = cardSprites[card.pairId];
            card.img.color = Color.white;
        }
        else
        {
            card.img.sprite = cardBack;
            card.img.color = new Color(0.482f, 0.373f, 0.753f, 1f);
        }
    }

    private IEnumerator FlipBackDelay(Card a, Card b)
    {
        yield return new WaitForSeconds(1f);
        FlipCard(a, false);
        FlipCard(b, false);
        _canFlip = true;
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(0.5f);
        int monedas = Mathf.Max(10, 200 - _attempts * 10);
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = $"¡Ganaste!\n{_attempts} intentos\n+{monedas} monedas";
        }
        var petNeeds = FindObjectOfType<PetNeeds>();
        if (petNeeds != null) petNeeds.RecompensaMinijuego(monedas);
    }

    private void UpdateUI()
    {
        if (attemptsText != null) attemptsText.text = $"Intentos: {_attempts}";
    }
}
