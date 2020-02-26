using Godot;
using System;

public class Key : Node2D
{
    public void BodyEnter(Node2D b)
    {
        if(b is Knight k)
        {
            QueueFree();
            k.HasKey = true;
        }
    }   
}
