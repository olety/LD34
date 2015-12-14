using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour {
	public Text text;
	public HeroController player;
	string defaultText;
	// Use this for initialization
	void Start () {
		defaultText = text.text;
	}
	void updateText(){
		this.text.text = defaultText + " " + ((int)player.Score).ToString() + "/" + ((int)player.maxSize).ToString();
	}
	// Update is called once per frame
	void Update () {
		updateText ();
	}
}
