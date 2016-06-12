using UnityEngine;
using System.Collections;

public class ParedDinamica : MonoBehaviour {

	GameObject pared1, pared2;

	public void AsignarParedes (){
		pared1 = transform.GetChild (0).gameObject;
		pared2 = transform.GetChild (1).gameObject;
	}

	public void EstablecerVisible ( bool pared1Visible ){
		pared1.SetActive (pared1Visible);
		pared2.SetActive (!pared1Visible);
	}
}
