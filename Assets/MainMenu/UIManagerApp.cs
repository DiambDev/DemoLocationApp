using UnityEngine;
using UnityEngine.UIElements;
public class UIManagerApp : MonoBehaviour
{
     public static UIManagerApp Instance;

    [SerializeField] private UIDocument uiDocument;
    private VisualElement root;

    void Awake()
    {
        Instance = this;
        root = uiDocument.rootVisualElement;
    }

    public void LoadScreen(string screenName)
    {
        root.Q("content-area")?.Clear();

        var visualTree = Resources.Load<VisualTreeAsset>($"UI/Screens/{screenName}");
        var screen = visualTree.CloneTree();

        root.Q("content-area").Add(screen);
    }
}
