using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class JugadorMovimiento : NetworkBehaviour {

	public float speed;
	const float baseSpeed = 2;
	const float sphereSpeed = 1.5f;
	const float triangleSpeed = 1.0f;
	Rigidbody2D rb;

	float angle;
	GameObject character;

	void Start (){
		rb = GetComponent ("Rigidbody2D") as Rigidbody2D;
	}



	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
	}
}
