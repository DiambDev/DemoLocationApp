using UnityEngine;
using UnityEngine.UIElements;

public class NavBarController : MonoBehaviour
{
    public UIDocument ui;

    void OnEnable()
    {
        Debug.Log("===== NAVBAR DEBUG =====");

        if (ui == null)
        {
            Debug.LogError("UI DOCUMENT ES NULL — NO LE HAS ASIGNADO NADA EN EL INSPECTOR");
            return;
        }

        if (ui.visualTreeAsset == null)
        {
            Debug.LogError("VISUAL TREE ASSET ES NULL — EL UIDOCUMENT NO TIENE UXML");
        }
        else
        {
            Debug.Log("UXML ASSET: " + ui.visualTreeAsset.name);
        }

        var root = ui.rootVisualElement;
        Debug.Log("ROOT: " + (root != null));

        var navMapa = root.Q<Button>("navMapa");
        var navContactos = root.Q<Button>("navContactos");
        var navRegistro = root.Q<Button>("navRegistro");
        var navSettings = root.Q<Button>("navSettings");

        Debug.Log("navMapa: " + navMapa);
        Debug.Log("navContactos: " + navContactos);
        Debug.Log("navRegistro: " + navRegistro);
        Debug.Log("navSettings: " + navSettings);

        navMapa.clicked += () => UIManagerApp.Instance.LoadScreen("Mapa");
        navContactos.clicked += () => UIManagerApp.Instance.LoadScreen("Contactos");
        navRegistro.clicked += () => UIManagerApp.Instance.LoadScreen("Registro");
        navSettings.clicked += () => UIManagerApp.Instance.LoadScreen("Settings"); 
        Debug.Log("ASSET UXML: " + ui.visualTreeAsset);
    }
}
