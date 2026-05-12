using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra visual para una de las 4 stats: Hambre, Felicidad, Energía, Higiene.
/// Requiere Image con Type=Filled, FillMethod=Horizontal.
/// </summary>
[RequireComponent(typeof(Image))]
public class NeedsBarUI : MonoBehaviour
{
    public enum StatType { Hambre, Felicidad, Energia, Higiene, XP }

    [SerializeField] private StatType   statType;
    [SerializeField] private PetNeeds   petNeeds;
    [SerializeField] private float      lerpSpeed = 6f;

    [Header("Colores")]
    [SerializeField] private Color colorFull     = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color colorMid      = new Color(0.95f, 0.78f, 0.35f);
    [SerializeField] private Color colorCritical = new Color(0.90f, 0.35f, 0.35f);
    [SerializeField] [Range(0f, 1f)] private float criticalLevel = 0.20f;
    [SerializeField] [Range(0f, 1f)] private float midLevel      = 0.50f;

    private Image _img;
    private float _target;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _img.type       = Image.Type.Filled;
        _img.fillMethod = Image.FillMethod.Horizontal;
    }

    private void OnEnable()
    {
        if (petNeeds == null) return;

        switch (statType)
        {
            case StatType.Hambre:    petNeeds.OnHambreChanged    += SetTarget; break;
            case StatType.Felicidad: petNeeds.OnFelicidadChanged += SetTarget; break;
            case StatType.Energia:   petNeeds.OnEnergiaChanged   += SetTarget; break;
            case StatType.Higiene:   petNeeds.OnHigieneChanged   += SetTarget; break;
            case StatType.XP:        petNeeds.OnXpChanged        += (xp, _) => SetTarget(petNeeds.XpNorm); break;
        }

        // Valor inicial
        float init = GetCurrent();
        _target = init;
        _img.fillAmount = init;
        UpdateColor(init);
    }

    private void OnDisable()
    {
        if (petNeeds == null) return;
        switch (statType)
        {
            case StatType.Hambre:    petNeeds.OnHambreChanged    -= SetTarget; break;
            case StatType.Felicidad: petNeeds.OnFelicidadChanged -= SetTarget; break;
            case StatType.Energia:   petNeeds.OnEnergiaChanged   -= SetTarget; break;
            case StatType.Higiene:   petNeeds.OnHigieneChanged   -= SetTarget; break;
        }
    }
    

    private void Update()
    {
        _img.fillAmount = Mathf.Lerp(_img.fillAmount, _target, lerpSpeed * Time.deltaTime);
    }

    private void SetTarget(float v)
    {
        _target = v;
        UpdateColor(v);
    }

    private void UpdateColor(float v)
    {
        _img.color = v <= criticalLevel ? colorCritical
                   : v <= midLevel      ? colorMid
                   : colorFull;
    }

    private float GetCurrent() => statType switch
    {
        StatType.Hambre    => petNeeds.HambreNorm,
        StatType.Felicidad => petNeeds.FelicidadNorm,
        StatType.Energia   => petNeeds.EnergiaNorm,
        StatType.Higiene   => petNeeds.HigieneNorm,
        StatType.XP        => petNeeds.XpNorm,
        _                  => 1f
    };
}
