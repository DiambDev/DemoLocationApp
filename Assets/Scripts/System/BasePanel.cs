using UnityEngine;
using System;
public class BasePanel : MonoBehaviour,IBasePanel
{
    public Action<BasePanel> active;
    public virtual void Hide()
    {

    }
    public virtual void Show()
    {

    }
}
