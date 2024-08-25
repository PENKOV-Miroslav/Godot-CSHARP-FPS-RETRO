using Godot;
using System;

public class Enemy : KinematicBody
{
	//Varaibles
	[Export]
	int health = 3; // point de vie
	[Export]
	float speed = 1.5f; //vitesse mouvement
	[Export]
	float attackDistance = 3.0f; //Distance d'attaque

	KinematicBody player; // le joueur à attaquer

	AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		base._Ready();
		//On récupere le player
		player = GetTree().GetRoot().GetNode("Jeu/Player") as KinematicBody;
		//Récupere animationplayer
		animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
		//Animation de l'ennemi
		WalkAnim();
	}

	public void WalkAnim(){


		//Jouer l'animation par défaut en boucle
		animationPlayer.GetAnimation("Walk").SetLoop(true);
		//Lancer l'animation de marche
		animationPlayer.Play("Walk");
		
	}

	public override void _PhysicsProcess(float delta)
	{
		// Calcul du vecteur direction vers le joueur
		var dir = (player.Translation - Translation).Normalized();
		dir.y = 0;

		//Déplace le monstre vers le joueur
		if (Translation.DistanceTo(player.Translation) > attackDistance){
			MoveAndSlide(dir * speed, Vector3.Up);
		}
		
		// Regarder le joueur
		LookAt(player.GlobalTransform.origin, Vector3.Up);
		RotateObjectLocal(Vector3.Up, 3.14f);
	}

	// Prendre des dégats
	public void TakeDamage(){
		health--;
		//Si vie <= 0 alors on disparait
		if(health <= 0){
			QueueFree();
			player.Call("IncrementScore");
		}
	}

	public void Timeout(){
		if (Translation.DistanceTo(player.Translation) <= attackDistance)
		{
			GD.Print("Attack");
			Attack();
		}else
		{
			WalkAnim();
		}
	}

	public void Attack(){
		animationPlayer.PlaybackSpeed = 3;
		animationPlayer.Play("Attack");
		player.Call("TakeDamage");
	}

}
