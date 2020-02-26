using Godot;
using Godot.Collections;
using System;

public class LoadingManager : Node
{
    ResourceInteractiveLoader loader;
    int wait_frames = 1;
    int time_max = 100;

    public int Progres=0;
    public int ToLoad=100;
    string current = "";

    Dictionary<string, Resource> AlreadyLoaded = new Dictionary<string, Resource>(); 

    public override void _Ready()
    {
        //LoadingScreen = (PackedScene)GD.Load("res://Loading.tscn");
    }

    public void LoadScene(string path)
    {
        if(AlreadyLoaded.ContainsKey(path))
        {
            var resource = AlreadyLoaded[path];
            GetTree().ChangeSceneTo(resource as PackedScene);
            return;
        }
        current = path;
        var ls = (PackedScene)GD.Load("res://Loading.tscn");
        Loading l = ls.Instance() as Loading;
        GetTree().CurrentScene.AddChild(l);


        loader = ResourceLoader.LoadInteractive(path);
        if(loader == null)
        {
            return;
        }
        SetProcess(true);

        wait_frames = 1;
    }

    public override void _Process(float delta)
    {
        if(loader == null)
        {
            SetProcess(false);
            return;
        }

        if(wait_frames < 0)
        {
            wait_frames -= 0;
            return;
        }

        var t = OS.GetTicksMsec();
        while(OS.GetTicksMsec() < t + time_max)
        {
            var err = loader.Poll();

            if(err == Error.FileEof)
            {
                Progres = ToLoad;
                var resource = loader.GetResource();
                AlreadyLoaded.Add(current, resource);
                loader = null;
                GetTree().ChangeSceneTo(resource as PackedScene);
                break;
            }
            else if(err == Error.Ok)
            {
                Progres = loader.GetStage();
                ToLoad = loader.GetStageCount();
            }
            else
            {
                loader = null;
                break;
            }
        }
    }
}