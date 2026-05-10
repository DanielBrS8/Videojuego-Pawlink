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
        Debug.Log("[GM] === Awake() RUNNING ===");
        Debug.Log("[GM] btnAlimentar null? " + (btnAlimentar == null));
        Debug.Log("[GM] btnJugar null? "     + (btnJugar == null));
        Debug.Log("[GM] btnBanar null? "     + (btnBanar == null));
        Debug.Log("[GM] btnDormir null? "    + (btnDormir == null));
        Debug.Log("[GM] btnTienda null? "    + (btnTienda == null));
        Debug.Log("[GM] btnLogros null? "    + (btnLogros == null));
        Debug.Log("[GM] btnHub null? "       + (btnHub == null));
        Debug.Log("[GM] petAnimController null? " + (petAnimController == null));
        Debug.Log("[GM] petNeeds null? "     + (petNeeds == null));

        if (btnAlimentar != null) btnAlimentar.onClick.AddListener(OnAlimentarClicked);
        if (btnJugar     != null) btnJugar    .onClick.AddListener(OnJugarClicked);
        if (btnBanar     != null) btnBanar    .onClick.AddListener(OnBanarClicked);
        if (btnDormir    != null) btnDormir   .onClick.AddListener(OnDormirClicked);
        if (btnTienda    != null) btnTienda   .onClick.AddListener(OnTiendaClicked);
        if (btnLogros    != null) btnLogros   .onClick.AddListener(OnLogrosClicked);
        if (btnHub       != null) btnHub      .onClick.AddListener(OnHubClicked);
        Debug.Log("[GM] === Awake() DONE ===");
    }

    // Métodos nombrados (más fáciles de detectar en el profiler / debugger que lambdas)
    private void OnAlimentarClicked()
    {
        Debug.Log("[GM] CLICK btnAlimentar");
        if (petAnimController != null) petAnimController.PlayEatAnimation();
    }

    private void OnJugarClicked()
    {
        Debug.Log("[GM] CLICK btnJugar");
        if (petAnimController != null) petAnimController.PlayPlayAnimation();
    }

    private void OnBanarClicked()
    {
        Debug.Log("[GM] CLICK btnBanar");
        if (petAnimController != null) petAnimController.PlayBathAnimation();
    }

    private void OnDormirClicked()
    {
        Debug.Log("[GM] CLICK btnDormir");
        if (petAnimController != null) petAnimController.PlaySleepAnimation();
        if (petNeeds != null) petNeeds.Descansar();
    }

    private void OnTiendaClicked()
    {
        Debug.Log("[GM] CLICK btnTienda → TiendaScene");
        SceneManager.LoadScene("TiendaScene");
    }

    private void OnLogrosClicked()
    {
        Debug.Log("[GM] CLICK btnLogros → LogrosScene");
        SceneManager.LoadScene("LogrosScene");
    }

    private void OnHubClicked()
    {
        Debug.Log("[GM] CLICK btnHub → HubScene");
        SceneManager.LoadScene("HubScene");
    }

    private void OnEnable()
    {
        Debug.Log("[GM] === OnEnable() RUNNING ===");
        if (petNeeds == null)
        {
            Debug.LogError("[GM] petNeeds NULL en OnEnable — referencia serializada perdida");
            return;
        }
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
        Debug.Log("[GM] === Start() RUNNING ===");
        if (TamagotchiApiManager.Instance == null)
        {
            Debug.LogError("[GM] TamagotchiApiManager.Instance NULL — singleton no inicializado");
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
}
