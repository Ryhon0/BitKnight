using Godot;
using System;

public class StartCutscene : Node2D
{
    Label Skip;
    Timer SkipTimer;
    public override void _Ready()
    {
        Skip = GetNode<Label>("Skip");
        SkipTimer = GetNode<Timer>("SkipTimer");
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Cutscene");
    }
    public override void _Process(float delta)
    {
        Skip.Visible = SkipTimer.TimeLeft != 0;
    }  
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey k && k.Pressed)
            if(SkipTimer.TimeLeft != 0)
                Load();
            else   
                SkipTimer.Start(1f);
    }

    void Load(string a="")
    {
        GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://Story.tscn");
    }
}
