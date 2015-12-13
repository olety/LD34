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

	public void Lose(){
		//TODO: Implement
		Debug.Log ("Lose requested");
		LoadScene ("Lose");
	}

	public void Win(){
		//TODO: Implement
		Debug.Log ("Win requested");

	}

	public void pauseGame(){
		Debug.Log ("Pausing game");
		Time.timeScale = 0f;
	}

	public void unPauseGame(){
		Debug.Log ("Unpausing game");
		Time.timeScale = 1f;
	}

	public void slowMo( float slownessRatio ){
		Debug.Log ("Enabling SlowMo , ratio = " + slownessRatio);
		Time.timeScale = slownessRatio;
	}

	public void unSlowMo (){
		Debug.Log ("Disabling SlowMo");
		Time.timeScale = 1f;
	}

	public void LoadScene ( string sceneName ){
		Debug.Log ("Load requested for scene " + sceneName);
		Application.LoadLevel (sceneName);
	}

	public void Quit (){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}


}
