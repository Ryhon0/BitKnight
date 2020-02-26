using Godot;
using System;

public class Soul : KinematicBody2D
{
    public Node2D Target;
    Vector2 Velocity = new Vector2(0,-15);
    Timer KillTimer;
    Particles2D Particle;
    float MoveSpeed = 30;
    bool Used;
    public override void _Ready()
    {
        KillTimer = GetNode<Timer>("KillTimer");
        Particle = GetNode<Particles2D>("Particle");
    }

    public override void _Process(float delta)
    {
        if(Target == null) return;
        if(GlobalPosition.DistanceTo(Target.GlobalPosition) < 5 && KillTimer.TimeLeft == 0)
        {
            KillTimer.Start(0.3f);
            Particle.Emitting = false;
            if(!Used && Target is Knight k)
            {
                Used = true;
                k.SoulCount++;
            }
        }

        Velocity += new Vector2(Target.GlobalPosition.x > GlobalPosition.x ? MoveSpeed*delta : -MoveSpeed*delta,
                                Target.GlobalPosition.y > GlobalPosition.y ? MoveSpeed*delta : -MoveSpeed*delta);

        MoveAndSlide(Velocity, new Vector2(0,1));        
    }

    void Destroy()
    {
        QueueFree();
    }
}
