using UnityEngine;
using UnityEngine.Networking;

public class GestorMultijugador : NetworkBehaviour {
	//---------------------------------------------------------------------------------------------
	//---------------------------- VARIABLES NO SINCRONIZADAS -------------------------------------
	//---------------------------------------------------------------------------------------------
	public GameObject generadorMapasPref;
	GameObject generadorMapas;
	//---------------------------------------------------------------------------------------------
	//----------------------------- VARIABLES SINCRONIZADAS ---------------------------------------
	//---------------------------------------------------------------------------------------------
	public SyncListUInt equipo1JugadoresIDs = new SyncListUInt();
	public SyncListUInt equipo2JugadoresIDs = new SyncListUInt();
	public SyncListUInt jugadoresListos = new SyncListUInt(); //Han pulsado LISTO en el Lobby

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

	[ClientRpc]
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
	}
}
