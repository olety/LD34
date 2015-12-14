using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	float screenWidth;
	public Vector2 minLevelCoords = new Vector2(-200,-200);
	public Vector2 maxLevelCoords = new Vector2(200,200);
	public HeroController player;
	public bool startInStartMenu = true;
	public bool isPaused;
	public bool isInStartMenu = true;
	public GameObject startCanvas;
	public GameObject winCanvas;
	public GameObject loseCanvas;
	public GameObject pauseCanvas;
	public GameObject startAboutCanvas;
	public GameObject winAboutCanvas;
	public GameObject loseAboutCanvas;
	public GameObject pauseAboutCanvas;
	float tempTimeScale;
	void Awake(){
		screenWidth = Screen.width;
		screenHeight = Screen.height;	
	}

	void Start(){
		Time.timeScale = 1f;
		if (startInStartMenu) {
			showStartMenu ();	
		}
	}

	void Update(){
		if (Input.GetButtonDown("Pause") && Time.timeScale!=0f){
			flipPause();
		}
	}

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


	public void Lose(){
		//TODO: Implement
		Debug.Log ("Lose requested");
		Time.timeScale = 0f;
		loseCanvas.gameObject.SetActive (true);
	}

	public void Win(){
		//TODO: Implement
		Debug.Log ("Win requested");
		Time.timeScale = 0f;
		winCanvas.gameObject.SetActive (true);

	}

	public void flipPause(){
		if (isPaused) {
			unPauseGame ();
		} else {
			pauseGame ();
		}
	}

	public void pauseGame(){
		Debug.Log ("Pausing game");
		isPaused = true;
		tempTimeScale = Time.timeScale;
		pauseCanvas.gameObject.SetActive (true);
		Time.timeScale = 0f;
	}

	public void unPauseGame(){
		Debug.Log ("Unpausing game");
		isPaused = false;
		pauseCanvas.gameObject.SetActive (false);
		Time.timeScale = tempTimeScale;
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
//		Time.timeScale = 1f;
	}

	public void Quit (){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}


	void showStartMenu(){
		Time.timeScale = 0f;
		isInStartMenu = true;
		startCanvas.SetActive (true);
	}

	void hideStartMenu( bool resetTime ){
		startCanvas.SetActive (false);
		if (resetTime) {
			Time.timeScale = 1f;
		}
	}

	public void startAbout(){
		startCanvas.SetActive (false);
		startAboutCanvas.SetActive (true);
	}

	public void startAboutBack(){
		startAboutCanvas.SetActive (false);
		startCanvas.SetActive (true);
	}

	public void winAbout(){
		winCanvas.SetActive (false);
		winAboutCanvas.SetActive (true);
	}

	public void winAboutBack(){
		winAboutCanvas.SetActive (false);
		winCanvas.SetActive (true);
	}

	public void loseAbout(){
		loseCanvas.SetActive (false);
		loseAboutCanvas.SetActive (true);
	}

	public void loseAboutBack(){
		loseAboutCanvas.SetActive (false);
		loseCanvas.SetActive (true);
	}

	public void pauseAbout(){
		pauseCanvas.SetActive (false);
		pauseAboutCanvas.SetActive (true);
	}

	public void pauseAboutBack(){
		pauseAboutCanvas.SetActive (false);
		pauseCanvas.SetActive (true);
	}

}
