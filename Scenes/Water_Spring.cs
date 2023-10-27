using System;
using Godot;
using System.Collections.Generic;

public class Water_Spring : Node2D
{

    // #the spring's current velocity
    // var velocity = 0
    float velocity = 0;

    // #the force being applied to the spring
    // var force = 0
    float force = 0;

    // #the current height of the spring
    // var height = 0
    float height = 0;

    // #the natural position of the spring
    // var target_height = 0
    float target_height = 0;

    // onready var collision = $Area2D/CollisionShape2D
    CollisionShape2D collision = null;
    void r_collision()
    {
        collision = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
    }

    // # the index of this spring
    // #we will set it on initialize
    // var index = 0
    int index = 0;

    // #how much an external object movement will affect this spring
    // var motion_factor = 0.015
    float motion_factor = 0.015f;

    // #the last instance this spring collided with
    // #we check so it won't collide twice
    // var collided_with = null
    Node2D collided_with = null;

    // #we will trigger this signal to call the splash function
    // #to make our wave move!
    // signal splash
    [Signal]
    delegate void splash(int index, float speed);

    // func water_update(spring_constant, dampening):
    void water_update(int spring_constant, float dampening)
    {
        // 	## This function applies the hooke's law force to the spring!!
        // 	## This function will be called in each frame
        // 	## hooke's law ---> F =  - K * x

        // 	#update the height value based on our current position
        // 	height = position.y
        height = Position.y;

        // 	#the spring current extension
        // 	var x = height - target_height
        var x = height - target_height;

        // 	var loss = -dampening * velocity
        var loss = -dampening * velocity;

        // 	#hooke's law:
        // 	force = - spring_constant * x + loss
        force = -spring_constant * x + loss;

        // 	#apply the force to the velocity
        // 	#equivalent to velocity = velocity + force
        // 	velocity += force
        velocity += force;

        // 	#make the spring move!
        // 	position.y += velocity
        Position = new Vector2(Position.x, Position.y + velocity);
        // 	pass
    }

    // func initialize(x_position,id):
    void initialize(float x_position, int id)
    {
        // 	height = position.y
        height = Position.y;
        // 	target_height = position.y
        target_height = Position.y;

        // 	velocity = 0
        velocity = 0;
        // 	position.x = x_position
        Position = new Vector2(x_position, Position.y);
        // 	index = id
        index = id;
    }

    // func set_collision_width(value):
    void set_collision_width(float value)
    {
        // 	#this function will set the collision shape size of our springs
        // 	var extents = collision.shape.get_extents()
        var extents = (collision.Shape as RectangleShape2D).Extents;

        // 	#the new extents will mantain the value on the y width
        // 	#the "value" variable is the space between springs, which we already have
        // 	var new_extents = Vector2(value/2, extents.y)
        var new_extents = new Vector2(value / 2, extents.y);

        // 	#set the new extents
        // 	collision.shape.set_extents(new_extents)
        (collision.Shape as RectangleShape2D).Extents = new_extents;
        // 	pass
    }


    // func _on_Area2D_body_entered(body):
    void _on_Area2D_body_entered(Node2D body)
    {

        // 	#called when a body collides with a spring

        // 	#if the body already collided with the spring, then do not collide
        // 	if body == collided_with:
        if (body == collided_with)
        {
            // return
            return;
        }

        // 	#the body is the last thing this spring collided with
        // 	collided_with = body
        collided_with = body;

        // 	#we multiply the motion of the body by the motion factor
        // 	#if we didn't the speed would be huge, depending on your game
        // 	var speed = body.motion.y * motion_factor

        var speed = (body as PhysicsTestMotionResult).Motion.y * motion_factor;

        // 	#emit the signal "splash" to call the splash function, at our water body script
        // 	emit_signal("splash",index,speed)
        EmitSignal(nameof(splash), index, speed);
        // 	pass # Replace with function body.

    }
}