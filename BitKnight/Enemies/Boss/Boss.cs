using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Boss : KinematicBody2D, Enemy
{
    int Health = 10;
    Vector2 Start;
    PackedScene Fireball;
    Timer FireballTimer, AttackCooldown, TeleportCooldown, TeleportMove;
    AnimationPlayer Animation;
    Area2D Area;
    Sprite Sprite;
    public override void _Ready()
    {
        Start = GlobalPosition;
        Animation = GetNode<AnimationPlayer>("AnimationPlayer");
        Area = GetNode<Area2D>("..");
        FireballTimer = GetNode<Timer>("FireballTimer");
        AttackCooldown = GetNode<Timer>("AttackCooldown");
        TeleportCooldown = GetNode<Timer>("TeleportCooldown");
        TeleportMove  = GetNode<Timer>("TeleportMove");
        Fireball = GD.Load("res://Enemies/Boss/Fireball.tscn") as PackedScene;
        Sprite = GetNode<Sprite>("Sprite");
    }

    public override void _Process(float delta)
    {
        if(Health <= 0) return;

        var bs = Area.GetOverlappingBodies();
        var a = new Node2D[bs.Count];
        bs.CopyTo(a,0);
        a = a.Where(b=>b is Knight).ToArray();
        if(TeleportCooldown.TimeLeft == 0)
        {
            if(a.Any())
            {
                TeleportMove.Start(1f);
                TeleportCooldown.Start(4f);
                Animation.Play("Teleport");
                AttackCooldown.Start(1.6f);
            }
            else if(GlobalPosition != Start)
            {
                TeleportMove.Start(1f);
                TeleportCooldown.Start(4f);
                Animation.Play("Teleport");
                AttackCooldown.Start(1.6f);
            }
        }
        else
        {
            if(AttackCooldown.TimeLeft == 0)
            {
                TeleportCooldown.Start(3f);
                AttackCooldown.Start(3f);
                Animation.Play("Attack");
                FireballTimer.Start(1f);
            }
        }
    }

    public void Hurt(int h, Knight player)
    {
        Health -= h;

        if(Health == 0 && Animation.CurrentAnimation != "Death")
        {
            Animation.Play("Death");
            FireballTimer.Stop();
            AttackCooldown.Stop();
            TeleportCooldown.Stop();
            TeleportMove.Stop();
        }
    }

    public void FBHurt()
    {
        Health --;

        if(Health == 0 && Animation.CurrentAnimation != "Death")
        {
            Animation.Play("Death");
            FireballTimer.Stop();
            AttackCooldown.Stop();
            TeleportCooldown.Stop();
            TeleportMove.Stop();
        }
    }

    public void Teleport()
    {
        var bs = Area.GetOverlappingBodies();
        var a = new Node2D[bs.Count];
        bs.CopyTo(a,0);
        a = a.Where(b=>b is Knight).ToArray();
        if(a.Any())
        {
            var p = new List<Vector2>{Start, Start + new Vector2(20,0), Start + new Vector2(-20,0) };
            p.RemoveAll(t => t.DistanceTo(GlobalPosition) < 2);
            var r = new Random();
            GlobalPosition = p[r.Next(0,2)];
            Sprite.FlipH = a.First().GlobalPosition < GlobalPosition;
        }
        else
        {
            Sprite.FlipH = true;
            GlobalPosition = Start;
        }
    }

    public void Win(string a)
    {
        if(a == "Death")
        {
            GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://End.tscn");
        }
    }

    public void Shoot()
    {
        var bs = Area.GetOverlappingBodies();
        var a = new Node2D[bs.Count];
        bs.CopyTo(a,0);
        a = a.Where(b=>b is Knight).ToArray();

        var f = Fireball.Instance() as Fireball;
        GetParent().AddChild(f);
        f.GlobalPosition = GlobalPosition;
        f.Velocity = new Vector2(Sprite.FlipH ? -20 : 20 , 0);
    }
}
