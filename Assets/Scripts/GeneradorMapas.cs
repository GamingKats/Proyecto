using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class GeneradorMapas : NetworkBehaviour {

	//-------	VARIABLES SINCRONIZADAS CON LOS CLIENTES
	//public SyncListBool randomDinamicas = new SyncListBool(); // No consigo que me funcione
	[SyncVar]
	public int  numeroDinamicas = 0;
	[SyncVar]
	public int mapa = 0;
	//-------------------------------------------------------------

	string[,] currentMap;
	int mapSize = 0;
	int paredesDinamicas = 0;
	public int MapSize { get { return mapSize; } }

	public Texture2D[] texturas;//bitMap
	public GameObject paredDinamicaPref;//dynamicWallPref;
	public GameObject metaPref;//endAreaPref;
	public GameObject puertaPref;//gatePref;
	public GameObject teleportPref;
	public GameObject paredPref;//singleWallPref;
	public GameObject posicionIniEsfPref;
	public GameObject posicionIniTriPref;

	List<GameObject> dynamicWallsList;
	List<GameObject> gatesList;
	List<GameObject> teleportsList;


	void Start (){
		DontDestroyOnLoad (gameObject);
	}

	public void CrearMapa (){
		
		Debug.LogError ("Numero dinamicas: " + numeroDinamicas);
		Debug.LogError ("Mapa: " + mapa);
		paredesDinamicas = 0;

		dynamicWallsList = new List<GameObject>();
		gatesList = new List<GameObject>();
		teleportsList = new List<GameObject>();
		mapSize = 0;
		GameObject mapaaux = GameObject.Find ("Map");
		if ( mapaaux != null )
			Destroy (mapaaux);
		ReadMap (texturas [mapa]);
		RenderSettings.ambientLight = Color.black;
	}

	public void ReiniciarValores (){
		mapa = Random.Range(0, texturas.Length);
		numeroDinamicas = Random.Range ( 0, 999999 );

	}

	void ReadMap ( Texture2D map ){
		
		Color32[] pix = map.GetPixels32();
		//System.Array.Reverse(pix);

		mapSize = map.width;
		Debug.Log (map.width + ", " + map.height);
		currentMap = new string[map.width, map.height];
		Color32 c;

		GameObject mapObj = new GameObject ("Map");
		GameObject blockedObj = new GameObject ("Blocked");
		GameObject dynamicsObj = new GameObject ("Dynamics");
		GameObject teleportsObj = new GameObject ("Teleports");
		GameObject gatesObj = new GameObject ("Gates");
		GameObject spawnsObj = new GameObject ("Spawns");
		GameObject spawnsTri = new GameObject ("SpawnTris");
		GameObject spawnsSph = new GameObject ("SpawnSph");


		blockedObj.transform.parent = mapObj.transform;
		dynamicsObj.transform.parent = mapObj.transform;
		teleportsObj.transform.parent = mapObj.transform;
		gatesObj.transform.parent = mapObj.transform;
		spawnsObj.transform.parent = mapObj.transform;
		spawnsSph.transform.parent = spawnsObj.transform;
		spawnsTri.transform.parent = spawnsObj.transform;

		int offset = (mapSize -1) / 2;
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
					currentMap[i,j] = "0";
					continue;
				}
				/*************************************************************
					 ******************** PARED FIJA *****************************
					 ************************************************************/
				if ( c == Color.black ){
					currentMap[i,j] = "1";
					PlaceWall( blockedObj.transform, x,y);
					continue;
				}
				/*************************************************************
					 ******************** PARED DINAMICA**************************
					 ************************************************************/
				if ( c.r == c.g && c.r == c.b ){
					currentMap[i,j] = "d"+c.r;
					PlaceDynamicWalls( dynamicsObj.transform,(int)c.r, x,y);
					continue;
				}
				/*************************************************************
					 ********************** TELEPORTS ****************************
					 ************************************************************/
				if ( c.r == c.b && c.g == 0){//Pinks
					currentMap[i,j] = "tp"+c.r;
					PlaceTeleports ( teleportsObj.transform,(int)c.r, x,y);
					continue;
				}
				/*************************************************************
					 ************************** GATES ****************************
					 ************************************************************/
				if ( c.g == c.b && c.r == 0){//Cyans
					currentMap[i,j] = "g";
					PlaceGates ( gatesObj.transform,(int)c.g, x,y);
					continue;
				}
				/*************************************************************
					 *********************** META ********************************
					 ************************************************************/
				if ( c == Color.red ){
					currentMap[i,j] = "m";
					PlaceEndArea( mapObj.transform, x,y);
					continue;
				}
				/*************************************************************
					 ******************* TRIANGLES SPAWN *************************
					 ************************************************************/
				if ( c == Color.blue ){
					currentMap[i,j] = "st";
					PlaceSpawn (spawnsTri.transform, true, x, y);
					continue;
				}
				/*************************************************************
					 ********************* SPHERES SPAWN *************************
					 ************************************************************/
				if ( c == Color.green ){
					currentMap[i,j] = "ss";
					PlaceSpawn (spawnsSph.transform, false, x, y);
					continue;
				}
			}
		}
	}

	private void PlaceEndArea ( Transform parent, int x, int y ){
		GameObject go = Instantiate (metaPref);
		go.transform.position = new Vector3 ( x, y , 0);
		go.transform.parent = parent;
		go.name = "Meta";
	}

	private void PlaceWall ( Transform parent, int x, int y ){

		GameObject go = Instantiate (paredPref);
		go.transform.position = new Vector3 ( x, y , 0);
		go.transform.parent = parent;
		go.name = "wall";
	}

	private void PlaceSpawn ( Transform parent, bool triangle, int x, int y){
		GameObject go;
		if (triangle) {
			go = Instantiate (posicionIniTriPref);
			go.name = "PosicionInicialTriangulo";

		} else {
			go = Instantiate (posicionIniEsfPref);
			go.name = "PosicionInicialEsfera";
		}
		go.transform.position = new Vector3 ( x, y , 0);
		go.transform.parent = parent;

	}
	private void PlaceDynamicWalls ( Transform dynamics, int key, int x, int y){
		GameObject pair = null;
		foreach (GameObject d in dynamicWallsList) {
			if ( d.name == "d"+key ){
				pair = d;

				GameObject wall = Instantiate (paredDinamicaPref);
				wall.transform.position = new Vector2 (x, y);
				wall.transform.parent = pair.transform;
				wall.name = "dWall2";

				ParedDinamica dw = pair.GetComponent<ParedDinamica>();
				dw.AsignarParedes();
				bool visible = (numeroDinamicas & 1<<paredesDinamicas) == 1<<paredesDinamicas;
				dw.EstablecerVisible( visible );

				paredesDinamicas++;
				break;
			}
		}

		if (pair == null) {

			pair = Instantiate(paredDinamicaPref);
			pair.transform.parent = dynamics;
			pair.transform.position = Vector2.zero;
			pair.name = "d"+key;

			GameObject wall = Instantiate (paredPref);
			wall.transform.position = new Vector2 (x, y);
			wall.transform.parent = pair.transform;
			wall.name = "dWall1";

			dynamicWallsList.Add( pair );
		}

	}

	private void PlaceTeleports ( Transform teleport, int key, int x, int y){
		GameObject pair = null;
		foreach (GameObject d in teleportsList) {
			if ( d.name == "tp"+key ){
				pair = d;

				GameObject wall = Instantiate (teleportPref);
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

			GameObject wall = Instantiate (teleportPref);
			wall.transform.position = new Vector3 (x, y, 0.9f);
			wall.transform.parent = pair.transform;
			wall.name = "tp1";

			teleportsList.Add( pair );
		}
	}

	private void PlaceGates ( Transform gate, int key, int x, int y){
		GameObject pair = null;
		foreach (GameObject d in gatesList) {
			if ( d.name == "g"+key ){
				pair = d;

				GameObject wall = Instantiate (puertaPref);
				wall.transform.position = new Vector2 (x, y);
				wall.transform.parent = pair.transform;
				wall.name = "gate2";

				Puerta tp1 = wall.GetComponent<Puerta>();
				Puerta tp2 = pair.transform.GetChild(0).gameObject.GetComponent<Puerta>();
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

			GameObject wall = Instantiate (puertaPref);
			wall.transform.position = new Vector2 (x, y);
			wall.transform.parent = pair.transform;
			wall.name = "gate1";

			gatesList.Add( pair );
		}
	}
}
