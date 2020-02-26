using Godot;
using System;
using System.Linq;

public class Archer : KinematicBody2D, Enemy
{    Sprite Sprite;
    AnimationPlayer Animation;
    int Health = 1;
    Timer ShootTimer, AttackCooldown;
    PackedScene Arrow;
    bool SoulColected;
    public override void _Ready()
    {
        ShootTimer = GetNode<Timer>("ShootTimer");
        AttackCooldown = GetNode<Timer>("AttackCooldown");
        Sprite = GetNode<Sprite>("Sprite");
        Animation = GetNode<AnimationPlayer>("Animation");

        Arrow = (PackedScene)GD.Load("res://Enemies/Archer/Arrow.tscn");
    }

    public void Hurt(int damage, Knight player)
    {
        Health -= damage;
        if(Health <= 0 && !SoulColected)
        {
                var soul = (GD.Load("res://Enemies/Soul.tscn") as PackedScene).Instance() as Soul;
                GetParent().AddChild(soul);
                soul.GlobalPosition = GlobalPosition;
                soul.Target = player;
                Animation.Play("D");
                ShootTimer.Stop();
                SoulColected = true;
        }
    }

    public override void _Process(float delta)
    {
        var g = GetTree().GetNodesInGroup("Player");
        Node2D[] p = new Node2D[g.Count];
        g.CopyTo(p,0);
        p = p.Where(n => (n as Knight).Health > 0).ToArray();
        p = p.Where(n => n.GlobalPosition.DistanceTo(GlobalPosition) < 40).ToArray();

        if(p.Any() && AttackCooldown.TimeLeft == 0 && Health > 0)
        {
            var k = p.FirstOrDefault();

            Sprite.FlipH = k.GlobalPosition < GlobalPosition;
            AttackCooldown.Start(3f);
            ShootTimer.Start(1.5f);
            Animation.Play("A");
        }
    }

    public void ShootTimeout()
    {
        Arrow a = Arrow.Instance() as Arrow;
        GetParent().AddChild(a);
        a.GlobalPosition =  Sprite.FlipH ? new Vector2(-5,0) + GlobalPosition :
                            new Vector2(5,0) + GlobalPosition;
        a.Velocity = (new Vector2(20, 0) * (Sprite.FlipH ? -1 : 1));
    }
}
