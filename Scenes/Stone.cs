using System;
using Godot;
using System.Collections.Generic;

public class Stone : KinematicBody2D
{

    // #the gravity value
    // var gravity = 35
    float gravity = 35;


    // #the motion vector
    // #we'll use it to move our stone with move_and_slide
    // var motion = Vector2.ZERO
    public Vector2 motion = Vector2.Zero;

    // #so the stone won't accelerate forever
    // var max_speed = 450
    float max_speed = 450;

    // var max_speed_in_water = 200
    float max_speed_in_water = 200;

    // func _physics_process(delta):

    public override void _PhysicsProcess(float delta)
    {
        // 	#at each frame add gravity to our motion vector
        // 	#clamp the motion value to the speed
        // 	#move the stone with move_and_slide
        // 	motion.y += gravity
        motion.y += gravity;
        // 	motion.y = clamp(motion.y, -max_speed, max_speed)
        motion.y = Mathf.Clamp(motion.y, -max_speed, max_speed);
        // 	motion = move_and_slide(motion)
        motion = MoveAndSlide(motion);
    }

    // #initializes the stone at a set position
    // func initialize(pos):
    public void initialize(Vector2 pos)
    {
        // 	#set the stone's position
        // 	global_position = pos
        GlobalPosition = pos;
    }

    // func in_water():
    public void in_water()
    {
        // 	#this function will be called when the stone enters the water
        // 	#we will change the gravity and the max speed
        // 	gravity = gravity / 3
        gravity = gravity / 3;
        // 	max_speed = max_speed_in_water
        max_speed = max_speed_in_water;
    }
}