using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class JugadorMovimiento : NetworkBehaviour {

	[SerializeField] float velocidad;
	[SerializeField] float velocidadBase = 2;
	[SerializeField] float velocidadEsferas = 1.5f;
	[SerializeField] float velocidadTriangulos = 1.0f;
	[SerializeField] Rigidbody2D rb;

	float angle;
	GameObject character;
	JugadorEnSalaDeEspera jesde;

	void Start (){
		rb = GetComponent ("Rigidbody2D") as Rigidbody2D;
		jesde = GetComponent<JugadorEnSalaDeEspera> ();
		EstablecerBando (true);
	}

	public void EstablecerBando ( bool esfera ){
		if (esfera) {
			velocidad = velocidadBase * velocidadEsferas;
		} else {
			velocidad = velocidadBase * velocidadTriangulos;
		}
	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (move.x != 0 || move.y != 0) {
			int a, b;
			a = (int)(move.x != 0 ? (move.x > 0 ? 1 : -1) : 0);
			b = (int)(move.y != 0 ? (move.y > 0 ? 1 : -1) : 0);

			angle = Mathf.Acos ((a) / Mathf.Sqrt (((a * a) + (b * b)))) * 180 / Mathf.PI; //=ACOS(REDONDEAR(B6)/(REDONDEAR(B6)*REDONDEAR(B6)+REDONDEAR(C6)*REDONDEAR(C6))^(1/2))*180/PI();

			//Debug.Log(angle);
			if (b < 0) {
				angle = 360 - angle;
			}
			angle +=90;

			move.x *= velocidad;
			move.y *= velocidad;
			rb.velocity = move;
		} else {
			rb.angularVelocity = 0;
			rb.velocity = Vector2.zero;
		}
		if ( jesde.enabled )
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y-4,-10);
		else
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
		//Triangles angle
		//if (GameManager.GetTeam (netId.Value) == GameManager.GetTrianglesTeam ())
		transform.rotation = Quaternion.Euler (0, 0, angle);
	}
}
