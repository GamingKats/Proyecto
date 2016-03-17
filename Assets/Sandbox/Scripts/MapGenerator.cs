using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestSinglePlayer{
	public class MapGenerator : MonoBehaviour{

		public static MapGenerator singleton;
		string[,] currentMap;
		int mapSize = 0;
		public int MapSize { get { return mapSize; } }

		public Texture2D[] bitMap;
		public GameObject dynamicWallPref;
		//public GameObject singleDynamicWallPref;
		public GameObject endAreaPref;
		public GameObject gatePref;
		public GameObject teleportPref;
		public GameObject singleWallPref;

		List<GameObject> dynamicWallsList;
		List<GameObject> gatesList;
		List<GameObject> teleportsList;

		void Start (){
			MapGenerator.ReadMap (bitMap[0]);
			RenderSettings.ambientLight = Color.black;
		}
		void Awake (){
			singleton = this;
		}

		public static void ResetParams (){
			MapGenerator.singleton.dynamicWallsList = new List<GameObject>();
			MapGenerator.singleton.gatesList = new List<GameObject>();
			MapGenerator.singleton.teleportsList = new List<GameObject>();
			MapGenerator.singleton.mapSize = 0;
		}

		public static void ReadMap ( Texture2D map ){
			MapGenerator.ResetParams ();

			Color32[] pix = map.GetPixels32();
			//System.Array.Reverse(pix);

			MapGenerator.singleton.mapSize = map.width;
			MapGenerator.singleton.currentMap = new string[map.width, map.height];
			Color32 c;

			GameObject mapObj = new GameObject ();
			GameObject blockedObj = new GameObject ();
			GameObject dynamicsObj = new GameObject ();
			GameObject teleportsObj = new GameObject ();
			GameObject gatesObj = new GameObject ();
			
			mapObj.name = "Map";
			blockedObj.name = "Blocked";
			dynamicsObj.name = "Dynamics";
			teleportsObj.name = "Teleports";
			gatesObj.name = "Gates";
			blockedObj.transform.parent = mapObj.transform;
			dynamicsObj.transform.parent = mapObj.transform;
			teleportsObj.transform.parent = mapObj.transform;
			gatesObj.transform.parent = mapObj.transform;

			int offset = (MapGenerator.singleton.mapSize -1) / 2;
			int x, y;
			for (int i=0; i< map.height; i++) {
				y = i-offset;
				for ( int j=0; j< map.width; j++){
					x = j-offset;
					c = pix[i*(map.height)+j];
					/*************************************************************
					 ******************** CAMINO LIBRE ***************************
					 ************************************************************/
					if ( c == Color.white ){
						MapGenerator.singleton.currentMap[i,j] = "0";
						continue;
					}
					/*************************************************************
					 ******************** PARED FIJA *****************************
					 ************************************************************/
					if ( c == Color.black ){
						MapGenerator.singleton.currentMap[i,j] = "1";
						MapGenerator.PlaceWall( blockedObj.transform, x,y);
						continue;
					}
					/*************************************************************
					 ******************** PARED DINAMICA**************************
					 ************************************************************/
					if ( c.r == c.g && c.r == c.b ){
						MapGenerator.singleton.currentMap[i,j] = "d"+c.r;
						MapGenerator.PlaceDynamicWalls( dynamicsObj.transform,(int)c.r, x,y);
						continue;
					}
					/*************************************************************
					 ********************** TELEPORTS ****************************
					 ************************************************************/
					if ( c.r == c.b && c.g == 0){//Pinks
						MapGenerator.singleton.currentMap[i,j] = "tp"+c.r;
						MapGenerator.PlaceTeleports ( teleportsObj.transform,(int)c.r, x,y);
						continue;
					}
					/*************************************************************
					 ************************** GATES ****************************
					 ************************************************************/
					if ( c.g == c.b && c.r == 0){//Cyans
						MapGenerator.singleton.currentMap[i,j] = "g";
						MapGenerator.PlaceGates ( gatesObj.transform,(int)c.g, x,y);
						continue;
					}
					/*************************************************************
					 *********************** META ********************************
					 ************************************************************/
					if ( c == Color.red ){
						MapGenerator.singleton.currentMap[i,j] = "m";
						MapGenerator.PlaceEndArea( mapObj.transform, x,y);
						continue;
					}
					/*************************************************************
					 ******************* TRIANGLES SPAWN *************************
					 ************************************************************/
					if ( c == Color.blue ){
						MapGenerator.singleton.currentMap[i,j] = "st";
						//TODO
						continue;
					}
					/*************************************************************
					 ********************* SPHERES SPAWN *************************
					 ************************************************************/
					if ( c == Color.green ){
						MapGenerator.singleton.currentMap[i,j] = "ss";
						//TODO
						continue;
					}
				}
			}
		}

		private static void PlaceEndArea ( Transform parent, int x, int y ){
			GameObject go = Instantiate (MapGenerator.singleton.endAreaPref);
			go.transform.position = new Vector3 ( x, y , 1);
			go.transform.parent = parent;
			go.name = "EndArea";
			go.tag = "EndArea";
		}

		private static void PlaceWall ( Transform parent, int x, int y ){

			GameObject go = Instantiate (MapGenerator.singleton.singleWallPref);
			go.transform.position = new Vector3 ( x, y , 0);
			go.transform.parent = parent;
			go.name = "wall";
		}

		private static void PlaceDynamicWalls ( Transform dynamics, int key, int x, int y){
			GameObject pair = null;
			foreach (GameObject d in MapGenerator.singleton.dynamicWallsList) {
				if ( d.name == "d"+key ){
					pair = d;

					GameObject wall = Instantiate (MapGenerator.singleton.singleWallPref);
					wall.transform.position = new Vector2 (x, y);
					wall.transform.parent = pair.transform;
					wall.name = "dWall2";

					DynamicWall dw = pair.GetComponent<DynamicWall>();
					dw.SetChildWalls();
					dw.SetRandom();
					break;
				}
			}

			if (pair == null) {

				pair = Instantiate(MapGenerator.singleton.dynamicWallPref);
				pair.transform.parent = dynamics;
				pair.transform.position = Vector2.zero;
				pair.name = "d"+key;

				GameObject wall = Instantiate (MapGenerator.singleton.singleWallPref);
				wall.transform.position = new Vector2 (x, y);
				wall.transform.parent = pair.transform;
				wall.name = "dWall1";

				MapGenerator.singleton.dynamicWallsList.Add( pair );
			}

		}

		private static void PlaceTeleports ( Transform teleport, int key, int x, int y){
			GameObject pair = null;
			foreach (GameObject d in MapGenerator.singleton.teleportsList) {
				if ( d.name == "tp"+key ){
					pair = d;
					
					GameObject wall = Instantiate (MapGenerator.singleton.teleportPref);
					wall.transform.position = new Vector3 (x, y, 0.9f);
					wall.transform.parent = pair.transform;
					wall.name = "tp2";
					
					Teleport tp1 = wall.GetComponent<Teleport>();
					Teleport tp2 = pair.transform.GetChild(0).gameObject.GetComponent<Teleport>();
					tp1.destination = tp2;
					tp2.destination = tp1;
					break;
				}
			}
			
			if (pair == null) {
				
				pair = new GameObject();
				pair.transform.parent = teleport;
				pair.transform.position = Vector2.zero;
				pair.name = "tp"+key;
				
				GameObject wall = Instantiate (MapGenerator.singleton.teleportPref);
				wall.transform.position = new Vector3 (x, y, 0.9f);
				wall.transform.parent = pair.transform;
				wall.name = "tp1";
				
				MapGenerator.singleton.teleportsList.Add( pair );
			}
		}

		private static void PlaceGates ( Transform gate, int key, int x, int y){
			GameObject pair = null;
			foreach (GameObject d in MapGenerator.singleton.gatesList) {
				if ( d.name == "g"+key ){
					pair = d;
					
					GameObject wall = Instantiate (MapGenerator.singleton.gatePref);
					wall.transform.position = new Vector2 (x, y);
					wall.transform.parent = pair.transform;
					wall.name = "gate2";
					
					Gate tp1 = wall.GetComponent<Gate>();
					Gate tp2 = pair.transform.GetChild(0).gameObject.GetComponent<Gate>();
					tp1.other = tp2;
					tp2.other = tp1;
					break;
				}
			}
			
			if (pair == null) {
				
				pair = new GameObject();
				pair.transform.parent = gate;
				pair.transform.position = Vector2.zero;
				pair.name = "g"+key;
				
				GameObject wall = Instantiate (MapGenerator.singleton.gatePref);
				wall.transform.position = new Vector2 (x, y);
				wall.transform.parent = pair.transform;
				wall.name = "gate1";
				
				MapGenerator.singleton.gatesList.Add( pair );
			}
		}
	}
}
