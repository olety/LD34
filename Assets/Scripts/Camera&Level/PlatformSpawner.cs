using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;

public class PlatformSpawner : MonoBehaviour {
	public bool generatePlatforms = true;
	public bool infiniteMode = false;
	public string platformName = "Platform";
	public int numPlatforms = 5;
	public Vector3 startPos = new Vector3 (0, 0, 0);
	public float xOffset = 0f;
	public float yOffset = -1f;
	Object platform;
	SpriteRenderer rend;
	Vector2 platformSize;

	void Awake() {
		if ((platform = Resources.Load (platformName, typeof(GameObject))) == null ) {
			Debug.LogError("Couldn't load a platform object from Resources folder");
		}

		if ( (rend = ((GameObject)platform).GetComponent<SpriteRenderer>()) == null ){
			Debug.LogError("Couldn't load a platform collider in PlatformSpawner");
		}
	}

	// Use this for initialization
	void Start () {
		if (generatePlatforms) {
			platformSize = new Vector2 (rend.bounds.size.x, rend.bounds.size.y);
//			Debug.Log("Initializing platformSize = " + platformSize);
			if (infiniteMode) {
				numPlatforms = -1;
				//TODO: Implement
			} else {
				for (int i = 0; i < numPlatforms; i++) {
					Instantiate (platform, startPos, Quaternion.identity);
//					Debug.Log("Creating a new platform at : " + startPos);
					startPos.x += platformSize.x + xOffset;
					startPos.y += platformSize.y + yOffset;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
