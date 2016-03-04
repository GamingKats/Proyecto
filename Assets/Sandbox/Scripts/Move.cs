using UnityEngine;

namespace TestSinglePlayer{
	[RequireComponent (typeof(Rigidbody2D))]
	[RequireComponent (typeof(CircleCollider2D))]
	public class Move : MonoBehaviour {
		
		
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
			// handle player input for movement
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
				
				move.x *= speed;
				move.y *= speed;
				rb.velocity = move;
			} else {
				rb.angularVelocity = 0;
				rb.velocity = Vector2.zero;
			}
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
			//Triangles angle
			//if (GameManager.GetTeam (netId.Value) == GameManager.GetTrianglesTeam ())
				transform.rotation = Quaternion.Euler (0, 0, angle);
		}
	}
}
