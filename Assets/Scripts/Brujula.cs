using UnityEngine;
using System.Collections;

public class Brujula : MonoBehaviour {

	GameObject meta = null;
	RectTransform rect;
	GameObject aux;
	// Use this for initialization
	void Start () {
		rect = GetComponent<RectTransform> ();
		aux = new GameObject ();
		transform.parent.gameObject.GetComponent<Canvas> ().worldCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.rotation = Quaternion.identity;
		if (meta == null) {
			meta = GameObject.FindWithTag ("Meta");
		} else {
			aux.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
			aux.transform.LookAt (meta.transform.position);

			Vector3 diff = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, 0) - meta.transform.position;
			diff.Normalize();

			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

			rect.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
		}
	}
}
