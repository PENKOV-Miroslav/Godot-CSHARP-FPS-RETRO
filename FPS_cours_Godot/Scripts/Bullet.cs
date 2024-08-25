using Godot;
using System;

public class Bullet : Spatial
{
	// Variables
	float speed = 350.0f;

// Fonctions Godot
	public override void _Process(float deltat) { 

		// Propoulser le projectile
		Translation -= GlobalTransform.basis.z * speed * deltat;
	}

	//Signaux
	public void Timeout() {
		GD.Print("Destruction bullet");
		QueueFree(); // Détruire l'objet apres un certain temps
	}

	// Quand la balle rentre en collision avec un body
	public void OnCollision(KinematicBody body) {

		// Si le body dispose de la fonction TakeDamage...
		if (body.HasMethod("TakeDamage")) {

			// On appelle cette fonction
			body.Call("TakeDamage");
			QueueFree();
		}
	}
}
