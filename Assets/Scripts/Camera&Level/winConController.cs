﻿using UnityEngine;
using System.Collections;

public class winConController : MonoBehaviour {
	public GameObject player;
	public LevelController level;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x >= this.transform.position.x){
			level.Win();
		}
	}
}
