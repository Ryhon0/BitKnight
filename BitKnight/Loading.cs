using Godot;
using System;

public class Loading : ColorRect
{
    ProgressBar LoadingBar;
    LoadingManager loadingManager;
    Label Count;
    public override void _Ready()
    {
        LoadingBar = GetNode<ProgressBar>("ProgressBar");
        loadingManager = GetNode<LoadingManager>("/root/LoadingManager");
    }

    public override void _Process(float delta)
    {
        LoadingBar.MaxValue = loadingManager.ToLoad;
        LoadingBar.Value = loadingManager.Progres;
    }
}
