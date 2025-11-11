using System;
using System.Threading.Tasks;
using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;


public class MapController : BasePanel
{
    [Header("Components")]
    [SerializeField] private GameObject[] mapcomponents;
    [SerializeField] private GameObject pnlMap;
    [Header("Buttons")]
    [SerializeField] private Button btn;
    void Start()
    {
        btn.onClick.AddListener(() => active?.Invoke(this));
    }

    public override void Show()
    {
        for(int i = 0; i < mapcomponents.Length; i++)
        {
            mapcomponents[i].SetActive(true) ;
        }
        pnlMap.SetActive(true);
    }
    public override void Hide()
    {
        for(int i = 0; i < mapcomponents.Length; i++)
        {
            mapcomponents[i].SetActive(false) ;
        }
        pnlMap.SetActive(false);
    }

}
