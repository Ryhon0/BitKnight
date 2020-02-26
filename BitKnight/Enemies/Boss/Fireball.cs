using Godot;
using System;

public class Fireball : KinematicBody2D
{
    public Vector2 Velocity;
    Area2D Area;
    bool Reflected;
    public override void _Ready()
    {
        Area = GetNode<Area2D>("Area");    
    }

    public override void _Process(float delta)
    {
        MoveAndSlide(Velocity, new Vector2(0,1));
        if(IsOnWall()) QueueFree();
    }

    public void Enter(Node2D b)
    {
        if(b is Knight k && !Reflected)
            if(k.HoldingShield)
            {
                Reflected = true;
                Velocity = Velocity * -1;
            }
            else
            {
                k.Hurt();
                QueueFree();
            }

        if(b is Boss boss && Reflected) 
        {
            boss.Hurt(1,null);
            QueueFree();
        };
    }
}
