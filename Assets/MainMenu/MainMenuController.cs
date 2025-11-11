using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public UIDocument ui;

    void Start()
    {
        if (ui == null)
        {
            Debug.LogError("UIDocument no asignado en el inspector!");
            return;
        }

        var root = ui.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("rootVisualElement es NULL. El UIDocument no cargó.");
            return;
        }

        // --- Botones del Action Area ---
        Button btnEmergencia = root.Q<Button>("btnEmergencia");
        Button btnRuta = root.Q<Button>("btnRuta");

        // --- Botones del Mapa ---
        Button btnZoomPlus = root.Q<Button>("btnZoomPlus");
        Button btnZoomMinus = root.Q<Button>("btnZoomMinus");

        // --- Botón dinámico ---
        Button btnDynamic = root.Q<Button>("btnDynamic");


        // Validación general
        ValidateButton(btnEmergencia, "btnEmergencia");
        ValidateButton(btnRuta, "btnRuta");
        ValidateButton(btnZoomPlus, "btnZoomPlus");
        ValidateButton(btnZoomMinus, "btnZoomMinus");
        ValidateButton(btnDynamic, "btnDynamic");


        // --- Eventos asignados ---
        btnEmergencia.clicked += () => Debug.Log("Emergencia!");
        btnRuta.clicked += () => Debug.Log("Ver Ruta!");

        btnZoomPlus.clicked += () => Debug.Log("Zoom +");
        btnZoomMinus.clicked += () => Debug.Log("Zoom -");

        btnDynamic.clicked += () => Debug.Log("Ubicación pulsada");
    }


    /// <summary>
    /// Valida que el botón exista en el UXML
    /// </summary>
    void ValidateButton(Button btn, string name)
    {
        if (btn == null)
            Debug.LogError($"No se encontró el botón: {name}");
    }
}
