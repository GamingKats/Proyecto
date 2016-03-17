using UnityEngine;
using UnityEngine.Networking;

public class JugadorEnSalaDeEspera : NetworkBehaviour {

	bool editandoNombre = false;
	string nombreAux = "Elije Nombre";
	int width, height, spacing, posX, posY;
	Jugador esteJugador;

	void Start (){
		esteJugador = GetComponent<Jugador> ();
	}

	public override void OnStartClient(){
		if ( isServer )
			GestorMultijugador.singleton.AñadirJugador (netId.Value, 0==Random.Range(0,2));
		if (isLocalPlayer) {
			if (PlayerPrefs.HasKey ("ApodoJugador")) {
				nombreAux = PlayerPrefs.GetString ("ApodoJugador");
			}
			esteJugador.Cmd_RenombrarJugador ( nombreAux );
		}
	}

	void OnGUI (){
		if (!isLocalPlayer)
			return;
		
		MostrarJugadores ();
	}

	void MostrarJugadores (){
		posX = Screen.width / 20;
		posY = Screen.height / 13;
		width = (Screen.width - 3 * posX) / 2;
		height = posY;
		spacing = posX;

		GUI.Button (new Rect ((Screen.width - width) / 2, 0, width, spacing), NetworkManager.singleton.matchName);

		//GUI.Button (new Rect (posX, posY, width, height), "TEAM1");
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach ( GameObject user in players ){
			Jugador jugador = user.GetComponent<Jugador>();
			if ( !GestorMultijugador.singleton.JugadorEnEquipo1( jugador.netId.Value ) )
				continue;
			posY += height;
			if (jugador.netId.Value == netId.Value) {
				GUI.skin.button.normal.textColor = Color.red;
				if (editandoNombre) {
					nombreAux = GUI.TextField (new Rect (posX, posY, width-2*spacing, height), nombreAux);
					if (GUI.Button (new Rect (posX + width - 2*spacing, posY, spacing, height), "V")) {
						esteJugador.Cmd_RenombrarJugador( nombreAux );
						gameObject.name = nombreAux;
						editandoNombre = false;
						PlayerPrefs.SetString ("ApodoJugador", nombreAux);
					}
					if (GUI.Button (new Rect (posX + width-1*spacing, posY, spacing, height), "X")) {
						nombreAux = esteJugador.nombreUsuario;
						editandoNombre = false;
					}
				} else {
					if (GUI.Button (new Rect (posX, posY, width - spacing, height), jugador.nombreUsuario)) {
						nombreAux = esteJugador.nombreUsuario;
						editandoNombre = true;
					}
					string pReady = GestorMultijugador.singleton.jugadoresListos.Contains(jugador.netId.Value) ? "V":"X";
					GUI.Button ( new Rect ( posX+width-spacing, posY, spacing, height), pReady);
				}
			} else {
				GUI.skin.button.normal.textColor = Color.white;
				GUI.Button (new Rect (posX, posY, width - spacing, height), jugador.nombreUsuario);
				string pReady = GestorMultijugador.singleton.jugadoresListos.Contains(jugador.netId.Value) ? "V":"X";
				GUI.Button ( new Rect ( posX+width-spacing, posY, spacing, height), pReady);
			}
		}
		GUI.skin.button.normal.textColor = Color.white;
		posY += height;
		if ( !GestorMultijugador.singleton.JugadorEnEquipo1( netId.Value ) && !GestorMultijugador.singleton.jugadoresListos.Contains(netId.Value)) {
			if ( GUI.Button ( new Rect ( posX,posY,width, height), "Join team1") ){
				Cmd_EstablecerEquipo(netId.Value, true);
			}
		}

		posX += width + spacing;
		posY = Screen.height / 13;
		//GUI.Button (new Rect (posX, posY, width, height), "TEAM2");
		foreach ( GameObject user in players ){
			Jugador jugador = user.GetComponent<Jugador>();
			if ( GestorMultijugador.singleton.JugadorEnEquipo1( jugador.netId.Value ) )
				continue;
			posY += height;
			if (jugador.netId.Value == netId.Value) {
				GUI.skin.button.normal.textColor = Color.red;
				if (editandoNombre) {
					nombreAux = GUI.TextField (new Rect (posX, posY, width-2*spacing, height), nombreAux);
					if (GUI.Button (new Rect (posX + width - 2*spacing, posY, spacing, height), "V")) {
						esteJugador.Cmd_RenombrarJugador( nombreAux );
						gameObject.name = nombreAux;
						editandoNombre = false;
						PlayerPrefs.SetString ("ApodoJugador", nombreAux);
					}
					if (GUI.Button (new Rect (posX + width-1*spacing, posY, spacing, height), "X")) {
						nombreAux = esteJugador.nombreUsuario;
						editandoNombre = false;
					}
				} else {
					if (GUI.Button (new Rect (posX, posY, width - spacing, height), jugador.nombreUsuario)) {
						nombreAux = esteJugador.nombreUsuario;
						editandoNombre = true;
					}
					string pReady = GestorMultijugador.singleton.jugadoresListos.Contains(jugador.netId.Value) ? "V":"X";
					GUI.Button ( new Rect ( posX+width-spacing, posY, spacing, height), pReady);
				}
			} else {
				GUI.skin.button.normal.textColor = Color.white;
				GUI.Button (new Rect (posX, posY, width - spacing, height), jugador.nombreUsuario);
				string pReady = GestorMultijugador.singleton.jugadoresListos.Contains(jugador.netId.Value) ? "V":"X";
				GUI.Button ( new Rect ( posX+width-spacing, posY, spacing, height), pReady);
			}
		}
		GUI.skin.button.normal.textColor = Color.white;
		posY += height;
		if (GestorMultijugador.singleton.JugadorEnEquipo1( netId.Value ) && !GestorMultijugador.singleton.jugadoresListos.Contains(netId.Value)) {
			if ( GUI.Button ( new Rect ( posX,posY,width, height), "Join team2") ){
				Cmd_EstablecerEquipo(netId.Value, false);
			}
		}

		if (!GestorMultijugador.singleton.jugadoresListos.Contains(netId.Value)) {
			if (GUI.Button (new Rect ((Screen.width - width) / 2, Screen.height - height - spacing, width, height), "I'm Ready")) {
				Cmd_JugadorListo (netId.Value);
			}
		}
	}

	[Command]
	void Cmd_EstablecerEquipo ( uint jugadorId, bool equipo1 ){
		GestorMultijugador.singleton.EstablecerEquipo (jugadorId, equipo1);
	}
	[Command]
	void Cmd_JugadorListo ( uint jugadorId){
		GestorMultijugador.singleton.JugadorListo (jugadorId);
	}
}
