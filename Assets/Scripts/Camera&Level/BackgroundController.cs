using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundController : MonoBehaviour {
	public float scrollSpeed;
	CameraProperties props;
	Object background;
	List<GameObject> backgrounds;
	List<Vector3> startPositions;

	void Awake () { 
		props = new CameraProperties();
		Debug.Log ("Trying to load Background from Resources/Background");
		if (!(background = Resources.Load ("Background", typeof(GameObject)))) {
			Debug.LogError("Couldn't load background");
		}
		backgrounds = new List<GameObject>();
		startPositions = new List<Vector3> ();
	}
	
	void Start(){
		int numCopies = Mathf.CeilToInt (props.CamWidth/props.CamHeight)+2; // we need n+2 copies of background object, where n is the screen height
		// 1 |           [play area]             | 1 (left and right to be sure that we won't show blue unity bg when moving ours)
		//   | n obj. with len = props.camHeight |
		for (int i = 0; i < numCopies; i++) {
			startPositions.Add(new Vector3(props.BottomLeft.x+(i-0.5f)*props.CamHeight , 0, 0 ));
//			Debug.Log("Creating a new background object at : " + startPositions[i]);
			backgrounds.Add(Instantiate( background, startPositions[i], Quaternion.identity ) as GameObject); 
			backgrounds[i].transform.localScale = props.GetBackgroundScale;
		}
		InvokeRepeating ("updateStartPositions", 1.0f, 1.0f);
	}

	void updateStartPositions(){
		for ( int i = 0; i < startPositions.Count; i++){
			updateStartPosition(i);
		}
	}

	void updateStartPosition(int i){
		startPositions[i] = new Vector3(props.BottomLeft.x+(i-0.5f)*props.CamHeight , 0, 0 );
	}

	// Update is called once per frame
	void Update () {
//		props.updateCameraProperties ();
		int i = 0;
		foreach (GameObject bg in backgrounds) {
			float newPosMult = Mathf.Repeat (Time.time * scrollSpeed, props.CamHeight);
			bg.transform.position = startPositions[i] + Vector3.left * newPosMult;
			i++;
		}
	}

	void OnDestroy(){
		backgrounds.Clear ();
		startPositions.Clear ();
	}
}
