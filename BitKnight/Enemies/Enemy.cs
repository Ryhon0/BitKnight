using Godot;
using System;

public interface Enemy
{
    void Hurt(int health, Knight player);
}
