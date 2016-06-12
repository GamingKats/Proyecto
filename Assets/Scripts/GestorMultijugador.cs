
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GestorMultijugador : NetworkBehaviour {
	//---------------------------------------------------------------------------------------------
	//---------------------------- VARIABLES NO SINCRONIZADAS -------------------------------------
	//---------------------------------------------------------------------------------------------
	public GameObject generadorMapasPref;
	GameObject generadorMapas;
	public bool jugando =false;
	public bool rondaFinalizada =false;

	//---------------------------------------------------------------------------------------------
	//----------------------------- VARIABLES SINCRONIZADAS ---------------------------------------
	//---------------------------------------------------------------------------------------------
	public SyncListUInt equipo1JugadoresIDs = new SyncListUInt();
	public SyncListUInt equipo2JugadoresIDs = new SyncListUInt();
	public SyncListUInt jugadoresListos = new SyncListUInt(); //Han pulsado LISTO en el Lobby

	//---------------------------------------------------------------------------------------------
	//----------------------------- VARIABLES SINCRONIZADAS CONTROL GAME --------------------------
	//---------------------------------------------------------------------------------------------
	[SyncVar]public int numRondas = 0;
	[SyncVar]public int rondaActual = 0;
	[SyncVar]public int tiempoRonda = 0;

	[SyncVar]public float tiempoRondaActual=0;
	[SyncVar]public float tiempoActual =0;
	[SyncVar]public float countBackActual = 5;

	[SyncVar]public bool movimientoGeneral = false;

	[SyncVar] bool esEquipo1Triangulos;

	//--------------------------------------------------------------------------------------------
	//---------------------------- IMPLEMENTACIÓN SINGLETON --------------------------------------
	//--------------------------------------------------------------------------------------------
	private static GestorMultijugador _singleton = null;

	public static GestorMultijugador singleton{
		get{
			//Debug.LogError (_singleton);
			if (_singleton == null) {
				_singleton = Object.FindObjectOfType (typeof(GestorMultijugador)) as GestorMultijugador;

				if (_singleton == null) {
					Debug.LogError ("Capushoo");
					GameObject go = new GameObject ("GestorMultijugador");
					DontDestroyOnLoad (go);
					_singleton = go.AddComponent<GestorMultijugador> ();
				}
			}

			return _singleton;
		}
	}

	//--------------------------------------------------------------------------------------------------
	//----------------------------------- FIN SINGLETON ------------------------------------------------
	//--------------------------------------------------------------------------------------------------



	void Awake (){
		if (GestorMultijugador._singleton == null) {
			GestorMultijugador._singleton = GestorMultijugador.singleton;
			//Debug.LogError ("[ERROR]: Awake del singleton");
		}else{
			GestorMultijugador._singleton = this;
			Destroy (GameObject.Find ("GestorMultijugador"));
		}
	}

	// Update is called once per frame
	void Update () {
		if (isServer) {
			Debug.Log ("Soy el server");
			Debug.Log ("Jugando: "+jugando);
			//Gestion de tiempo de ronda/partida
			if (SceneManager.GetActiveScene ().name == "InGameScene") {
				Debug.Log ("Estoy en la escena de juego");
				if (jugando) {
					if (rondaFinalizada) {
						Debug.Log("Finalizo ronda");
						ContinuarPartida ();
					} else {
						cuentaAtras ();
					}
				} else {
					cuentaAtrasInicial ();
				}

			}
		}
	}
	//--------------------------------------------------------------------------------------------------
	//-----------------------------------------	UTILES	-------------------------------------------
	//--------------------------------------------------------------------------------------------------
	public bool JugadorEnEquipo1 ( uint netid ){
		return equipo1JugadoresIDs.Contains (netid);
	}

	public bool TodosListos ( ){
		return jugadoresListos.Count == (equipo1JugadoresIDs.Count + equipo2JugadoresIDs.Count) 
			//&&	equipo1JugadoresIDs.Count == equipo2JugadoresIDs.Count
			;
	}

	public void AñadirJugador ( uint jugadorID, bool equipo1 ){
		if (equipo1)
			equipo1JugadoresIDs.Add (jugadorID);
		else
			equipo2JugadoresIDs.Add (jugadorID);
	}


	public void EstablecerEquipo ( uint jugadorId, bool equipo1){
		if (equipo1) {
			if (!equipo1JugadoresIDs.Contains (jugadorId)) {
				if (equipo2JugadoresIDs.Contains (jugadorId)) {
					equipo1JugadoresIDs.Add (jugadorId);
					equipo2JugadoresIDs.Remove (jugadorId);
				} else {
					Debug.LogError ("[ERROR] Intentando cambiar de equipo, pero el jugador no estaba en ninguno.");
				}
			} else
				Debug.LogWarning ("[Warning] Intentando cambiar de equipo al mismo equipo.");
		} else {
			if (!equipo2JugadoresIDs.Contains (jugadorId)) {
				if (equipo1JugadoresIDs.Contains (jugadorId)) {
					equipo2JugadoresIDs.Add (jugadorId);
					equipo1JugadoresIDs.Remove (jugadorId);
				} else {
					Debug.LogError ("[ERROR] Intentando cambiar de equipo, pero el jugador no estaba en ninguno.");
				}
			} else
				Debug.LogWarning ("[Warning] Intentando cambiar de equipo al mismo equipo.");
		}
	}

	public void JugadorListo ( uint jugadorId ){
		//Comprobamos que no lo hemos añadido ya
		if (!jugadoresListos.Contains (jugadorId)) {
			jugadoresListos.Add (jugadorId);
		}
		if (TodosListos ()) {
			Debug.Log ("Estoy apunto de empezar");
			//Variables iniciales hardcodeadas
			numRondas = 1;
			rondaActual = 1;
			tiempoRonda = 30; // En segundos
			tiempoRondaActual = tiempoRonda;
			tiempoActual = 30.0f;

			Debug.Log ("Set variables iniciales");
			generadorMapas = (GameObject)Instantiate(generadorMapasPref);
			generadorMapas.GetComponent<GeneradorMapas> ().ReiniciarValores ();
			NetworkServer.Spawn (generadorMapas);
			GestorRed.singleton.ServerChangeScene ("InGameScene");
			//Rpc_ComenzarPartida (); //Está bug en Unity y no se ejecuta en los clientes
			//En su lugar se ejecuta en OnLevelWasLoaded(), en el script Jugador
		}
	}

	public bool EsEquipo1Triangulos (){
		return esEquipo1Triangulos;
	}

	/*[ClientRpc]
	public void Rpc_ComenzarPartida (){
		//VERSION ANTIGUA
		//GestorRed.singleton.ServerChangeScene ("InGameScene");
		//------------------------------------------------------
		//generadorMapas = GameObject.FindGameObjectWithTag("GeneradorMapas");
		ComenzarPartida ();
	}

	void ComenzarPartida (){
		GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
		Debug.Log ("JUgadores vacio??");
		foreach (GameObject go in jugadores) {
			Jugador jugador = go.GetComponent<Jugador> ();
			Debug.Log ("ComIeNzA A jugAr");
			jugador.ComienzaAJugar ();
		}
	}*/

	//Función que restaura los valores iniciales de cada ronda.
	void ContinuarPartida(){
		if (UltimaRonda()) {
			//ToDo
			Debug.Log ("Me tengo que ir a la pantalla de resultados");
			GestorMultijugador.singleton.movimientoGeneral = false;
			GestorRed.singleton.ServerChangeScene ("EndScene");
		} else {
			GameObject[] jugadores = GameObject.FindGameObjectsWithTag ("Player");
			Debug.Log ("JUgadores vacio??");
			Debug.Log ("Sumo y seteo Inicial");
			if (isServer) {
				rondaActual += 1;
				tiempoActual = 30;
				countBackActual = 5;
				jugando = false;
				rondaFinalizada = false;
			}
			foreach (GameObject go in jugadores) {
				Jugador jugador = go.GetComponent<Jugador> ();
				Debug.Log ("Continua jugando");
				GestorRed.singleton.ServerChangeScene ("InGameScene");
			}
		}

	}

	//Retorna si es la ultima ronda
	public bool UltimaRonda(){
		return (rondaActual>=numRondas);
	}

	//Cuenta atras para empezar el nivel
	public void cuentaAtrasInicial(){	
		Debug.Log ("Empiezo a contar");	
		if (countBackActual <= 0.0) {
			jugando = true;
			movimientoGeneral = true;

		} else {	
			movimientoGeneral = false;	
			countBackActual -= Time.deltaTime;
		}	
	}

	//Cuenta atras del nivel
	public void cuentaAtras(){	
		Debug.Log ("Continuo contando");
		if (tiempoActual <= 0.0) {
			jugando = false;
			rondaFinalizada = true;

		} else {		
			tiempoActual -= Time.deltaTime;
		}
		Debug.Log ("Tiempo ronda: " + tiempoActual);
	}

	//retorna si los jugadores están en el mismo equipo
	public bool sonMismoEquipo(uint jugador1, uint jugador2){
		if (equipo1JugadoresIDs.Contains (jugador1) && equipo1JugadoresIDs.Contains (jugador2))
			return true;
		if (equipo2JugadoresIDs.Contains (jugador1) && equipo2JugadoresIDs.Contains (jugador2))
			return true;
		return false;
	}
}
