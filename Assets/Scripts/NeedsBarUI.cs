using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Barra de progreso basada en anchorMax.x — no requiere sprite ni Image.Type.Filled.
/// El BarFill debe tener: anchorMin{0,0.1}, anchorMax{0,0.9}, pivot{0,0.5}, sizeDelta{0,0}.
/// El fill se controla desplazando anchorMax.x de 0 a 1.
/// </summary>
[RequireComponent(typeof(Image))]
public class NeedsBarUI : MonoBehaviour
{
    public enum StatType { Hambre, Felicidad, Energia, Higiene, XP }

    [SerializeField] private StatType statType;
    [SerializeField] private PetNeeds petNeeds;
    [SerializeField] private float    lerpSpeed = 6f;

    [Header("Texto opcional (auto-encuentra hermano llamado 'ValueText' si null)")]
    [SerializeField] private TMP_Text valueText;

    [Header("Colores")]
    [SerializeField] private Color colorFull     = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color colorMid      = new Color(0.95f, 0.78f, 0.35f);
    [SerializeField] private Color colorCritical = new Color(0.90f, 0.35f, 0.35f);
    [SerializeField] [Range(0f, 1f)] private float criticalLevel = 0.20f;
    [SerializeField] [Range(0f, 1f)] private float midLevel      = 0.50f;

    private Image         _img;
    private RectTransform _rt;
    private float         _current;
    private float         _target;
    private Action<int,int> _xpHandler;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _rt  = GetComponent<RectTransform>();
        // Configurar anclas para fill por anchorMax.x
        _rt.anchorMin = new Vector2(0f, 0.05f);
        _rt.anchorMax = new Vector2(1f, 0.95f);
        _rt.sizeDelta = Vector2.zero;
        _rt.pivot     = new Vector2(0f, 0.5f);

        // Auto-resolución del ValueText: busca un hermano llamado "ValueText"
        if (valueText == null && transform.parent != null)
        {
            var sibling = transform.parent.parent != null ? transform.parent.parent.Find("ValueText") : null;
            if (sibling != null) valueText = sibling.GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (petNeeds == null) { Debug.LogWarning($"[NeedsBarUI] {name} — petNeeds es NULL"); return; }

        switch (statType)
        {
            case StatType.Hambre:    petNeeds.OnHambreChanged    += SetTarget; break;
            case StatType.Felicidad: petNeeds.OnFelicidadChanged += SetTarget; break;
            case StatType.Energia:   petNeeds.OnEnergiaChanged   += SetTarget; break;
            case StatType.Higiene:   petNeeds.OnHigieneChanged   += SetTarget; break;
            case StatType.XP:
                _xpHandler = (xp, _) => SetTarget(petNeeds.XpNorm);
                petNeeds.OnXpChanged += _xpHandler;
                break;
        }

        float init = GetCurrent();
        _target  = init;
        _current = init;
        ApplyFill(init);
        UpdateColor(init);
        UpdateValueText(init);
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
            case StatType.XP:
                if (_xpHandler != null) petNeeds.OnXpChanged -= _xpHandler;
                break;
        }
    }

    private void Update()
    {
        _current = Mathf.Lerp(_current, _target, lerpSpeed * Time.deltaTime);
        ApplyFill(_current);
    }

    private void ApplyFill(float v)
    {
        Vector2 max = _rt.anchorMax;
        max.x = Mathf.Clamp01(v);
        _rt.anchorMax = max;
    }

    private void SetTarget(float v)
    {
        _target = v;
        UpdateColor(v);
        UpdateValueText(v);
    }

    private void UpdateColor(float v)
    {
        _img.color = v <= criticalLevel ? colorCritical
                   : v <= midLevel      ? colorMid
                   : colorFull;
    }

    private void UpdateValueText(float norm)
    {
        if (valueText == null) return;
        if (statType == StatType.XP)
            valueText.text = Mathf.RoundToInt(norm * 100f) + "%";
        else
            valueText.text = Mathf.RoundToInt(norm * 100f).ToString();
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
