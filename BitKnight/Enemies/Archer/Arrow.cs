using Godot;
using System;

public class Arrow : KinematicBody2D, Enemy
{
    public Vector2 Velocity;
    Sprite Sprite;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite>("Sprite");
    }
    public override void _Process(float delta)
    {
        Sprite.FlipH = Velocity > new Vector2();
        MoveAndSlide(Velocity,new Vector2(0,1));

        if(IsOnWall()) QueueFree();
    }

    public void Hit(Node2D n)
    {
        if(n is Knight k)
        {
            k.Hurt();
            QueueFree();
        }
    }

    public void Destroy()
    {
        QueueFree();
    }

    public void Hurt(int health, Knight player)
    {
        QueueFree();
    }
}
