using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

public class Water_Body : Node2D
{

    // #spring factor, dampening factor and spread factor
    // #spread factor dictates how much the waves will spread to their neighboors
    // export var k = 0.015
    [Export]
    public float k = 0.015f;
    // export var d = 0.03
    [Export]
    public float d = 0.03f;
    // export var spread = 0.2
    [Export]
    public float spread = 0.2f;

    // #distance in pixel between each spring
    // export var distance_between_springs = 32
    [Export]
    public int distance_between_springs = 32;

    // #number of springs in the scene
    // export var spring_number = 6
    [Export]
    public int spring_number = 6;

    // #the spring array
    // var springs = []
    List<Water_Spring> springs = new List<Water_Spring>();
    // var passes = 12
    int passes = 12;


    // #total water body lenght
    // var water_lenght = distance_between_springs * spring_number
    int water_lenght = 0;
    void r_water_lenght()
    {
        water_lenght = distance_between_springs * spring_number;
    }

    // #spring scene reference
    // onready var water_spring = preload("res://Scenes/Water_Spring.tscn")
    PackedScene water_spring = GD.Load<PackedScene>("res://Scenes/Water_Spring.tscn");


    // #the body of water depth
    // export var depth = 1000
    [Export]
    public int depth = 1000;

    // var target_height = global_position.y
    float target_height = 0;
    void r_target_height()
    {
        target_height = GlobalPosition.y;
    }

    // #the position of the bottom of our body of water
    // var bottom = target_height + depth
    float bottom = 0;
    void r_bottom()
    {
        bottom = target_height + depth;
    }

    // #reference to our polygon2D
    // onready var water_polygon = $Water_Polygon
    Polygon2D water_polygon = null;
    void r_water_polygon()
    {
        water_polygon = GetNode<Polygon2D>("Water_Polygon");
    }

    // #reference to our water border
    // onready var water_border = $Water_Border
    Path2D water_border = null;
    void r_water_border()
    {
        water_border = GetNode<Path2D>("Water_Border");
    }


    // export var border_thickness = 1.1
    [Export]
    public float border_thickness = 1.1f;

    // #reference to the water body area and its collision shape
    // onready var collisionShape = $Water_Body_Area/CollisionShape2D
    CollisionShape2D collisionShape = null;
    void r_collisionShape()
    {
        collisionShape = GetNode<CollisionShape2D>("Water_Body_Area/CollisionShape2D");
    }

    // onready var water_body_area = $Water_Body_Area
    Area2D water_body_area = null;
    void r_water_body_area()
    {
        water_body_area = GetNode<Area2D>("Water_Body_Area");
    }

    // #reference to the particle we just created
    // onready var splash_particle = preload("res://Scenes/splash_particles.tscn")
    PackedScene splash_particle = GD.Load<PackedScene>("res://Scenes/splash_particles.tscn");

    // func _physics_process(delta):
    public override void _PhysicsProcess(float delta)
    {
        // 	#moves all the springs accordingly
        // 	for i in springs:
        for (int i = 0; i < springs.Count; i++)
        {
            // 		i.water_update(k,d)
            springs[i].water_update(k, d);
        }
        // 	#represents the movement of the left and right neighbor of the springs

        // 	var left_deltas = []
        List<float> left_deltas = new List<float>();
        // 	var right_deltas = []
        List<float> right_deltas = new List<float>();

        // 	#initialize the values with an array of zeros
        // 	for i in range (springs.size()):
        for (int i = 0; i < springs.Count; i++)
        {
            // 		left_deltas.append(0)
            left_deltas.Add(0);
            // 		right_deltas.append(0)
            right_deltas.Add(0);
            // 		pass
        }

        // 	for j in range(passes):
        for (int j = 0; j < passes; j++)
        {
            // 		#loops through each spring of our array
            // 		for i in range(springs.size()):
            for (int i = 0; i < springs.Count; i++)
            {
                // 			#adds velocity to the spring to the LEFT of the current spring
                // 			if i > 0:
                if (i > 0)
                {
                    // 				left_deltas[i] = spread * (springs[i].height - springs[i-1].height)
                    left_deltas[i] = spread * (springs[i].height - springs[i - 1].height);
                    // 				springs[i-1].velocity += left_deltas[i]
                    springs[i - 1].velocity += left_deltas[i];
                    // 			#adds velocity to the spring to the RIGHT of the current spring
                    // 			if i < springs.size()-1:
                    if (i < springs.Count - 1)
                    {
                        // 				right_deltas[i] = spread * (springs[i].height - springs[i+1].height)
                        right_deltas[i] = spread * (springs[i].height - springs[i + 1].height);
                        // 				springs[i+1].velocity += right_deltas[i]
                        springs[i + 1].velocity += right_deltas[i];
                        // 		pass
                    }
                }
            }

            // 	new_border()
            new_border();
            // 	draw_water_body()
            draw_water_body();
        }
    }

