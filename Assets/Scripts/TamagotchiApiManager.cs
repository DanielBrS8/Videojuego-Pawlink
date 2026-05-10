using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Singleton que gestiona todas las llamadas a la API Spring Boot.
/// Usa Coroutines para no bloquear el hilo principal de Unity.
///
/// USO:
///   TamagotchiApiManager.Instance.CargarMascota(idMascota, onSuccess, onError);
///   TamagotchiApiManager.Instance.RealizarAccion("alimentar", onSuccess, onError);
/// </summary>
public class TamagotchiApiManager : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────
    //  SINGLETON
    // ─────────────────────────────────────────────────────────

    public static TamagotchiApiManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─────────────────────────────────────────────────────────
    //  CONFIGURACIÓN
    // ─────────────────────────────────────────────────────────

    [Header("URL base de la API Spring Boot")]
    [SerializeField] private string baseUrl = "http://localhost:8082/api";

    [Tooltip("ID del usuario logueado — asignar al hacer login")]
    public int IdUsuarioActual = 4;  // Provisional; reemplazar con sistema de login

    // ─────────────────────────────────────────────────────────
    //  REFERENCIA A PetNeeds
    // ─────────────────────────────────────────────────────────

    [Header("Referencia al sistema de necesidades")]
    [SerializeField] private PetNeeds petNeeds;

    // ─────────────────────────────────────────────────────────
    //  MASCOTA
    // ─────────────────────────────────────────────────────────

    /// <summary>Carga la mascota activa del usuario desde el servidor.</summary>
    public void CargarMascota(int idMascotaVirtual, Action<MascotaVirtual> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(GET<MascotaVirtual>(
            $"{baseUrl}/juego/mascota/{idMascotaVirtual}",
            data => onSuccess?.Invoke(data),
            onError
        ));
    }

    // ─────────────────────────────────────────────────────────
    //  ACCIONES (alimentar / jugar / bañar)
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Registra una acción y recibe el estado actualizado de la mascota.
    /// tipo: "alimentar" | "jugar" | "bañar"
    /// </summary>
    public void RealizarAccion(string tipo, Action<AccionResponse> onSuccess = null, Action<string> onError = null)
    {
        if (petNeeds == null || petNeeds.IsDead) return;

        petNeeds.BeginSync();

        var request = new AccionRequest
        {
            id_mascota_virtual = petNeeds.IdVirtual,
            tipo               = tipo
        };

        StartCoroutine(POST<AccionRequest, AccionResponse>(
            $"{baseUrl}/acciones",
            request,
            data =>
            {
                petNeeds.ApplyActionResult(tipo, data.puntos_ganados);
                onSuccess?.Invoke(data);
            },
            err =>
            {
                petNeeds.AbortSync();
                onError?.Invoke(err);
                Debug.LogError($"[API] Error en acción {tipo}: {err}");
            }
        ));
    }

    // ─────────────────────────────────────────────────────────
    //  TIENDA
    // ─────────────────────────────────────────────────────────

    public void ObtenerTienda(Action<ItemTiendaList> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GET<ItemTiendaList>($"{baseUrl}/tienda", onSuccess, onError));
    }

    // ─────────────────────────────────────────────────────────
    //  INVENTARIO
    // ─────────────────────────────────────────────────────────

    public void ObtenerInventario(Action<InventarioList> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GET<InventarioList>(
            $"{baseUrl}/inventario/{IdUsuarioActual}",
            onSuccess, onError
        ));
    }

    /// <summary>Compra un item de la tienda y lo añade al inventario.</summary>
    public void ComprarItem(ItemTienda item, Action onSuccess = null, Action<string> onError = null)
    {
        if (petNeeds == null) return;
        if (petNeeds.Monedas < item.coste_monedas)
        {
            onError?.Invoke("Monedas insuficientes");
            return;
        }

        var request = new UsarItemRequest
        {
            id_usuario        = IdUsuarioActual,
            id_item           = item.id_item,
            id_mascota_virtual = petNeeds.IdVirtual
        };

        StartCoroutine(POST<UsarItemRequest, MascotaVirtual>(
            $"{baseUrl}/inventario/comprar",
            request,
            data =>
            {
                petNeeds.GastarMonedas(item.coste_monedas);
                petNeeds.ApplyItemEffects(item);
                petNeeds.SyncFromServer(data);
                onSuccess?.Invoke();
            },
            onError
        ));
    }

    /// <summary>Usa un item del inventario sobre la mascota.</summary>
    public void UsarItem(ItemTienda item, Action onSuccess = null, Action<string> onError = null)
    {
        var request = new UsarItemRequest
        {
            id_usuario        = IdUsuarioActual,
            id_item           = item.id_item,
            id_mascota_virtual = petNeeds.IdVirtual
        };

        // Optimistic UI: aplica el efecto localmente ya
        petNeeds.ApplyItemEffects(item);

        StartCoroutine(POST<UsarItemRequest, MascotaVirtual>(
            $"{baseUrl}/inventario/usar",
            request,
            data => petNeeds.SyncFromServer(data),  // corrección del servidor
            err  =>
            {
                // Revert: volver a cargar del servidor si falla
                CargarMascota(petNeeds.IdVirtual, data => petNeeds.SyncFromServer(data));
                onError?.Invoke(err);
            }
        ));
    }

    // ─────────────────────────────────────────────────────────
    //  LOGROS
    // ─────────────────────────────────────────────────────────

    public void ObtenerLogrosUsuario(Action<LogrosUsuarioList> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GET<LogrosUsuarioList>(
            $"{baseUrl}/logros/usuario/{IdUsuarioActual}",
            onSuccess, onError
        ));
    }

    // ─────────────────────────────────────────────────────────
    //  HTTP HELPERS GENÉRICOS
    // ─────────────────────────────────────────────────────────

    private IEnumerator GET<T>(string url, Action<T> onSuccess, Action<string> onError = null)
    {
        using var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var data = JsonUtility.FromJson<T>(req.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Error parseando JSON: {e.Message}");
                Debug.LogError($"[API GET] {url}\n{req.downloadHandler.text}\n{e}");
            }
        }
        else
        {
            onError?.Invoke(req.error);
            Debug.LogError($"[API GET] {url} → {req.responseCode}: {req.error}");
        }
    }

    private IEnumerator POST<TReq, TRes>(string url, TReq body, Action<TRes> onSuccess, Action<string> onError = null)
    {
        string json = JsonUtility.ToJson(body);
        byte[] raw  = Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler   = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var data = JsonUtility.FromJson<TRes>(req.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Error parseando JSON: {e.Message}");
                Debug.LogError($"[API POST] {url}\n{req.downloadHandler.text}\n{e}");
            }
        }
        else
        {
            onError?.Invoke($"{req.responseCode}: {req.error}");
            Debug.LogError($"[API POST] {url} → {req.responseCode}: {req.error}");
        }
    }
}
