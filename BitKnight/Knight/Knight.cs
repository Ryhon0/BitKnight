using Godot;
using System;
using System.Linq;

public class Knight : KinematicBody2D
{
    public Vector2 Checkpoint;
    Vector2 Up = new Vector2(0, -1);
    public float Gravity = 100;
    public float Speed = 10;
    public float JumpHeight = -50;
    Vector2 Velocity = new Vector2();
    Sprite Sprite;
    AnimationPlayer Animation;

    public bool HasKey;
    int Strenght = 1;
    public int Health = 1;
    public int SoulCount = 0;
    bool CanShield = true;
    public bool HoldingShield;
    bool ShieldTime;
    bool Attacking = false;
    Timer AttackTimer, ShieldTimer, ShieldCooldown, RespawnMoveTimer, RespawnTimer;
    Area2D AttackHitbox;

    Label HP, Souls;
    TextureRect Key;
    bool CanJump = true;
    public override void _Ready()
    {
        Sprite = GetNode<Sprite>("Sprite");
        ShieldTimer = GetNode<Timer>("ShieldTimer");
        Animation = GetNode<AnimationPlayer>("Animation");
        AttackTimer = GetNode<Timer>("AttackTimer");
        AttackHitbox = GetNode<Area2D>("AttackHitbox");
        ShieldCooldown = GetNode<Timer>("ShieldCooldown");
        RespawnTimer = GetNode<Timer>("RespawnTimer");
        RespawnMoveTimer = GetNode<Timer>("RespawnMoveTimer");

        HP = GetNode<Label>("HUD/ColorRect/HP");
        Souls = GetNode<Label>("HUD/ColorRect/Souls");
        Key = GetNode<TextureRect>("HUD/ColorRect/Key");
        GetNode<Control>("HUD/ColorRect").Visible = true;
    }

    public override void _Process(float delta)
    {
        Souls.Text = SoulCount.ToString();
        HP.Text = Health.ToString();
        Key.Visible = HasKey;
    }

    public override void _PhysicsProcess(float delta)
    {
        if(Health <= 0) return;
        if(IsOnFloor()) CanJump = true;
        if(Input.IsActionJustPressed("ui_up") && CanJump && !Attacking && !HoldingShield) Jump();

        if(Input.IsActionJustPressed("attack") && CanJump && !Attacking && !HoldingShield && !ShieldTime) Attack();
        if(Input.IsActionPressed("shield") && CanShield) Shield();
        else UnShield();

        Move(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));
        


        if(ShieldTime || HoldingShield) Animation.Play("Shield");
        ProcessGravity(delta);
    }

    public void Attack()
    {
        AttackTimer.Start(0.5f);
        Attacking = true;
        Animation.Play("Attack");
        AttackHitbox.Position = new Vector2(Mathf.Abs(AttackHitbox.Position.x) * (Sprite.FlipH ? -1 : 1), 0);
    }

    public void AttackTimeout()
    {
        var g = GetTree().GetNodesInGroup("Enemy");
        Node2D[] p = new Node2D[g.Count];
        g.CopyTo(p,0);
        p = p.Where(n => AttackHitbox.GetOverlappingBodies().Contains(n)).ToArray();

        if(p.Any())
            foreach(var e in p) (e as Enemy).Hurt(Strenght,this);

        Attacking = false;
    }

    public void AttackHitboxTimeout()
    {
        ShieldTime = false;   
    }

    public void Move(float vector)
    {
        if(Attacking || HoldingShield) vector = 0;
        
        vector = Mathf.Clamp(vector, -1,1);
        Velocity.x = vector * Speed;

        if(vector == 0)
        {
            if(!Attacking && !HoldingShield)
                if(IsOnFloor())Animation.Play("Idle");
        }
        else
        {
            Sprite.FlipH = vector < 0;
            if(IsOnFloor())Animation.Play("Move");
            Animation.PlaybackSpeed = Mathf.Abs(vector);
        }

        if(!IsOnFloor() && Mathf.Abs(Velocity.y) > 10) Animation.Play("Jump");
    }

    public void Jump()
    {
        CanJump = false;
        Velocity.y = JumpHeight;
    }

    public void ProcessGravity(float delta)
    {
        if(!IsOnFloor()) Velocity += new Vector2(0, Gravity*delta);
        Velocity = MoveAndSlide(Velocity, Up);
    }

    public void Shield()
    {
        if(!HoldingShield)
        {
            ShieldTime = true;
            ShieldTimer.Start(1);
        }
        HoldingShield = true;
    }

    public void UnShield()
    {
        if(!ShieldTime)HoldingShield = false;
    }

    public void ShieldTimeout()
    {
        ShieldTime = false;
    }

    public void Hurt()
    {
        if(HoldingShield)
        {
            CanShield = false;
            HoldingShield = false;
            ShieldTime = false;
            ShieldCooldown.Start(2.5f);
        }
        else Health--;
        Health = Mathf.Max(Health,0);

        if(Health == 0)
        {
            Animation.Play("Death");
            RespawnTimer.Start(1);
        }
    }

    void ShieldCooldownTimeout()
    {
        HoldingShield = false;
        ShieldTime = false;
        CanShield = true;
    }

    void Respawn()
    {
        if(SoulCount == 0 && Health == 0)
        {
            GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://Knight/GameOver.tscn");
        }
        else
        {
            Animation.PlayBackwards("Death");
            GlobalPosition = Checkpoint - new Vector2(0,8);
            RespawnMoveTimer.Start(1);
        }
    }

    void RespawnMove()
    {
        Health = SoulCount;
        SoulCount = 0;
    }
}
