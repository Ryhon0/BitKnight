using Godot;
using System;
using System.Linq;

public class Goblin : KinematicBody2D, Enemy
{
    Vector2 Up = new Vector2(0, -1);
    public float Gravity = 3;
    public float Speed = 25f;
    public float JumpHeight = -50;
    Vector2 Velocity = new Vector2();
    Sprite Sprite;
    AnimationPlayer Animation;
    bool Attacking = false;
    bool CanAttack = true;
    Timer AttackTimer, AttackCooldown;
    Area2D AttackHitbox;
    bool SoulColected;
    Timer KnockBackTimer;
    bool FlyR;

    [Export]
    int Health = 3;
    Vector2 SpawnPos;
    [Export]
    string PathToArea = "..";
    Area2D PatrolArea;
    public override void _Ready()
    {
        PatrolArea = GetNode<Area2D>(PathToArea);
        Sprite = GetNode<Sprite>("Sprite");
        Animation = GetNode<AnimationPlayer>("Animation");
        AttackTimer = GetNode<Timer>("AttackTimer");
        AttackCooldown = GetNode<Timer>("AttackCooldown");
        AttackHitbox = GetNode<Area2D>("AttackHitbox");
        SpawnPos = GlobalPosition;
        KnockBackTimer = GetNode<Timer>("KnockBackTimer");
    }

    public void Hurt(int damage, Knight player)
    {
        Health -= damage;
        if(Health <= 0)
        {
            Animation.Play("Death");
            if(!SoulColected)
            {
                var soul = (GD.Load("res://Enemies/Soul.tscn") as PackedScene).Instance() as Soul;
                GetParent().AddChild(soul);
                soul.GlobalPosition = GlobalPosition;
                soul.Target = player;
                SoulColected = true;
            }
        }
        else
        {
            Jump();
            KnockBackTimer.Start(0.75f);
            FlyR = player.GlobalPosition < GlobalPosition;
        }
        AttackTimer.Stop();
        CanAttack = true;
        Attacking = false;
    }

    public override void _Process(float delta)
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        if(KnockBackTimer.TimeLeft == 0)
        {
            var g = GetTree().GetNodesInGroup("Player");
            Node2D[] p = new Node2D[g.Count];
            g.CopyTo(p,0);
            p = p.Where(n => PatrolArea.GetOverlappingBodies().Contains(n) && (n as Knight).Health > 0).ToArray();

            if(p.Any())
            {
                if(p.FirstOrDefault().GlobalPosition.DistanceTo(GlobalPosition) > 10 && (p.FirstOrDefault() as Knight).Health != 0)Move(p.FirstOrDefault().GlobalPosition < GlobalPosition ? -1 : 1);
                else Move(0);
            }
            else
            {
                if(GlobalPosition.DistanceTo(SpawnPos) > 2)
                    Move(SpawnPos.x > GlobalPosition.x ? 1 : -1);
                else Move(0);
            }

            p = p.Where(n => n.GlobalPosition.DistanceTo(GlobalPosition) < 10).ToArray();

            if(p.Any() && IsOnFloor() && CanAttack && !Attacking && Health > 0) Attack();
        }
        else
        {
            Move(FlyR ? 1 : -1);
        }

        ProcessGravity(delta);
    }

    public void Attack()
    {
        AttackTimer.Start(0.5f);
        Attacking = true;
        CanAttack = false;
        Animation.Play("Attack");
        AttackHitbox.Position = new Vector2(Mathf.Abs(AttackHitbox.Position.x) * (Sprite.FlipH ? -1 : 1), 0);
        AttackCooldown.Start(1.5f);
    }

    public void AtkCooldown()
    {
        CanAttack = true;
    }

    public void AttackTimeout()
    {
        Attacking = false;
        var g = GetTree().GetNodesInGroup("Player");
        Node2D[] p = new Node2D[g.Count];
        g.CopyTo(p,0);
        p = p.Where(n => AttackHitbox.GetOverlappingBodies().Contains(n)).ToArray();

        if(p.Any())
            foreach(var e in p) (e as Knight).Hurt();
    }

    public void Move(float vector)
    {
        if(Attacking || Health <= 0) vector = 0;
        
        vector = Mathf.Clamp(vector, -1,1);
        Velocity.x = vector * Speed;

        if(vector == 0)
        {
            if(!Attacking && IsOnFloor() && Health > 0)
                Animation.Play("Idle");
        }
        else
        {
            Sprite.FlipH = vector < 0;
            if(FlyR && KnockBackTimer.TimeLeft != 0) Sprite.FlipH = !Sprite.FlipH;
            if(IsOnFloor())Animation.Play("Move");
            Animation.PlaybackSpeed = Mathf.Abs(vector);
        }

        if(!IsOnFloor() && Mathf.Abs(Velocity.y) > 10 ) Animation.Play("Jump");
    }

    public void Jump()
    {
        Velocity.y = JumpHeight;
    }

    public void ProcessGravity(float delta)
    {
        Velocity = MoveAndSlide(Velocity, Up);
        Velocity += new Vector2(0, Gravity);
    }
}
