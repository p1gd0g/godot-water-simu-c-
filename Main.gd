extends Node2D

#loads the stone scene
var stone = preload("res://Scenes/Stone.tscn")

#called when there's an input
func _input(event):
	#if there's any mouse button press
	if event is InputEventMouseButton and event.is_pressed():
		#makes an instance of the stone scene
		var s = stone.instance()
		
		#initializes the stone at the mouse position
		s.initialize(get_global_mouse_position())
		
		#adds the stone to the current scene
		get_tree().current_scene.add_child(s)
