using Godot;
using System;

public class Checkpoint : Node2D
{
    bool used;
    public void EnterCheckpoint(Node2D p)
    {
        if(p is Knight k && !used)
        {
            k.Checkpoint = GlobalPosition;
            used = true;               
        }
    }
}
