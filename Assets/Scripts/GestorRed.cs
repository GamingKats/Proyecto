using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class GestorRed : NetworkManager {


	public GameObject GeneradorMapasPref;
	static short jugadores = 0;

	public class UserMessage : MessageBase
	{
		public string name;
		//public string team;
	}

	public void TrasUnirseAPartida (JoinMatchResponse matchInfo){
		if (matchInfo.success){
			Debug.Log ( "Me estoy uniendo a una partida ya creada" );
		}
		base.OnMatchJoined(matchInfo);
	}


	public override void OnMatchCreate(CreateMatchResponse matchInfo)
	{
		if (matchInfo.success) {
			base.OnMatchCreate(matchInfo);
			//GameObject party = (GameObject)Instantiate (spawnPrefabs[0], Vector3.zero, Quaternion.identity);
			//NetworkServer.Spawn(party);
			GameObject generador = Instantiate(GeneradorMapasPref);
		}
	}

	/*
	public override void OnClientSceneChanged(NetworkConnection conn){
		if (conn.isReady)
			return;
		//Add Player
		base.OnClientSceneChanged (conn);
	}
	//Called ON CLIENT
	public override void OnClientConnect(NetworkConnection conn)
	{
		//Debug.Log ("Onclientconnect 0 ");
		UserMessage um = new UserMessage ();
		GameObject user = GameObject.FindWithTag("UserName");
		if (user != null) {
			um.name = user.name;
		} else {
			Debug.LogError ("No encuentra el user name");
		}
		Destroy (user);
		ClientScene.Ready (conn);
		ClientScene.AddPlayer(conn, 0, um);
		//Debug.Log ("Onclientconnect 1 ");
		///////////By Default do this....
		//if (string.IsNullOrEmpty (this.onlineScene) || this.onlineScene == this.offlineScene)
		//{
		//	ClientScene.Ready (conn);
		//	if (this.autoCreatePlayer)
		//	{
		//		ClientScene.AddPlayer (0);
		//	}
		//}
	}

	//Called ON SERVER

	public override void OnServerDisconnect(NetworkConnection conn){
		Debug.Log (conn.playerControllers.Count);
		foreach (PlayerController pc in conn.playerControllers) {
			if ( pc.unetView == null )
				continue;
			//GameManager.RemovePlayer(  pc.unetView.netId.Value  );
		}
		base.OnServerDisconnect (conn);
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		if (extraMessageReader != null) {
			//Debug.Log ("Onserveraddplayer 0 ");
			string s = extraMessageReader.ReadString ();
			//playerPrefab.name = s;
			GameObject prefab = (GameObject)Instantiate (playerPrefab, Vector3.zero, Quaternion.identity);
			prefab.name = s;
			//prefab.GetComponent<OnlineGUI> ().userName = s;

			short playerId = jugadores;
			NetworkServer.AddPlayerForConnection (conn, prefab, playerId);
			jugadores++;

		}
	}

	//---------------------------------------------------------------------------------------------------------------
	void OriginalOnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader){
		if (playerPrefab == null)
		{
			if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
			return;
		}

		if (playerPrefab.GetComponent<NetworkIdentity>() == null)
		{
			if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
			return;
		}

		if (playerControllerId < conn.playerControllers.Count  && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
		{
			if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
			return;
		}

		GameObject player;
		Transform startPos = GetStartPosition();
		if (startPos != null)
		{
			player = (GameObject)GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
		}
		else
		{
			player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		}

		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}*/
}
