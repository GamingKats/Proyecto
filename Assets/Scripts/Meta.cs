using UnityEngine;
using System.Collections;

public class Meta : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Para saber si colisiona con otro jugador
	void OnCollisionEnter(Collision collision) {
		Jugador j = collision.gameObject.GetComponent<Jugador> ();
		if (j != null) {
			if (!j.esTriangulo) {
				Debug.Log ("He escapado");
				j.Cmd_IncrementaEscapada(20);
				j.moverse = false;
			}else{
				Debug.Log("No hago nada");
			}
		}
	}
}
