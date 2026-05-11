using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PetAnimationController : MonoBehaviour
{
    private static readonly int ParamState    = Animator.StringToHash("State");
    private static readonly int ParamIsActing = Animator.StringToHash("IsActing");

    private const int ANIM_IDLE     = 0;
    private const int ANIM_EAT     = 1;
    private const int ANIM_PLAY    = 2;
    private const int ANIM_SLEEP   = 3;
    private const int ANIM_CRYING  = 4;
    private const int ANIM_CRITICAL = 5;
    private const int ANIM_DEAD    = 6;
    private const int ANIM_BATH    = 7;

    [Header("Duración de animaciones de acción (seg)")]
    [SerializeField] private float eatDuration   = 1.5f;
    [SerializeField] private float playDuration  = 2.0f;
    [SerializeField] private float bathDuration  = 2.0f;

    [SerializeField] private PetNeeds petNeeds;

    private Animator _animator;
    private float    _actionTimer = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        petNeeds ??= GetComponent<PetNeeds>();
    }

    private void OnEnable()
    {
        if (petNeeds == null) return;
        petNeeds.OnStateChanged += HandleStateChanged;
        petNeeds.OnPetDied      += () => SetAnimState(ANIM_DEAD);
    }

    private void OnDisable()
    {
        if (petNeeds == null) return;
        petNeeds.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (_actionTimer <= 0f) return;
        _actionTimer -= Time.deltaTime;
        if (_actionTimer <= 0f)
        {
            _animator.SetBool(ParamIsActing, false);
            HandleStateChanged(petNeeds.CurrentState);
        }
    }

    // ─── ACCIONES DEL JUGADOR ───────────────────────────────

    public void PlayEatAnimation()
    {
        Debug.Log("[PAC] PlayEatAnimation() llamado");
        StartAction(ANIM_EAT, eatDuration);
        CallApi("alimentar");
    }

    public void PlayPlayAnimation()
    {
        StartAction(ANIM_PLAY, playDuration);
        CallApi("jugar");
    }

    public void PlayBathAnimation()
    {
        Debug.Log("[PAC] PlayBathAnimation() llamado — IsDead:" + (petNeeds != null ? petNeeds.IsDead.ToString() : "petNeeds null"));
        StartAction(ANIM_BATH, bathDuration);
        Debug.Log("[PAC] SetState " + ANIM_BATH + " (Bath) — _actionTimer:" + _actionTimer);
        CallApi("bañar");
    }

    public void PlaySleepAnimation()
    {
        StartAction(ANIM_SLEEP, 2.0f);
        // Dormir es local — petNeeds.Descansar() se llama desde GameManager
    }

    private void CallApi(string tipo)
    {
        Debug.Log("[PAC] RealizarAccion llamado: " + tipo);

        if (TamagotchiApiManager.Instance == null)
        {
            Debug.LogError("[PAC] TamagotchiApiManager.Instance es NULL");
            return;
        }

        TamagotchiApiManager.Instance.RealizarAccion(
            tipo,
            onSuccess: response =>
            {
                Debug.Log("[PAC] ApplyActionResult recibido: " + tipo + " puntos=" + response.puntos_ganados);
            },
            onError: err =>
            {
                Debug.LogError("[PAC] Error en RealizarAccion " + tipo + ": " + err);
            }
        );
    }

    private void StartAction(int animState, float duration)
    {
        if (petNeeds != null && petNeeds.IsDead) return;
        SetAnimState(animState);
        _animator.SetBool(ParamIsActing, true);
        _actionTimer = duration;
    }

    // ─── ESTADO PASIVO ──────────────────────────────────────

    private void HandleStateChanged(PetNeeds.PetState state)
    {
        if (_actionTimer > 0f) return;

        int anim = state switch
        {
            PetNeeds.PetState.Normal   => ANIM_IDLE,
            PetNeeds.PetState.Hungry   => ANIM_CRYING,
            PetNeeds.PetState.Sad      => ANIM_CRYING,
            PetNeeds.PetState.Dirty    => ANIM_CRYING,
            PetNeeds.PetState.Tired    => ANIM_SLEEP,
            PetNeeds.PetState.Critical => ANIM_CRITICAL,
            PetNeeds.PetState.Dead     => ANIM_DEAD,
            _                          => ANIM_IDLE
        };

        SetAnimState(anim);
    }

    private void SetAnimState(int state)
    {
        _animator.SetInteger(ParamState, state);
    }
}
