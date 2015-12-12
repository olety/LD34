using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

	public List<GameObject> enemyTypes;
	public float minDistanceBetween, maxDistanceBetween;
	public float numEnemies;
	public float startPosX = 0, startPosY = 0;
	// Use this for initialization
	void Start () {
		Vector3 pos = new Vector3(startPosX, startPosY,0 );
		for (int i = 0; i < numEnemies; i++) {
			spawnRandomEnemyAtPos(pos);
			pos.x += Random.Range(minDistanceBetween, maxDistanceBetween);
			Debug.Log(pos);
		}
	}

	void spawnEnemy( int num, Vector3 pos ){
		Debug.Log ("Spawning an enemy of a type " + enemyTypes[num].name + " at " + pos);
		Instantiate(enemyTypes[num], pos, Quaternion.identity);
	}

	void spawnRandomEnemyAtPos( Vector3 pos ){
		spawnEnemy (Random.Range (0, this.enemyTypes.Count), pos);
	}

	void spawnRandomEnemy(){
		//TODO: Implement
	//	Vector3 pos = new Vector3 (Random.Range();
	//	spawnRandomEnemyAtPos (pos);
	}
}
