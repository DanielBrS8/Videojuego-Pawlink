using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pone un icono de texto grande encima del label de cada botón.
/// No necesita sprites ni fuentes externas — usa ASCII puro.
/// </summary>
public class ButtonIconManager : MonoBehaviour
{
    private static readonly (string btn, string icon, string label)[] Config =
    {
        ("Btn_Alimentar", "+",  "ALIMENTAR"),
        ("Btn_Jugar",     ">>", "JUGAR"),
        ("Btn_Banar",     "~~", "BANAR"),
        ("Btn_Dormir",    "zZ", "DORMIR"),
        ("Btn_Tienda",    "$",  "TIENDA"),
        ("Btn_Logros",    "*",  "LOGROS"),
    };

    // Color lavanda del resto de la UI
    private static readonly Color TextColor = new Color(0.91f, 0.835f, 1f, 1f);

    private void Awake()
    {
        foreach (var (btnName, icon, label) in Config)
            ApplyToButton(btnName, icon, label);
    }

    private static void ApplyToButton(string btnName, string icon, string label)
    {
        var btnGO = GameObject.Find(btnName);
        if (btnGO == null) return;

        // ── Icono ────────────────────────────────────────────────
        var iconT = btnGO.transform.Find("Icon");
        if (iconT != null)
        {
            var rt = iconT.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.sizeDelta        = new Vector2(80, 36);
                rt.anchoredPosition = new Vector2(0, 14);
            }

            var tmp = iconT.GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.enabled       = true;
                tmp.text          = icon;
                tmp.fontSize      = 22;
                tmp.fontStyle     = FontStyles.Bold;
                tmp.color         = TextColor;
                tmp.alignment     = TextAlignmentOptions.Center;
                tmp.raycastTarget = false;
            }

            // Ocultar cualquier Image residual del icon area
            var img = iconT.GetComponent<Image>();
            if (img != null) img.enabled = false;
        }

        // ── Label ────────────────────────────────────────────────
        var labelT = btnGO.transform.Find("Label");
        if (labelT != null)
        {
            var tmp = labelT.GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.enabled   = true;
                tmp.text      = label;
                tmp.fontSize  = 9;
                tmp.fontStyle = FontStyles.Bold;
                tmp.color     = TextColor;
                tmp.alignment = TextAlignmentOptions.Center;
            }
        }
    }
}
