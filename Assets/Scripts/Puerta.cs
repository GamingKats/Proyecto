using UnityEngine;
using System.Collections;

public class Puerta : MonoBehaviour {
	public Puerta other; // the player gate

	bool teleporting = false;
	Vector2 from;

	void OnTriggerEnter2D ( Collider2D col ){
		if (!teleporting) { 
			from = col.transform.position;
			teleporting = true;
		}
	}

	void OnTriggerExit2D ( Collider2D col ){
		/*Vector2 vector;
			vector.x = col.transform.position.x - transform.position.x;
			vector.y = col.transform.position.y - transform.position.y;
			if (Mathf.Abs (vector.x) > Mathf.Abs (vector.y)) {
				vector.y = 0;
			} else {
				vector.x = 0;
			}*/
		if (Vector2.Distance (from, col.transform.position) > 1) {
			Vector2 pos = Vector2.zero;
			pos.x = other.transform.position.x + ( col.transform.position.x - transform.position.x );
			pos.y = other.transform.position.y + ( col.transform.position.y - transform.position.y );
			teleporting = false;
			col.transform.position = pos;
		}
	}

}
