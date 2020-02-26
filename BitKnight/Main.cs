using Godot;
using System;

public class Main : Control
{
    Tween TransparencyTween;
    ColorRect Transition;
    bool DontSkip;

    public override void _Ready()
    {
        Transition = GetNode<ColorRect>("Transition");
        TransparencyTween = GetNode<Tween>("TransparencyTween");
        var targetcol = Transition.Color;
        targetcol.a = 0;
        GetNode<Button>("Main/VBoxContainer/Start").GrabFocus();
        TransparencyTween.InterpolateProperty(Transition, "color", Transition.Color,targetcol,0.5f);
        TransparencyTween.Start();
    }

    public void Start()
    {
        TransitionTransparency(GetNode<Control>("Play") ,GetNode<Control>("Main"), GetNode<Button>("Play/VBoxContainer/Button"));
    }

    public void Controlls()
    {
        TransitionTransparency(GetNode<Control>("Controlls") ,GetNode<Control>("Main"), GetNode<Button>("Controlls/Back"));
    }

    public void Back()
    {
        TransitionTransparency(GetNode<Control>("Main") ,GetNode<Control>("Controlls"), GetNode<Button>("Main/VBoxContainer/Start"));
    }
    public void BackPlay()
    {
        TransitionTransparency(GetNode<Control>("Main") ,GetNode<Control>("Play"), GetNode<Button>("Main/VBoxContainer/Start"));
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey && !DontSkip)
        {
            TransparencyTween.Stop(Transition);
            Transition.Color = new Color(199f/255,240f/255,216f/255,0);
            Timer t = GetNode<Timer>("TransparencyTimer");
            if(t.TimeLeft !=0)
            {
                t.Stop();
                TransparencyTimer();
            }
        }
    }

    Control nextcontrol, currentcontrol;
    Button nextbutton;
    public void TransitionTransparency(Control control, Control currentcontrol , Button button)
    {
        this.currentcontrol = currentcontrol;
        nextcontrol = control;
        nextbutton = button;

        Transition.Color = new Color(199/1,240/1,216/1,1);
        TransparencyTween.InterpolateProperty(Transition, "color", new Color(199f/255,240f/255,216f/255,0),new Color(199f/255,240f/255,216f/255,1),0.5f);
        TransparencyTween.InterpolateProperty(Transition, "color",new Color(199f/255,240f/255,216f/255,1), new Color(199f/255,240f/255,216f/255,0),0.5f, delay: 0.5f);
        TransparencyTween.Start();
        GetNode<Timer>("TransparencyTimer").Start(0.5f);
    }

    public void TransparencyTimer()
    {
        currentcontrol.Visible = false;
        nextcontrol.Visible = true;
        nextbutton.GrabFocus();
    }

    string LevelPath;
    public void LoadLevel(string path)
    {
        LevelPath = path;
        Transition.Color = new Color(67f/255,82f/255,61f/255,0);
        TransparencyTween.InterpolateProperty(Transition, "color",new Color(67f/255,82f/255,61f/255,0), new Color(67f/255,82f/255,61f/255,1),0.5f);
        TransparencyTween.Start();
        GetNode<Timer>("LoadLevelTimer").Start(0.5f);
    }

    public void LoadLevelTimedOut()
    {
        GetNode<LoadingManager>("/root/LoadingManager").LoadScene(LevelPath);
    }

    public void StoryMode()
    {
        if(GetNode<Timer>("LoadLevelTimer").TimeLeft == 0)
        {
            DontSkip = true;
            LoadLevel("res://StartCutscene.tscn");
        }
    }

    public void Exit()
    {
        GetTree().Quit();
    }
}