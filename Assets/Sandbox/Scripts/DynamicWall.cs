using UnityEngine;
using System.Collections;

namespace TestSinglePlayer{
	public class DynamicWall : MonoBehaviour {
		
		GameObject wall1,wall2;

		public void SetChildWalls (){
			wall1 = transform.GetChild (0).gameObject;
			wall2 = transform.GetChild (1).gameObject;
		}

		public void SetWallsPrefabs ( GameObject w1, GameObject w2 ){
			wall1 = w1;
			wall2 = w2;

		}

		public void SetRandom (){
			int i = Random.Range (0, 2);

			wall1.SetActive (i == 0);
			wall2.SetActive (i != 0);
		}

		public void SetActive(bool wall2){
			if (wall2) {
				this.wall2.SetActive (true);
				wall1.SetActive(false);
			} else {
				wall1.SetActive(true);
				this.wall2.SetActive(false);
			}
		}

		public void SwapWalls (){
			wall1.SetActive ( !wall1.activeSelf );
			wall2.SetActive ( !wall2.activeSelf );
		}
	}
}
