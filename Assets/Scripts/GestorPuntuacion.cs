using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GestorPuntuacion : MonoBehaviour {

	int maxPlayers = 20;
	int numEquipo1 =0;
	int numEquipo2 =0;

	public Text salida;

	// Use this for initialization
	void Start () {
		generaPuntuacion ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Busca todos los objetos de tipo Player y muestra su puntuación
	public void generaPuntuacion(){
		Jugador[] equipo1 = new Jugador[maxPlayers];
		Jugador[] equipo2 = new Jugador[maxPlayers];

		GameObject[] jugadores = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject objeto in jugadores) {
			Jugador jugador = objeto.GetComponent<Jugador> ();
			if (jugador.estaEnEquipo1) {
				equipo1 [numEquipo1] = jugador;
				numEquipo1++;
			} else {
				equipo2 [numEquipo2] = jugador;
				numEquipo2++;
			}
		}

		Debug.Log ("Equipo1");
		for (int i = 0; i < equipo1.Length; i++) {
			salida .text = salida.text + ("Equipo 1: " + equipo1[i].nombreUsuario + " - " +equipo1 [i].puntosEscapada + " - " + equipo1 [i].puntosPillada);
		}
		Debug.Log ("Equipo2");
		for (int i = 0; i < equipo2.Length; i++) {
			salida .text = salida.text + ("Equipo 2:" + equipo2[i].nombreUsuario + " - " +equipo2 [i].puntosEscapada + " - " + equipo2 [i].puntosPillada);
		}
	}
}
