using UnityEngine;
using System.Collections;

namespace TestSinglePlayer{
	public class GateChilds : MonoBehaviour {
		
		
		void OnTriggerEnter2D(Collider2D other){
			SendMessageUpwards ("TriggerEnter"+name, other );
		}
		void OnTriggerExit2D(Collider2D other){
			SendMessageUpwards ("TriggerExit"+name, other );
		}
	}
}