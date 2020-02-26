using Godot;
using System;

public class GameOver : CanvasLayer
{
    public override void _Ready()
    {
        var t = GetNode<Tween>("Tween");
        var cr = GetNode<ColorRect>("UI");
        var targetcol = cr.Color;
        targetcol.a = 0;
        t.InterpolateProperty(cr, "color", targetcol, cr.Color,0.5f);
        t.Start();

    }

    void Show()
    {
        GetNode<Control>("UI/Control").Visible = true;
        GetNode<Button>("UI/Control/VBoxContainer/Button").GrabFocus();
    }

    void Retry()
    {
        GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://Story.tscn");
    }

    void Menu()
    {
        GetNode<LoadingManager>("/root/LoadingManager").LoadScene("res://Main.tscn");
    }
}