    // func draw_water_body():
    void draw_water_body()
    {
        // 	#gets the curve of the border
        // 	var curve = water_border.curve
        // Curve2D curve = water_border.Curve;
        // 	#makes an array of the points in the curve
        // 	var points = Array(curve.get_baked_points())
        // Vector2[] points = curve.GetBakedPoints();
        // 	#the water polygon will contain all the points of the surface
        // 	var water_polygon_points = points
        // Vector2[] water_polygon_points = points;

        List<Vector2> water_polygon_points = new List<Vector2>();
        foreach (var item in springs)
        {
            water_polygon_points.Add(item.Position);
        }


        // 	#gets the first and last indexes of our surface array
        // 	var first_index = 0
        // int first_index = 0;
        // 	var last_index = water_polygon_points.size()-1
        // int last_index = water_polygon_points.Count - 1;
        // 	#add other two points at the bottom of the polygon, to close the water body
        // 	water_polygon_points.append(Vector2(water_polygon_points[last_index].x, bottom))
        water_polygon_points.Add(new Vector2(water_polygon_points.Last().x, bottom));
        // 	water_polygon_points.append(Vector2(water_polygon_points[first_index].x, bottom))
        water_polygon_points.Add(new Vector2(water_polygon_points.First().x, bottom));
        // 	#transforms our normal array into a poolvector2array
        // 	#the polygon draw function uses poolvector2arrays to draw the polygon, so we converted it
        // 	water_polygon_points = PoolVector2Array(water_polygon_points)
        // 	#sets the points of our polygon, and also our UV in the case we want to give it a texture
        // 	water_polygon.set_polygon(water_polygon_points)
        // 	water_polygon.set_uv(water_polygon_points)
        water_polygon.Polygon = water_polygon_points.ToArray();
        water_polygon.Uv = water_polygon_points.ToArray();
    }


    // func new_border():
    void new_border()
    {
        // 	#DRAW A NEW BORDER TO THE WATER

        // 	#creates a new curve 2D
        // 	var curve = Curve2D.new().duplicate()
        // Curve2D curve = water_border.Curve.Duplicate() as Curve2D;
        var curve = water_border.Curve;
        var diff = springs.Count - curve.GetPointCount();
        for (int i = 0; i <= diff; i++)
        {
            curve.AddPoint(new Vector2());
        }

        // 	#creates a new array, that holds the positions of the surface points!!
        // 	#we'll use those points to draw our border
        // 	var surface_points = []
        List<Vector2> surface_points = new List<Vector2>();
        // 	for i in range(springs.size()):
        for (int i = 0; i < springs.Count; i++)
        {
            // 		surface_points.append(springs[i].position)
            // surface_points.Add(springs[i].Position);
            curve.SetPointPosition(i, springs[i].Position);
        }

        // 	#adds the points to the curve
        // 	for i in range(surface_points.size()):
        // 		curve.add_point(surface_points[i])

        // 	#puts the new curve into our border, smooths it out and then update the border drawing
        // 	water_border.curve = curve
        // water_border.Curve = curve;
        // 	#water_border.smooth(true)
        // 	water_border.update()
        water_border.Update();
        // 	pass
    }


    // #this function adds a speed to a spring with this index
    // func splash(index, speed):
    void splash(int index, float speed)
    {
        // 	#checks if the index is valid
        // 	if index >= 0 and index < springs.size():
        if (index >= 0 && index < springs.Count)
        {
            // 		springs[index].velocity += speed
            springs[index].velocity += speed;
        }
        // 	pass
    }

    // func _on_Water_Body_Area_body_entered(body):
    void _on_Water_Body_Area_body_entered(Stone body)
    {

        // 	body.in_water()
        body.in_water();

        // 	#creates a instace of the particle system
        // 	var s = splash_particle.instance()
        var s = splash_particle.Instance() as Node2D;

        // 	#adds the particle to the scene
        // 	get_tree().current_scene.add_child(s)
        GetTree().CurrentScene.AddChild(s);

        // 	#sets the position of the particle to the same of the body
        // 	s.global_position = body.global_position
        s.GlobalPosition = body.GlobalPosition;
        // 	pass # Replace with function body.
    }

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        r_water_lenght();
        r_target_height();
        r_bottom();
        r_water_polygon();
        r_water_border();
        r_collisionShape();
        r_water_body_area();

        // #initializes the spring array and all the springs
        // func _ready():
        // 	water_border.width = border_thickness

        // 	spread = spread / 1000
        spread = spread / 1000f;

        // 	#loops through all the springs
        // 	#makes an array with all the springs
        // 	#initializes each spring
        // 	for i in range(spring_number):

        for (int i = 0; i < spring_number; i++)
        {
            // 		#the spring x position
            // 		#they are generated from left to right --- > 0, 32, 64, 96...
            // 		var x_position = distance_between_springs * i
            float x_position = distance_between_springs * i;
            // 		var w = water_spring.instance()
            var w = water_spring.Instance() as Water_Spring;
            // 		add_child(w)
            AddChild(w);
            // 		springs.append(w)
            springs.Add(w);
            // 		w.initialize(x_position, i)
            w.initialize(x_position, i);
            // 		w.set_collision_width(distance_between_springs)
            w.set_collision_width(distance_between_springs);
            // 		#connects our splash signal and calls the function splash
            // 		w.connect("splash", self, "splash")
            w.Connect(nameof(splash), this, nameof(splash));
        }

        // 	#calculates the total lenght of the water body
        // 	var total_lenght = distance_between_springs * (spring_number - 1)
        int total_lenght = distance_between_springs * (spring_number - 1);

        // 	#creates a new rectangle shape 2D
        // 	var rectangle = RectangleShape2D.new().duplicate()
        RectangleShape2D rectangle = collisionShape.Shape.Duplicate() as RectangleShape2D;

        // 	# area position stays right in the middle of the water body
        // 	# the extents of the rectangle are half of the size of the water body
        // 	var rect_position = Vector2(total_lenght / 2, depth / 2)
        Vector2 rect_position = new Vector2(total_lenght / 2, depth / 2);
        // 	var rect_extents = Vector2(total_lenght / 2, depth / 2)
        Vector2 rect_extents = new Vector2(total_lenght / 2, depth / 2);

        // 	#sets the position and the extents of the area and the collisionshape
        // 	water_body_area.position = rect_position
        water_body_area.Position = rect_position;
        // 	rectangle.set_extents(rect_extents)
        rectangle.Extents = rect_extents;
        // 	collisionShape.set_shape(rectangle)
        collisionShape.Shape = rectangle;

    }

}
