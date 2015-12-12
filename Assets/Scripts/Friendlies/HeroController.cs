using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;

public class HeroController : MonoBehaviour {
	public CameraProperties camProps;
	public LevelController level;
	public float horSpeed;
	public string spikeName = "Spike";

	Rigidbody2D rigidbody;
	BoxCollider2D collider;
	PolygonCollider2D spikeCollider;
	Vector2 spikeSize;
	Object spike;
	bool firingSpike;
	Vector2 velocity;

	//SIZE PACKAGE BEGINS HERE
	public float startSize = 10f;
	public float maxSize = 200f;
	public float spikeSizeCost = 10f;
	public float slamSizeCost = 30f;

	float size;
	
	public float Size {
		get {
			return size;
		}
	}

	static float sizeToScaleRatio;

	public static float SizeToScaleRatio {
		get {
			return sizeToScaleRatio;
		}
	}

	public void incSize ( float amount ){
		Debug.Log ("Hero size inc requested, amount = " + amount);
		this.setSize (this.size + amount);
	}
	
	public void decSize ( float amount ){
		Debug.Log ("Hero size dec requested, amount = " + amount);
		this.setSize (this.size - amount);
	}

	public void setSize ( float amount ){
		Debug.Log ("Hero size set requested, amount = " + amount);
		if (amount <= 0f) {
			level.Lose();
		}
		this.size = Mathf.Clamp (amount, 0, this.maxSize);
		this.updateSize ();
	}

	void updateSize(){
		Vector3 newScale = new Vector3 (size * sizeToScaleRatio, size * sizeToScaleRatio, 0f);
		Debug.Log ("Hero size update requested. NewScale = " + newScale + " x :" + newScale.x);
		this.transform.localScale = newScale;
		updateCollSize();
	}

	Vector2 collSize;
	
	public Vector2 CollSize {
		get {
			return collSize;
		}
	}
	
	void updateCollSize(){
		collSize.x = this.collider.bounds.size.x;
		collSize.y = this.collider.bounds.size.y;
		Debug.Log ("Hero collider update requested. x = " + collSize.x + " ,y = " + collSize.y); 
	}

	//SIZE PACKAGE ENDS HERE

	void processPickup ( PickupController.pickupMessage msg){
		Debug.Log ("Hero picked up a pickup. size = " + msg.size + " type = " + msg.type);
		incSize (msg.size);
	}


	void Awake () { 
		this.rigidbody = this.GetComponent<Rigidbody2D> ();
		this.collider = this.gameObject.GetComponent<BoxCollider2D> ();
		sizeToScaleRatio = this.transform.localScale.x / startSize;
		collSize = new Vector2 (this.collider.bounds.size.x, this.collider.bounds.size.y);
		updateCollSize ();
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("Size to scale ratio = " + sizeToScaleRatio);
		setSize (startSize);
		if (!(spike = Resources.Load (spikeName, typeof(GameObject)))) {
			Debug.LogError("Couldn't load a spike object");
		}
		this.spikeCollider = ((GameObject) spike).gameObject.GetComponent<PolygonCollider2D> ();
		spikeSize = new Vector2(spikeCollider.bounds.size.x, spikeCollider.bounds.size.y);
		firingSpike = false;
		//velocity setup
		velocity = new Vector2 (horSpeed, 0);
	}

	// Update is called once per frame
	void Update () {
		if ( Input.GetButtonDown("FireBullet") ){
			Debug.Log ("FireBullet button pressed, size = " + size + " cost = " + spikeSizeCost);
			if ( size > spikeSizeCost && firingSpike == false){
				firingSpike = true; // set that we're firing a spike
			} 
		}

//		if ( Input.GetKeyDown(KeyCode.Space) ){
//			decSize(50.0f);
//		}

	
	}

	void fireSpike(){
//		Debug.Log ("Firing a spike");
		Vector3 spikePos = new Vector3(this.transform.position.x+this.collSize.x/2+this.spikeSize.x/2+0.5f, 
		                               this.transform.position.y-this.collSize.y/2+this.spikeSize.y/2, 
		                               0);
//		Debug.Log ("This.pos x = " + transform.position.x + " pos y = " + transform.position.y); 
//		Debug.Log("SpikePos x: " + spikePos.x + " SpikePos y : " + spikePos.y);
		Debug.Log ("Creating a spike at " + spikePos);
		Instantiate(spike, spikePos, Quaternion.identity);
	}

	void FixedUpdate () {

		this.rigidbody.velocity = velocity;

		if (firingSpike) {
			firingSpike = false;
			decSize(spikeSizeCost);
			fireSpike();
//			Time.timeScale = 0.001f;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy")
			coll.gameObject.SendMessage("ApplyDamage", 10);
		
	}


}
