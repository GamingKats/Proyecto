using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GestorJugadas : MonoBehaviour {

	//Variables de muestra en el canvas.
	public Text tiempo;
	public Text rondas;
	public Text countBack;

	// Use this for initialization
	void Start () {
		InicializoPartida ();
	}
	
	// Update is called once per frame
	void Update () {
		InicializoPartida ();
	}

	void InicializoPartida(){
		tiempo.text = ((int)(GestorMultijugador.singleton.tiempoActual)).ToString ();
		rondas.text = GestorMultijugador.singleton.rondaActual + "/" + GestorMultijugador.singleton.numRondas;
		if (GestorMultijugador.singleton.countBackActual >= 0) {
			countBack.text = ((int)(GestorMultijugador.singleton.countBackActual)).ToString ();
		} else {
			countBack.text = "";
		}

	}
}