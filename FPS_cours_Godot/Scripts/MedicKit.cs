using Godot;
using System;

public class MedicKit : Area
{
	AnimationPlayer animationPlayer;
	public override void _Ready(){
		base._Ready();
		animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
		animationPlayer.GetAnimation("Rotation").SetLoop(true);
		animationPlayer.Play("Rotation");
	}
	public void OnCollision(KinematicBody body){
		
		if (body.HasMethod("Health")) {

			// On appelle cette fonction
			body.Call("Health");
			QueueFree();
		}
	}
}
