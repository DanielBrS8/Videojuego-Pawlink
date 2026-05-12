using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class NeedsBarUI : MonoBehaviour
{
    public enum StatType { Hambre, Felicidad, Energia, Higiene, XP }

    [SerializeField] private StatType statType;
    [SerializeField] private PetNeeds petNeeds;
    [SerializeField] private float lerpSpeed = 6f;
    [SerializeField] private TMP_Text valueText;

    [SerializeField] private Color colorFull     = new Color(0.361f, 0.722f, 0.361f);
    [SerializeField] private Color colorMid      = new Color(0.941f, 0.678f, 0.306f);
    [SerializeField] private Color colorCritical = new Color(0.851f, 0.325f, 0.310f);
    [SerializeField] [Range(0f,1f)] private float criticalLevel = 0.20f;
    [SerializeField] [Range(0f,1f)] private float midLevel      = 0.50f;

    private Image _img;
    private float _current;
    private float _target;
    private Action<int,int> _xpHandler;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _img.type = Image.Type.Filled;
        _img.fillMethod = Image.FillMethod.Horizontal;
        _img.fillOrigin = 0;

        if (petNeeds == null)
            petNeeds = FindObjectOfType<PetNeeds>();

        if (valueText == null && transform.parent != null && transform.parent.parent != null)
        {
            var vt = transform.parent.parent.Find("ValueText");
            if (vt != null) valueText = vt.GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (petNeeds == null) { Debug.LogError($"[NeedsBarUI] {name} petNeeds NULL"); return; }

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

        float init = GetNorm();
        _target = _current = init;
        _img.fillAmount = init;
        UpdateColor(init);
        UpdateText(init);
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
        if (Mathf.Abs(_current - _target) > 0.001f)
        {
            _current = Mathf.Lerp(_current, _target, lerpSpeed * Time.deltaTime);
            _img.fillAmount = _current;
        }
    }

    private void SetTarget(float v) { _target = v; UpdateColor(v); UpdateText(v); }

    private void UpdateColor(float v)
    {
        _img.color = v <= criticalLevel ? colorCritical : v <= midLevel ? colorMid : colorFull;
        if (valueText != null) valueText.color = _img.color;
    }

    private void UpdateText(float norm)
    {
        if (valueText == null) return;
        valueText.text = Mathf.RoundToInt(norm * 100f).ToString();
    }

    private float GetNorm() => statType switch
    {
        StatType.Hambre    => petNeeds.HambreNorm,
        StatType.Felicidad => petNeeds.FelicidadNorm,
        StatType.Energia   => petNeeds.EnergiaNorm,
        StatType.Higiene   => petNeeds.HigieneNorm,
        StatType.XP        => petNeeds.XpNorm,
        _                  => 1f
    };
}
