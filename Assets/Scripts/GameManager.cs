using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PetNeeds petNeeds;
    [SerializeField] private PetAnimationController petAnimController;

    [Header("TopBar")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Button btnHub;

    [Header("ActionBar")]
    [SerializeField] private Button btnAlimentar;
    [SerializeField] private Button btnJugar;
    [SerializeField] private Button btnBanar;
    [SerializeField] private Button btnDormir;
    [SerializeField] private Button btnTienda;
    [SerializeField] private Button btnLogros;

    private void Awake()
    {
        // ── Auto-resolución de referencias (fallback si el Inspector no las tiene) ──
        if (petNeeds == null)
            petNeeds = FindObjectOfType<PetNeeds>();
        if (petAnimController == null)
            petAnimController = FindObjectOfType<PetAnimationController>();

        if (btnAlimentar == null) btnAlimentar = GameObject.Find("Btn_Alimentar")?.GetComponent<Button>();
        if (btnJugar     == null) btnJugar     = GameObject.Find("Btn_Jugar")?.GetComponent<Button>();
        if (btnBanar     == null) btnBanar     = GameObject.Find("Btn_Banar")?.GetComponent<Button>();
        if (btnDormir    == null) btnDormir    = GameObject.Find("Btn_Dormir")?.GetComponent<Button>();
        if (btnTienda    == null) btnTienda    = GameObject.Find("Btn_Tienda")?.GetComponent<Button>();
        if (btnLogros    == null) btnLogros    = GameObject.Find("Btn_Logros")?.GetComponent<Button>();
        if (btnHub       == null) btnHub       = GameObject.Find("BtnHub")?.GetComponent<Button>();

        if (nameText  == null) nameText  = GameObject.Find("PetNameText")?.GetComponent<TMP_Text>();
        if (levelText == null) levelText = GameObject.Find("SpeciesText")?.GetComponent<TMP_Text>();
        if (coinsText == null) coinsText = GameObject.Find("CoinsText")?.GetComponent<TMP_Text>();

        Debug.Log($"[GM] Awake resuelto — petNeeds:{petNeeds != null} petAnim:{petAnimController != null}" +
                  $" btnAlimentar:{btnAlimentar != null} btnJugar:{btnJugar != null}" +
                  $" btnBanar:{btnBanar != null} btnDormir:{btnDormir != null}");

        // ── Cablear botones ──
        if (btnAlimentar != null) btnAlimentar.onClick.AddListener(OnAlimentarClicked);
        if (btnJugar     != null) btnJugar    .onClick.AddListener(OnJugarClicked);
        if (btnBanar     != null) btnBanar    .onClick.AddListener(OnBanarClicked);
        if (btnDormir    != null) btnDormir   .onClick.AddListener(OnDormirClicked);
        if (btnTienda    != null) btnTienda   .onClick.AddListener(OnTiendaClicked);
        if (btnLogros    != null) btnLogros   .onClick.AddListener(OnLogrosClicked);
        if (btnHub       != null) btnHub      .onClick.AddListener(OnHubClicked);
    }

    private void OnEnable()
    {
        if (petNeeds == null) return;
        petNeeds.OnDataLoaded     += RefreshTopBar;
        petNeeds.OnXpChanged      += OnXpChanged;
        petNeeds.OnMonedasChanged += OnMonedasChanged;
    }

    private void OnDisable()
    {
        if (petNeeds == null) return;
        petNeeds.OnDataLoaded     -= RefreshTopBar;
        petNeeds.OnXpChanged      -= OnXpChanged;
        petNeeds.OnMonedasChanged -= OnMonedasChanged;
    }

    private void Start()
    {
<<<<<<< HEAD
        btnAlimentar.onClick.AddListener(petAnimController.PlayEatAnimation);
        btnJugar    .onClick.AddListener(petAnimController.PlayPlayAnimation);
        btnBanar    .onClick.AddListener(petAnimController.PlayBathAnimation);
        if (btnDormir != null) btnDormir.onClick.AddListener(OnDormirClicked);
        btnTienda   .onClick.AddListener(OpenTienda);
        btnLogros   .onClick.AddListener(OpenLogros);
=======
        // OnEnable corre antes de Start — si petNeeds se resolvió en Awake pero
        // OnEnable ya había pasado con null, nos re-suscribimos aquí de forma segura.
        if (petNeeds != null)
        {
            petNeeds.OnDataLoaded     -= RefreshTopBar;
            petNeeds.OnXpChanged      -= OnXpChanged;
            petNeeds.OnMonedasChanged -= OnMonedasChanged;
            petNeeds.OnDataLoaded     += RefreshTopBar;
            petNeeds.OnXpChanged      += OnXpChanged;
            petNeeds.OnMonedasChanged += OnMonedasChanged;
        }
>>>>>>> e0c651cabc38e6ede1a7f82693974f0d78ae4d2e

        if (TamagotchiApiManager.Instance == null)
        {
            Debug.LogError("[GM] TamagotchiApiManager.Instance NULL");
            return;
        }
        if (petNeeds == null)
        {
            Debug.LogError("[GM] petNeeds NULL en Start — no puedo cargar mascota");
            return;
        }
        TamagotchiApiManager.Instance.CargarMascota(
            1,
            mascota => petNeeds.LoadFromServer(mascota),
            err => Debug.LogError("[GM] Error cargando mascota: " + err)
        );
    }

    // ── Handlers de botones ─────────────────────────────────────────────────

    private void OnAlimentarClicked()
    {
        Debug.Log($"[GM] CLICK Alimentar — petAnim:{petAnimController != null} petNeeds:{petNeeds != null}");
        petAnimController?.PlayEatAnimation();
    }

    private void OnJugarClicked()
    {
        Debug.Log("[GM] CLICK Jugar");
        petAnimController?.PlayPlayAnimation();
    }

<<<<<<< HEAD
    private void OnDormirClicked()
    {
        if (!petNeeds.TieneMonedas(PetNeeds.COSTE_DORMIR)) return;
        petNeeds.GastarMonedas(PetNeeds.COSTE_DORMIR);
        petNeeds.Descansar();
    }

    private void OpenTienda()  { }
    private void OpenLogros()  { }
=======
    private void OnBanarClicked()
    {
        Debug.Log("[GM] CLICK Banar");
        petAnimController?.PlayBathAnimation();
    }

    private void OnDormirClicked()
    {
        Debug.Log("[GM] CLICK Dormir");
        petAnimController?.PlaySleepAnimation();
        petNeeds?.Descansar();
    }

    private void OnTiendaClicked() => SceneManager.LoadScene("TiendaScene");
    private void OnLogrosClicked() => SceneManager.LoadScene("LogrosScene");
    private void OnHubClicked()    => SceneManager.LoadScene("HubScene");

    // ── UI ──────────────────────────────────────────────────────────────────

    private void RefreshTopBar()
    {
        if (nameText  != null) nameText.text  = petNeeds.NombreMascota;
        if (levelText != null) levelText.text = "Nv. " + petNeeds.Nivel;
        if (coinsText != null) coinsText.text = petNeeds.Monedas.ToString();
    }

    private void OnXpChanged(int xp, int nivel)
    {
        if (levelText != null) levelText.text = "Nv. " + nivel;
    }

    private void OnMonedasChanged(int coins)
    {
        if (coinsText != null) coinsText.text = coins.ToString();
    }
>>>>>>> e0c651cabc38e6ede1a7f82693974f0d78ae4d2e
}
