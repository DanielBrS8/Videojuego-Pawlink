using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PetNeeds petNeeds;
    [SerializeField] private PetAnimationController petAnimController;

    [Header("TopBar")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text coinsText;

    [Header("ActionBar")]
    [SerializeField] private Button btnAlimentar;
    [SerializeField] private Button btnJugar;
    [SerializeField] private Button btnBanar;
    [SerializeField] private Button btnTienda;
    [SerializeField] private Button btnLogros;

    private void OnEnable()
    {
        petNeeds.OnDataLoaded    += RefreshTopBar;
        petNeeds.OnXpChanged     += OnXpChanged;
        petNeeds.OnMonedasChanged += OnMonedasChanged;
    }

    private void OnDisable()
    {
        petNeeds.OnDataLoaded    -= RefreshTopBar;
        petNeeds.OnXpChanged     -= OnXpChanged;
        petNeeds.OnMonedasChanged -= OnMonedasChanged;
    }

    private void Start()
    {
        btnAlimentar.onClick.AddListener(petAnimController.PlayEatAnimation);
        btnJugar    .onClick.AddListener(petAnimController.PlayPlayAnimation);
        btnBanar    .onClick.AddListener(petAnimController.PlayBathAnimation);
        btnTienda   .onClick.AddListener(OpenTienda);
        btnLogros   .onClick.AddListener(OpenLogros);

        // Carga la mascota con ID 1 del usuario actual.
        // TamagotchiApiManager.petNeeds también debe apuntar al mismo PetNeeds para que
        // RealizarAccion funcione. Asignar ambas referencias en el Inspector de GameScene.
        TamagotchiApiManager.Instance.CargarMascota(
            1,
            mascota => petNeeds.LoadFromServer(mascota),
            err => Debug.LogError($"[GameManager] Error cargando mascota: {err}")
        );
    }

    private void RefreshTopBar()
    {
        nameText.text  = petNeeds.NombreMascota;
        levelText.text = $"Nv. {petNeeds.Nivel}";
        coinsText.text = petNeeds.Monedas.ToString();
    }

    private void OnXpChanged(int xp, int nivel)  => levelText.text = $"Nv. {nivel}";
    private void OnMonedasChanged(int coins)      => coinsText.text = coins.ToString();

    private void OpenTienda()  { }
    private void OpenLogros()  { }
}
