using UnityEngine;
using UnityEngine.UI;

public class UIRouteButtons : MonoBehaviour
{
    [SerializeField] private RouteController routeController;
    public Button originButton;
    public Button destinationButton;
    public Button drawButton;

    void Start()
    {
        originButton.onClick.AddListener(routeController.SetOrigin);
        destinationButton.onClick.AddListener(routeController.SetDestination);
        drawButton.onClick.AddListener(routeController.DrawRoute);
    }
}
