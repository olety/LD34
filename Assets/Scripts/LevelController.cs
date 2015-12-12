using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	float screenWidth;

	public float ScreenWidth {
		get {
			return screenWidth;
		}
	}

	float screenHeight;

	public float ScreenHeight {
		get {
			return screenHeight;
		}
	}

	void Awake() {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
