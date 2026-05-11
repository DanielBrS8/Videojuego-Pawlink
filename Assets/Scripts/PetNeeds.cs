using System;
using UnityEngine;

/// <summary>
/// Stats: Hambre · Felicidad · Energía · Higiene  (alineadas con mascotas_virtuales)
///
/// FLUJO:
///  1. ApiManager carga la mascota → llama a LoadFromServer()
///  2. Stats decaen localmente para dar feedback visual inmediato
///  3. Al realizar acción → ApiManager POST → respuesta → SyncFromServer()
///  4. Muerte: hambre Y felicidad llegan a 0
/// </summary>
public class PetNeeds : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────
    //  ENUM
    // ─────────────────────────────────────────────────────────

    public enum PetState { Normal, Hungry, Sad, Tired, Dirty, Critical, Dead }

    // ─────────────────────────────────────────────────────────
    //  INSPECTOR
    // ─────────────────────────────────────────────────────────

    [Header("Decaimiento local (unidades / segundo)")]
    [SerializeField] private float hambreDecay    = 8f;
    [SerializeField] private float felicidadDecay = 5f;
    [SerializeField] private float energiaDecay   = 6f;
    [SerializeField] private float higieneDecay   = 3f;

    [Header("Umbral crítico")]
    [Range(0, 40)] [SerializeField] private float criticalThreshold = 20f;

    [Header("XP por nivel")]
    [SerializeField] private int xpPerLevel = 200;

    // ─────────────────────────────────────────────────────────
    //  PROPIEDADES PÚBLICAS
    // ─────────────────────────────────────────────────────────

    public float Hambre    { get; private set; } = 100f;
    public float Felicidad { get; private set; } = 100f;
    public float Energia   { get; private set; } = 100f;
    public float Higiene   { get; private set; } = 100f;

    public int   Nivel       { get; private set; }
    public int   Experiencia { get; private set; }
    public int   Monedas     { get; private set; }  // Suma de puntos_ganados de acciones

    public string NombreMascota { get; private set; }
    public string Especie       { get; private set; }
    public int    IdVirtual     { get; private set; }
    public int    IdUsuario     { get; private set; }

    // Normalizados 0..1
    public float HambreNorm    => Hambre    / 100f;
    public float FelicidadNorm => Felicidad / 100f;
    public float EnergiaNorm   => Energia   / 100f;
    public float HigieneNorm   => Higiene   / 100f;
    public float XpNorm        => xpPerLevel > 0 ? (Experiencia % xpPerLevel) / (float)xpPerLevel : 0f;

    public PetState CurrentState { get; private set; } = PetState.Normal;
    public bool IsDead => CurrentState == PetState.Dead;

    // ─────────────────────────────────────────────────────────
    //  EVENTOS
    // ─────────────────────────────────────────────────────────

    public event Action<float> OnHambreChanged;
    public event Action<float> OnFelicidadChanged;
    public event Action<float> OnEnergiaChanged;
    public event Action<float> OnHigieneChanged;
    public event Action<int, int> OnXpChanged;      // (experiencia, nivel)
    public event Action<int>   OnMonedasChanged;
    public event Action<PetState> OnStateChanged;
    public event Action        OnPetDied;
    public event Action        OnDataLoaded;

    // ─────────────────────────────────────────────────────────
    //  ESTADO INTERNO
    // ─────────────────────────────────────────────────────────

    private bool _isDataLoaded = true;
    private bool _isSyncing    = false;

    // ─────────────────────────────────────────────────────────
    //  UNITY
    // ─────────────────────────────────────────────────────────

    private void Update()
    {
        if (!_isDataLoaded || IsDead || _isSyncing) return;
        ApplyDecay(Time.deltaTime);
        EvaluateState();
    }

    // ─────────────────────────────────────────────────────────
    //  CARGA / SYNC CON SERVIDOR
    // ─────────────────────────────────────────────────────────

    public void LoadFromServer(MascotaVirtual data, int monedasIniciales = 0)
    {
        ApplyData(data);
        Monedas = monedasIniciales;
        _isDataLoaded = true;
        EvaluateState();
        OnDataLoaded?.Invoke();
    }

    public void SyncFromServer(MascotaVirtual data, int puntosGanados = 0)
    {
        _isSyncing = false;
        ApplyData(data);
        if (puntosGanados > 0)
        {
            Monedas += puntosGanados;
            OnMonedasChanged?.Invoke(Monedas);
        }
        EvaluateState();
    }

    private void ApplyData(MascotaVirtual d)
    {
        IdVirtual    = d.id_virtual;
        IdUsuario    = d.id_usuario;
        NombreMascota = d.nombre;
        Especie      = d.especie;
        Nivel        = d.nivel;
        Experiencia  = d.experiencia;

        SetHambre(d.hambre);
        SetFelicidad(d.felicidad);
        SetEnergia(d.energia);
        SetHigiene(d.higiene);

        OnXpChanged?.Invoke(Experiencia, Nivel);
    }

    // ─────────────────────────────────────────────────────────
    //  DECAIMIENTO LOCAL
    // ─────────────────────────────────────────────────────────

    private void ApplyDecay(float dt)
    {
        SetHambre(Hambre       - hambreDecay    * dt);
        SetFelicidad(Felicidad - felicidadDecay * dt);
        SetEnergia(Energia     - energiaDecay   * dt);
        SetHigiene(Higiene     - higieneDecay   * dt);
    }

    // ─────────────────────────────────────────────────────────
    //  SETTERS INTERNOS
    // ─────────────────────────────────────────────────────────

    private void SetHambre(float v)
    {
        Hambre = Mathf.Clamp(v, 0f, 100f);
        OnHambreChanged?.Invoke(HambreNorm);
    }
    private void SetFelicidad(float v)
    {
        Felicidad = Mathf.Clamp(v, 0f, 100f);
        OnFelicidadChanged?.Invoke(FelicidadNorm);
    }
    private void SetEnergia(float v)
    {
        Energia = Mathf.Clamp(v, 0f, 100f);
        OnEnergiaChanged?.Invoke(EnergiaNorm);
    }
    private void SetHigiene(float v)
    {
        Higiene = Mathf.Clamp(v, 0f, 100f);
        OnHigieneChanged?.Invoke(HigieneNorm);
    }

    // ─────────────────────────────────────────────────────────
    //  EVALUACIÓN DE ESTADO
    // ─────────────────────────────────────────────────────────

    private void EvaluateState()
    {
        if (Hambre <= 0 && Felicidad <= 0)
        {
            if (CurrentState != PetState.Dead)
            {
                SetState(PetState.Dead);
                OnPetDied?.Invoke();
            }
            return;
        }

        int critCount = 0;
        if (Hambre    < criticalThreshold) critCount++;
        if (Felicidad < criticalThreshold) critCount++;
        if (Energia   < criticalThreshold) critCount++;
        if (Higiene   < criticalThreshold) critCount++;

        PetState next;
        if      (critCount >= 2)               next = PetState.Critical;
        else if (Hambre    < criticalThreshold) next = PetState.Hungry;
        else if (Felicidad < criticalThreshold) next = PetState.Sad;
        else if (Energia   < criticalThreshold) next = PetState.Tired;
        else if (Higiene   < criticalThreshold) next = PetState.Dirty;
        else                                    next = PetState.Normal;

        SetState(next);
    }

    private void SetState(PetState s)
    {
        if (CurrentState == s) return;
        CurrentState = s;
        OnStateChanged?.Invoke(s);
    }

    // ─────────────────────────────────────────────────────────
    //  HELPERS PARA ApiManager
    // ─────────────────────────────────────────────────────────

    public void BeginSync() => _isSyncing = true;
    public void AbortSync() => _isSyncing = false;

    // Debe coincidir con EFECTO_ACCION en AccionService.java del backend.
    private const float EfectoAccion = 40f;

    public void ApplyActionResult(string tipo, int puntos)
    {
        Debug.Log($"[PetNeeds] ApplyActionResult llamado: {tipo}");
        _isSyncing = false;

        switch (tipo)
        {
            case "alimentar": SetHambre(Hambre       + EfectoAccion); break;
            case "jugar":     SetFelicidad(Felicidad + EfectoAccion); break;
            case "bañar":     SetHigiene(Higiene     + EfectoAccion); break;
        }

        if (puntos > 0)
        {
            Experiencia += puntos;
            if (Experiencia >= xpPerLevel)
            {
                Nivel++;
                Experiencia -= xpPerLevel;
            }
            OnXpChanged?.Invoke(Experiencia, Nivel);

            Monedas += puntos;
            OnMonedasChanged?.Invoke(Monedas);
        }

        EvaluateState();
    }

    /// <summary>Optimistic UI: aplica efectos de item antes de confirmación del servidor.</summary>
    public void ApplyItemEffects(ItemTienda item)
    {
        SetHambre(Hambre       + item.efecto_hambre);
        SetFelicidad(Felicidad + item.efecto_felicidad);
        SetEnergia(Energia     + item.efecto_energia);
        SetHigiene(Higiene     + item.efecto_higiene);
        EvaluateState();
    }

    public void Descansar()
    {
        SetEnergia(Mathf.Min(100f, Energia + 40f));
        EvaluateState();
    }

    public void GastarMonedas(int cantidad)
    {
        Monedas = Mathf.Max(0, Monedas - cantidad);
        OnMonedasChanged?.Invoke(Monedas);
    }
}
