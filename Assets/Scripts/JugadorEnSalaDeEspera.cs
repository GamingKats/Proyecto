using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class JugadorEnSalaDeEspera : NetworkBehaviour {

	[SyncVar]public bool enabled = true;

	Jugador esteJugador;
	bool equipo1;

	GameObject modelo, luz;
	void Start (){
		esteJugador = GetComponent<Jugador> ();
		EstablecerEquipoInicial ();
	}

	public override void OnStartClient(){
		Debug.LogError ("ClienteOnStart");
		if ( isServer ){
			if (SceneManager.GetActiveScene().name == "LobbyScene")
				GestorMultijugador.singleton.AñadirJugador (netId.Value, 0==Random.Range(0,2));
		}
	}

	void EstablecerEquipoInicial (){
		GameObject[] spawn; // = GameObject.FindGameObjectsWithTag ("PosicionInicialEsferas");
		if (GestorMultijugador.singleton.JugadorEnEquipo1 (netId.Value)) {
			spawn = GameObject.FindGameObjectsWithTag ("PosicionInicialEsferas");
			modelo = transform.GetChild (1).gameObject;
			equipo1 = true;
		} else {
			spawn = GameObject.FindGameObjectsWithTag ("PosicionInicialTriangulos");
			modelo = transform.GetChild (0).gameObject;
			equipo1 = false;
		}

		transform.GetChild(1).GetChild (3).gameObject.SetActive (false);
		modelo.SetActive (true);
		luz = modelo.transform.GetChild (0).gameObject;
		if (!isLocalPlayer) {
			luz.SetActive (false);
		}
		int indice = Random.Range (0, spawn.Length);
		transform.position = spawn [indice].transform.position;
		Destroy (spawn [indice]);
	}

	void EstablecerEquipo ( ){
		modelo.SetActive (false);
		if (equipo1) {
			modelo = transform.GetChild (1).gameObject;
		} else {
			modelo = transform.GetChild (0).gameObject;
		}
		modelo.SetActive (true);
		luz.SetActive (false);
		luz = modelo.transform.GetChild (0).gameObject;
		if (isLocalPlayer) {
			luz.SetActive (true);
		}
	}
		
	void OnTriggerEnter2D ( Collider2D col ){
		if (col.tag == "Meta" && isLocalPlayer) {
			Destroy (this);
			Cmd_JugadorListo (netId.Value);
		}
	}
	void Update (){
		if (isLocalPlayer) {
			if (transform.position.x >= 0) {
				if (equipo1) {
					equipo1 = false;
					Cmd_EstablecerEquipo (netId.Value, equipo1);
					EstablecerEquipo ();
				}
			} else {
				if (!equipo1) {
					equipo1 = true;
					Cmd_EstablecerEquipo (netId.Value, equipo1);
					EstablecerEquipo ();
				}
			}
		}
	}

	[Command]
	void Cmd_EstablecerEquipo ( uint jugadorId, bool equipo1 ){
		GestorMultijugador.singleton.EstablecerEquipo (jugadorId, equipo1);
	}
	[Command]
	void Cmd_JugadorListo ( uint jugadorId){
		//DontDestroyOnLoad (gameObject);
		enabled = false;
		GestorMultijugador.singleton.JugadorListo (jugadorId);
	}
}
