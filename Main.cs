using System;
using Godot;
using System.Collections.Generic;

public class Main : Node2D
{

    // # loads the stone scene
    //     var stone = preload("res://Scenes/Stone.tscn")
    PackedScene stone = (PackedScene)GD.Load("res://Scenes/Stone.tscn");


    // #called when there's an input
    // func _input(event):

    public override void _Input(InputEvent @event)
    {
        // 	#if there's any mouse button press
        // 	if event is InputEventMouseButton and event.is_pressed():
        if (@event is InputEventMouseButton && @event.IsPressed())
        {
            // # makes an instance of the stone scene
            // 		var s = stone.instance()
            Stone s = (Stone)stone.Instance();
            // # initializes the stone at the mouse position
            // 		s.initialize(get_global_mouse_position())
            s.initialize(GetGlobalMousePosition());
            // # adds the stone to the current scene
            // 		get_tree().current_scene.add_child(s)
            GetTree().CurrentScene.AddChild(s);
        }
    }

}