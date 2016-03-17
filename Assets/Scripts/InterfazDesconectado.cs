using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;

public class InterfazDesconectado : MonoBehaviour {

	public GestorRed gestorRed;
	public InputField nombreSala;
	public Button botonComenzar;

	bool redLocal;
	bool aleatorio;

	int partidasPorPagina = 20;
	int paginaOffset = 0;

	// Use this for initialization
	void Start () {
		gestorRed = gestorRed.GetComponent<GestorRed> ();
		NetworkManager.singleton = gestorRed;
		gestorRed.StartMatchMaker ();
		NombreSalaCambiado ();
	}


	public void NombreSalaCambiado (){
		if (nombreSala.text == "") {
			botonComenzar.transform.GetChild (0).GetComponent<Text> ().text = "Jugar YA";
		} else if (nombreSala.text.Length == 1) {
			botonComenzar.transform.GetChild (0).GetComponent<Text> ().text = "Jugar en";
		}
	}

	public void CambiarTipoDeRed (Slider tipoRed){
		Debug.Log ("Tipo: " + tipoRed.value);
		if (tipoRed.value == 0) {
			redLocal = true;
			gestorRed.StopMatchMaker ();
		} else {
			redLocal = false;
			gestorRed.StartMatchMaker ();
		}
	}

	public void PulsarBotonPartidaConcreta(){
		aleatorio = false;
		if (redLocal) {
			CrearUnirseLAN ();
		}else
			BuscarPartida ();
	}
	public void PulsarBotonPartidaAleatoria(){
		aleatorio = true;
		if (redLocal) {
			CrearUnirseLAN ();
		}else
			BuscarPartida ();
	}

	public void CrearUnirseLAN (){

		IPAddress address;
		bool esCliente = true;
		if (nombreSala.text == "localhost" || nombreSala.text == "") {
			esCliente = false;
		}else if (IPAddress.TryParse (nombreSala.text, out address)) {
			if (nombreSala.text.StartsWith ("127.")) {
				esCliente = false;
			}
		}
		if (esCliente) {
			gestorRed.networkAddress = nombreSala.text;
			gestorRed.StartClient ();
		} else {
			gestorRed.StartHost ();
		}
	}

	public void BuscarPartida (){
		/*
		 * Descomentando este bloqe podemos dejarlo en un solo botón en lugar de dos.
		 * Ahora hay un botón para modo aleatorio y otro para entrar en una parida en concreto.
		 * La idea es dejarlo en un solo botón y que el jugador entre en una partida concreta
		 * al poner el nombre de la sala, y si deja el nombre en blanco entrará aleatoriamente.
		 */
		//if (nombreSala.text == "")
		//	aleatorio = true;
		//else
		//	aleatorio = false;
		if ( aleatorio )
			gestorRed.matchMaker.ListMatches(0,partidasPorPagina,"",UnirseOCrear);
		else
			gestorRed.matchMaker.ListMatches(0,partidasPorPagina,nombreSala.text, UnirseOCrear);
	}

	//ASync
	//Se ejecutará una vez la función NetworkManager.matchMaker.ListMatches se haya completado
	void UnirseOCrear (ListMatchResponse matchList){
		gestorRed.matches = matchList.matches;
		if (gestorRed.matchInfo == null) {
			if (matchList.matches == null) {
				Debug.LogError ("ListMatchResponse.matches = null");
				return;
			}
			if (matchList.matches.Count > 0) { //Hay al menos una partida
				int idx = 0;

				if (aleatorio) {
					List<int> publicas = new List<int> ();

					//Busca las partidas publicas
					foreach (var match in matchList.matches) {
						if (!match.isPrivate)
							publicas.Add (idx);
						idx++;
					}

					//Todas las partidas que ha encontrado eran privadas
					if (publicas.Count == 0) {
						//¿Habia mas partidas por buscar?
						if (matchList.matches.Count == partidasPorPagina) {
							paginaOffset += partidasPorPagina;
							gestorRed.matchMaker.ListMatches (paginaOffset, partidasPorPagina, "", UnirseOCrear);
						} else {
							CrearPartida ();
						}
						return;
					}

					//Elije una partida aleatoria de entre las partidas publicas
					idx = publicas [Random.Range (0, publicas.Count)];
				} else {
					if (matchList.matches.Count > 1)
						Debug.LogError ("Hay más de una partida con el mismo nombre");
				}

				//Se une a la partida dado el idx
				//Si la partida no era aleatoria idx será 0
				gestorRed.matchName = matchList.matches [idx].name;
				gestorRed.matchSize = (uint)matchList.matches [idx].maxSize;
				gestorRed.matchMaker.JoinMatch (matchList.matches [idx].networkId, "", gestorRed.TrasUnirseAPartida);

			} else { // No hay partidas, la creo
				CrearPartida ();
			}
		}
	}

	void CrearPartida (){
		//TODO
		string nombre;
		if (aleatorio)
			nombre = "random" + Random.value;
		else {
			nombre = nombreSala.text;
		}
		gestorRed.matchName = nombre;
		//gestorRed.matchSize = (uint)0;
		gestorRed.matchMaker.CreateMatch(nombre, gestorRed.matchSize,true, "", gestorRed.OnMatchCreate);
		Debug.Log ("Creando partida");
	}

	void UnirseAPartidaConContraseña(){
		//TODO
	}
}
