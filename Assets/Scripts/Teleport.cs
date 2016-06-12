using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ParticleSystem))]
[RequireComponent (typeof(Collider2D))]
public class Teleport : MonoBehaviour {

	public Teleport destination;
	public float waitTimeForTeleport = 1.0f;
	public bool teleportOnEnter = true;

	bool teleporting = false;
	ParticleSystem particles;
	GameObject player;

	void Start (){
		particles = gameObject.GetComponent<ParticleSystem> ();
	}

	void FixedUpdate (){
		if (teleporting) {
			if ( player ){
				Vector2 pos = transform.position;
				player.transform.position = pos;
			}
		}
	}

	void OnTriggerEnter2D ( Collider2D other ){
		if (!teleporting) {
			destination.teleporting = true;
			teleporting = true;
			destination.StartEffect();
			StartEffect();
			player = other.gameObject;
			StartCoroutine (SwapPositions ());
		}
	}

	IEnumerator SwapPositions (){
		yield return new WaitForSeconds ( waitTimeForTeleport );
		Vector2 pos = destination.transform.position;
		player.transform.position = pos;
		player = null;
		Invoke ("ResetTeleports", 0.2f);
	}


	void ResetTeleports (){
		destination.teleporting = false;
		teleporting = false;

		destination.StopEffect ();
		StopEffect ();
	}

	void StartEffect (){
		particles.Play ();
	}

	void StopEffect (){
		particles.Stop ();
	}
}
