using Godot;
using System;

public class End : AnimationPlayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Play("Cutscene");
    }
    public void Finish(string a = "")
    {
        GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://Main.tscn");
    }
}
