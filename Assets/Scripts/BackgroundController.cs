using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundController : MonoBehaviour {
	Object background;
	public float scrollSpeed;
	List<GameObject> backgrounds;
	List<Vector3> startPositions;
	Vector3 viewPort, bottomLeft, topRight;
	float camHeight, camWidth;
	// Use this for initialization
	void Awake () { 
		updateCameraProperties ();
		Debug.Log ("Trying to load Background from Prefabs/Background");
		if (!(background = Resources.Load ("Background", typeof(GameObject)))) {
			Debug.LogError("Couldn't load background");
		}
		backgrounds = new List<GameObject>();
		startPositions = new List<Vector3> ();
	}

	void updateCameraProperties () {
		viewPort = new Vector3(0,0,0);
		bottomLeft = Camera.main.ViewportToWorldPoint(viewPort);
		viewPort.Set(1,1,1);
		topRight = Camera.main.ViewportToWorldPoint(viewPort);
		Debug.Log ("Camera botLeft : " + bottomLeft);
		Debug.Log ("Camera topRight : " + topRight);
		camHeight = topRight.y - bottomLeft.y;
		camWidth = topRight.x - bottomLeft.x;
	}

	Vector3 getNewBGScale (){
		return new Vector3 (camHeight, camHeight, 1);
	}


	void Start(){
		int numCopies = Mathf.CeilToInt (camWidth/camHeight)+2;
		for (int i = 0; i < numCopies; i++) {
			startPositions.Add(new Vector3(bottomLeft.x+(i-1)*camHeight , 0, 0 ));
			Debug.Log("Creating a new background object at : " + startPositions);
			backgrounds.Add(Instantiate( background, startPositions[i], Quaternion.identity ) as GameObject); 
			backgrounds[i].transform.localScale = getNewBGScale();
		}
	}

	// Update is called once per frame
	void Update () {
		int i = 0;
		foreach (GameObject bg in backgrounds) {
			float newPosMult = Mathf.Repeat (Time.time * scrollSpeed, camHeight);
			bg.transform.position = startPositions[i] + Vector3.left * newPosMult;
			i++;
		}
	}

	void OnDestroy(){
		backgrounds.Clear ();
		startPositions.Clear ();
	}
}
