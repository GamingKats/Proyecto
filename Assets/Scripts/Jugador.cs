using UnityEngine;
using UnityEngine.Networking;

public class Jugador : NetworkBehaviour {

	[SyncVar]
	public string nombreUsuario;



	//--------------------------------------------------------------------------------------------------
	//-----------------------------------------	COMANDOS	-------------------------------------------
	//--------------------------------------------------------------------------------------------------
	[Command]
	public void Cmd_RenombrarJugador ( string nombre ){
		nombreUsuario = nombre;
		name = nombre;
	}
}
