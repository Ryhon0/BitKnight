using Godot;
using System;

public class Door : Node2D
{
    bool Opened = false;
    public void Enter(Node2D p)
    {
        if(p is Knight k && k.HasKey && !Opened)
        {
            k.HasKey = false;
            GetNode<Timer>("OpenTimer").Start(1f);
            GetNode<AnimationPlayer>("Animation").Play("Open");
            Opened = true;
        }
    }

    public void Open()
    {
        GetNode<Node2D>("StaticBody2D").QueueFree();
    }
}
