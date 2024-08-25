using Godot;
using System;

public class Player : KinematicBody
{
	[Export]
	int health = 10;
	int score = 0;
	// Variable de la souris/regard
	float lookAngle = 90.0f;

	[Export]
	float mouseSensivity = 1.0f;

	Vector2 mouseDelta = new Vector2();
	
	//fin souris

	//Variables du clavier/déplacement
	[Export]
	float moveSpeed = 4.0f;
	float gravity = 10.0f;
	float jumpForce = 8.0f;
	Vector3 velocity = new Vector3();

	//fin clavier

	//Création réf
	Camera camera;
	PackedScene bulletScene;
	Spatial gun;

	AudioStream son;
	AudioStreamPlayer audioStreamPlayer;

	Label label;

	//Fonction GODOT
	//Ready
	public override void _Ready(){
		base._Ready();
		//On recupére le noeud camera
		camera = GetNode("Camera") as Camera;
		gun = GetNode("Camera/Gun") as Spatial;
		bulletScene = (PackedScene) ResourceLoader.Load("res://Scenes/Bullet.tscn");
		son = (AudioStream) ResourceLoader.Load("res://Assets/laser.wav");
		audioStreamPlayer = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
		audioStreamPlayer.Stream = son;
		label = GetTree().GetRoot().GetNode("Jeu/HUD/Label") as Label;
		label.Text = health.ToString();
	}

	// Gestion des inputs et du regard (camera)

	public override void _Input(InputEvent ev) {
		if (ev is InputEventMouseMotion eventMouse)
		{
			//On recupére le mouvement de la souris
			mouseDelta = eventMouse.Relative;
		}
	}

	public override void _Process(float deltat) {

		//On applique la rotation sur camera (axe y)
		camera.RotationDegrees -= new Vector3(Mathf.Rad2Deg(mouseDelta.y),0, 0) * mouseSensivity * deltat;
		camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.x, -lookAngle, lookAngle),camera.RotationDegrees.y,camera.RotationDegrees.z);
		
		//rotation sur le perso (axe x)
		RotationDegrees -= new Vector3(0,Mathf.Rad2Deg(mouseDelta.x),0) * mouseSensivity * deltat;

		mouseDelta = new Vector2();

		//Gestion du tir / fusil
		if (Input.IsActionJustPressed("shoot"))
		{
			Shoot();
		}
	}

	public override void _PhysicsProcess(float deltat) {

		//Vélocité initial
		velocity.x = 0;
		velocity.z = 0;

		//Direction du mouvement
		var direction = new Vector2();

		//Test des touche zqsd
		if (Input.IsActionPressed("forward"))
		{
			direction.y  -= 1;
		}
		if (Input.IsActionPressed("back"))
		{
			direction.y  += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			direction.x  -= 1;
		}
		if (Input.IsActionPressed("right"))
		{
			direction.x  += 1;
		}

		direction = direction.Normalized();

		var forward = GlobalTransform.basis.z; //Récupere l'orientation du perso pour faire la bonne translation / mouvement
		var right = GlobalTransform.basis.x;

		//On calcule la vélocité
		//avant /arriére
		velocity.z = (forward * direction.y + right * direction.x).z * moveSpeed;

		//gauche /droite

		velocity.x = (forward * direction.y + right * direction.x).x * moveSpeed;

		//haut bas / saut

		velocity.y -= gravity * deltat;

		//On applique le mouvement
		velocity = MoveAndSlide(velocity, Vector3.Up);

		//Gestion du saut
		//Si touche espace + on est au sol alors on peut sauter
		if (Godot.Input.IsActionPressed("ui_select") && IsOnFloor()) //&& = ET
		{
			velocity.y = jumpForce;
		}

	}

	//Fonction perso

	public void Shoot ()
	{

		audioStreamPlayer.Play();
		//Instancier un projectile / bullet

		Spatial bullet = (Spatial) bulletScene.Instance();
		//bullet.SetTranslation(Vector3.One); //positionne l'objet instancier la ou on le souhaite
		//On défini la position du projectile
		bullet.GlobalTransform = gun.GlobalTransform;
		bullet.Scale = new Vector3(0.1f,0.1f,0.1f);
		// On l'ajoute à l'arbre de scéne
		GetTree().GetRoot().CallDeferred("add_child", bullet);

	}

		// Prendre des dégats
	public void TakeDamage(){
		GD.Print("Aie ! Ma vie est de " + health);
		health--;
		label.Text = health.ToString();
		//Si vie <= 0 alors on disparait et reload le niveau
		if(health <= 0){
			GetTree().ReloadCurrentScene();
		}
	}

	public void Health(){
		GD.Print("YEAH ! Ma vie est de " + health);
		health+=5;
		label.Text = health.ToString();
		//Si vie >= 30 alors on ne peux plus stocké de la vie
		if(health >= 30){
			health = 30;
		}
	}

	public void IncrementScore(){
		score++ ;

		if (score >= 1)
		{
			GetTree().ChangeScene("res://Scenes/Lvl2.tscn");
		}
	}
}
