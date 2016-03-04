using UnityEngine;
using System.Collections;

namespace TestCodes{
	public class LightCheat : MonoBehaviour {
		
		int pos = 0;
		public GameObject dirLight;
		
		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown (KeyCode.L))  {
				pos = 1;
			}
			if (Input.GetKeyDown (KeyCode.I)) {
				if ( pos == 1 )
					pos = 2;
				else
					pos = 0;
			}
			if (Input.GetKeyDown (KeyCode.G)) {
				if ( pos == 2 )
					pos = 3;
				else
					pos = 0;
			}
			if (Input.GetKeyDown (KeyCode.H)) {
				if ( pos == 3 )
					pos = 4;
				else
					pos = 0;
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				if ( pos == 4 ){
					pos = 5;
					dirLight.SetActive( !dirLight.activeSelf );
				}else
					pos = 0;
			}
		}
	}
}
