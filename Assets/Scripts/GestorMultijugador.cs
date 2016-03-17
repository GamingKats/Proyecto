using UnityEngine;
using UnityEngine.Networking;

public class GestorMultijugador : NetworkBehaviour {
	//---------------------------------------------------------------------------------------------
	//---------------------------- VARIABLES NO SINCRONIZADAS -------------------------------------
	//---------------------------------------------------------------------------------------------

	//---------------------------------------------------------------------------------------------
	//----------------------------- VARIABLES SINCRONIZADAS ---------------------------------------
	//---------------------------------------------------------------------------------------------
	public SyncListUInt equipo1JugadoresIDs = new SyncListUInt();
	public SyncListUInt equipo2JugadoresIDs = new SyncListUInt();
	public SyncListUInt jugadoresListos = new SyncListUInt(); //Han pulsado LISTO en el Lobby

	//--------------------------------------------------------------------------------------------
	//---------------------------- IMPLEMENTACIÓN SINGLETON --------------------------------------
	//--------------------------------------------------------------------------------------------
	private static GestorMultijugador _singleton = null;

	public static GestorMultijugador singleton{
		get{
			if (_singleton == null) {
				_singleton = Object.FindObjectOfType (typeof(GestorMultijugador)) as GestorMultijugador;

				if (_singleton == null) {
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
		return jugadoresListos.Count == (equipo1JugadoresIDs.Count + equipo2JugadoresIDs.Count) &&
			equipo1JugadoresIDs.Count == equipo2JugadoresIDs.Count;
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
			GestorRed.singleton.ServerChangeScene ("InGameScene");
		}
	}

}
