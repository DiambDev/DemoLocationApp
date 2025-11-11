using UnityEngine;
using UnityEngine.UI;
public class ReportController : BasePanel
{
     [Header("Components")]
    [SerializeField] private GameObject pnlMap;
    [Header("Buttons")]
    [SerializeField] private Button btn;
      void Start()
    {
        btn.onClick.AddListener(() => active?.Invoke(this));
    }

   public override void Show()
    {
        pnlMap.SetActive(true);
    }
    public override void Hide()
    {
        pnlMap.SetActive(false);
    }
}
