using UnityEngine;
using System.Collections.Generic;
using System;
using SGUI;

public interface IETGModMenu
{

    void Start();
    void Update();
    void OnGUI();
    void OnDestroy();
    void OnOpen();
    void OnClose();

}

public abstract class ETGModMenu : IETGModMenu
{
    public SGroup GUI { get; protected set; }
    public abstract void Start();
    public virtual void Update() { }
    public virtual void OnGUI() { }
    public virtual void OnDestroy() { }
    public virtual void OnOpen()
    {
    }
    public virtual void OnClose()
    {
    }
}

public sealed class ETGModNullMenu : IETGModMenu
{
    public void Start() { }
    public void Update() { }
    public void OnGUI() { }
    public void OnDestroy() { }
    public void OnOpen() { }
    public void OnClose() { }
}