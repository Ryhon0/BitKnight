using Godot;
using System;

public class LevelTransition : CanvasLayer
{
    Tween Tween;
    ColorRect Rect;
    public override void _Ready()
    {
        Rect = GetNode<ColorRect>("ColorRect");
        Tween = GetNode<Tween>("Tween");
        Rect.Visible = true;
        var newc = Rect.Color;
        newc.a = 0;
        Tween.InterpolateProperty(Rect,"color",Rect.Color,newc,0.5f);
        Tween.Start();
    }


    public void Transition()
    {
        var newc = Rect.Color;
        newc.a = 1;
        Tween.InterpolateProperty(Rect,"color",Rect.Color,newc,0.5f);
        Tween.Start();
    }
}
