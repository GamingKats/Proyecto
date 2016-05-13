using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Jugador : NetworkBehaviour {

	[SyncVar]
	public string nombreUsuario;

	bool esTriangulo;

	bool estaEnEquipo1;


	void Awake (){
		DontDestroyOnLoad (gameObject);
		if (SceneManager.GetActiveScene ().name == "LobbyScene") {
			GetComponent<JugadorEnSalaDeEspera> ().enabled = true;
		}
		if (SceneManager.GetActiveScene ().name == "InGameScene") {
			ComienzaAJugar ();
		}
	}

	void OnLevelWasLoaded ( int lvl ){
		if (lvl == SceneManager.GetSceneByName ("LobbyScene").buildIndex) {
			GetComponent<JugadorEnSalaDeEspera> ().enabled = true;
		}
		if (lvl == SceneManager.GetSceneByName ("InGameScene").buildIndex) {
			ComienzaAJugar ();
		}
	}

	public void ComienzaAJugar (){
		Debug.LogError ("COMIENZA A JUGAR");
		GetComponent<JugadorEnSalaDeEspera>().enabled = false;
		GetComponent<JugadorMovimiento> ().enabled = true;
		Invoke("AsignarBando", 1);
	}

	public void AsignarBando(){
		print ("NETID: " + netId.Value);
		Transform esfera, triangulo;
		triangulo = transform.GetChild (0);
		esfera = transform.GetChild (1);

		JugadorMovimiento jm = GetComponent<JugadorMovimiento> ();
		if (GestorMultijugador.singleton.EsEquipo1Triangulos ()) {
			if (GestorMultijugador.singleton.JugadorEnEquipo1 (netId.Value)) {
				triangulo.gameObject.SetActive (true);
				esfera.gameObject.SetActive (false);
				print ("Opcion 1: Jugador en Equipo 1 y es Triángulo");
				_AsignarEquipo (true, true);
				jm.EstablecerBando (false);
			} else {
				triangulo.gameObject.SetActive (false);
				esfera.gameObject.SetActive (true);
				print ("Opcion 2: Jugador en equipo 2 y es esfera");
				_AsignarEquipo (false, false);
				jm.EstablecerBando (true);
			}
		} else {
			jm.EstablecerBando (true);
			if (GestorMultijugador.singleton.JugadorEnEquipo1 (netId.Value)) {
				triangulo.gameObject.SetActive (false);
				esfera.gameObject.SetActive (true);
				print ("Opcion 3: Jugador en equipo 1 y es Esfera");
				_AsignarEquipo (true, false);
				jm.EstablecerBando (true);
			} else {
				triangulo.gameObject.SetActive (true);
				esfera.gameObject.SetActive (false);
				print ("Opcion 4: Jugador en equipo 2 y es triangulo");
				_AsignarEquipo (false, true);
				jm.EstablecerBando (false);
			}
		}
		if (isLocalPlayer)
			EstablecerPosicionInicial ();
	}

	void _AsignarEquipo ( bool equipo1, bool triangulo){
		esTriangulo = triangulo;
		estaEnEquipo1 = equipo1;
		Debug.LogError ("Asignando equipo " + esTriangulo + ", " + estaEnEquipo1);
	}

	void EstablecerPosicionInicial (){
		GameObject[] posiciones;
		if (esTriangulo) {
			posiciones = GameObject.FindGameObjectsWithTag ("PosicionInicialTriangulos");
		} else {
			posiciones = GameObject.FindGameObjectsWithTag ("PosicionInicialEsferas");
		}
		int indice;
		Collider2D colision;
		do {
			indice = Random.Range (0, posiciones.Length);
			colision = Physics2D.OverlapPoint( posiciones[indice].transform.position );
			Debug.LogError(posiciones[indice].transform.position);
		} while (colision != null);
		transform.position = posiciones [indice].transform.position;
	}
	//--------------------------------------------------------------------------------------------------
	//-----------------------------------------	CLIENT RPCs	-------------------------------------------
	//--------------------------------------------------------------------------------------------------
	[ClientRpc]
	void RpcAsignarBando (){
		AsignarBando ();
	}
	//--------------------------------------------------------------------------------------------------
	//-----------------------------------------	COMANDOS	-------------------------------------------
	//--------------------------------------------------------------------------------------------------
	[Command]
	public void Cmd_RenombrarJugador ( string nombre ){
		nombreUsuario = nombre;
		name = nombre;
	}

}
