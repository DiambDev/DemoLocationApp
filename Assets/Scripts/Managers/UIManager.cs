using System;
using UnityEngine;

public interface IBasePanel 
{
    public void Hide();
    public void Show();

}
public class UIManager : MonoBehaviour
{
    public BasePanel[] bases;
    public IBasePanel currentBase;

    void Start()
    {
        currentBase = bases[0];
        print(bases[0].name);
        SetPanel(currentBase);
    }
    void OnEnable()
    {
        for (int i = 0; i < bases.Length; i++)
        {
            bases[i].active += SetPanel;
        }
    }
    void OnDisable()
    {
        for (int i = 0; i < bases.Length; i++)
        {
            bases[i].active -= SetPanel;
        }
    }
    public void SetPanel(IBasePanel panel)
    {
        if (currentBase == panel)
        {            
            currentBase.Show();
        }
        else
        {
            currentBase?.Hide();

            currentBase = panel;

            currentBase.Show();
        }
       
      
    }    
}
