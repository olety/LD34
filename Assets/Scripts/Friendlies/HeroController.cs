using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;

public class HeroController : MonoBehaviour {
	public CameraProperties camProps;
	public LevelController level;
	public float horSpeed;
	public string spikeName = "Spike";
	public float slamVelocity = 2f;
	public enum attackType { Null, Spike, Slam };//, Teleport};
	public enum superAttackType { Null, Slam };
	public enum letterType { Null, A, D};
	//Time for the color change
	public float colorChangeDelay = 0.5f;
	//slam time cooldown
	public float slamAvailableTime = 2f;

	Rigidbody2D rigidbody;
	BoxCollider2D collider;
	PolygonCollider2D spikeCollider;
	Vector2 spikeSize;
	Object spike;
	Vector2 velocity;
	attackType atkType = attackType.Spike;
	letterType lType = letterType.D;
	//Filling stuff start
	Transform fill;
	Transform letterD;
	SpriteRenderer letterDSprite;
	Transform letterA;
	SpriteRenderer letterASprite;
	//Filling stuff end
	//Time count for the color change
	float colorTimeElapsed = 0f;
	//Slam reset time
	float slamTimeElapsed = 0f;
	//Invulnerability
	bool isVulnerable = true;

	//Booleans for attacks
	bool firingSpike = false;
	bool slamming = false, inSlam = false;
	
	
	
	bool teleporting = false;

//------------------SIZE-STUFF-START----------------------------------------
	public float startSize = 10f;
	public float maxSize = 200f;
	public float spikeSizeCost = 10f;
	public float slamSizeCost = 30f;
	public float tpSizeCost = 30f;
	
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
		if (isVulnerable == false) {
			Debug.Log("Hero's invulnerable, returning");
			return;
		}
		this.setSize (this.size - amount);
	}
	
	public void setSize ( float amount ){
		Debug.Log ("Hero size set requested, amount = " + amount);
		if (amount <= 0f) {
			level.Lose();
			return;
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

//------------------SIZE-STUFF-END----------------------------------------


//------------------USEFUL-STUFF-START----------------------------------------

	void activateGameObject(GameObject obj){
		obj.SetActive (true);
	}
	
	void disableGameObject(GameObject obj){
		obj.SetActive (false);
	}

	void turnOnFill(){
		if (fill) {
			fill.gameObject.SetActive (true);
		} else {
			Debug.LogError("Couldn't find \"Fill\" in the Hero transform");
		}
	}
	
	void turnOffFill (){
		if (fill) {
			fill.gameObject.SetActive (false);
		} else {
			Debug.LogError("Couldn't find \"Fill\" in the Hero transform");
		}
	}
	
	void enableSlowMo ( float rate, float dur, bool fill ){
		level.slowMo(rate);
		Invoke("unSlowMo", dur);
	}
	
	void unSlowMo (){
		level.unSlowMo();
	}

	void makeInvulnerable(){
		turnOnFill ();
		isVulnerable = false;
	}

	void makeVulnerable(){
		turnOffFill ();
		isVulnerable = true;
	}

//------------------USEFUL-STUFF-END----------------------------------------


//------------------PICKUP-STUFF-START----------------------------------------

	void processPickup ( PickupController.pickupMessage msg){
		Debug.Log ("Hero picked up a pickup. size = " + msg.size + " type = " + msg.type);
		incSize (msg.size);
		if (msg.type == PickupController.pickupType.Transformation) {
			changeAttackType(superAttackToPlain(PickupController.GetRandomEnum<superAttackType>()));
		}
	}

	attackType superAttackToPlain (superAttackType type){
		switch (type) {
			case superAttackType.Slam : 
				return attackType.Slam;
				break;
			default: 
				Debug.LogError("Couldn't determine SuperAttackType");
				return attackType.Null;
				break;
		}
	}

	void initAttackType( attackType newAtkType ){
		if (newAtkType == attackType.Null) {
			Debug.LogError ("[INIT] Setting attack type to null. stopping the function ");
			return;
		}
		changeLetterType (newAtkType);
		this.atkType = newAtkType;
	}

	void changeAttackType( attackType newAtkType ){
		if (newAtkType == attackType.Null) {
			Debug.LogError ("[RUNTIME] Setting attack type to null. stopping the function");
			return;
		}
		if (newAtkType == this.atkType) {
			return;
		}
		changeLetterType (newAtkType);
		this.atkType = newAtkType;
	}


	void activateLetterA(){
		lType = letterType.A;
		activateGameObject(letterA.gameObject);
		disableGameObject(letterD.gameObject);
	}

	void activateLetterD(){
		lType = letterType.D;
		activateGameObject(letterD.gameObject);
		disableGameObject(letterA.gameObject);
	}

	void changeLetterType ( attackType newAtkType ){
		if (newAtkType == attackType.Slam) {
			activateLetterA();
		} else if (newAtkType == attackType.Spike) {
			activateLetterD();
		} else {//if ( newAtkType == attackType.Teleport){
			Debug.Log("Teleport attack type");
		}
	}

	void LetterColorChange(){
		Color randColor = new Color ((Random.Range (0f, 250f))/255, //unity takes RGB colors in floats
		                   (Random.Range (0f, 250f))/255, // in the form (0..1,0..1,0..1)
		                   (Random.Range (0f, 250f))/255,  // so we take a random from 0.250 (255 would be all white) and normalize it
		                   255); // opacity. for this to be seen, it has to be 255
//		Debug.Log("Changing letter" + lType + " color to " + randColor);
		if (lType == letterType.A) {
			letterASprite.color = randColor;
		} else if (lType == letterType.D){
			letterDSprite.color = randColor;
		}
	}
	
//------------------PICKUP-STUFF-END----------------------------------------


//------------------ENEMY-STUFF-START----------------------------------------

	void processEnemyColl (EnemyController.enemyMessage msg){
		Debug.Log ("Hero collided with an enemy . enemysize = " + msg.size + " type = " + msg.type);
		if (msg.type == EnemyController.enemyType.Enemy1) {
			decSize (msg.size);
		}
	}

//------------------ENEMY-STUFF-END----------------------------------------


//------------------STANDARD-STUFF-START----------------------------------------
	void Awake () { 

		// Loading components
		if ( (this.rigidbody = this.gameObject.GetComponent<Rigidbody2D> ()) == null){
			Debug.LogError("Couldnt load rigidbody2D in HeroController");
		}

		if ((this.collider = this.gameObject.GetComponent<BoxCollider2D> ()) == null) {
			Debug.LogError("Couldnt load collider in HeroController");
		}

		//Loading children
		if ((this.fill = this.transform.Find ("Fill")) == null) {
			Debug.LogError("Couldn't find \"Fill\" in the Hero transform");
		}

		//Loading letter A
		if ((this.letterA = this.transform.Find ("heroA")) == null) {
			Debug.LogError("Couldn't find \"Letter A\" in the Hero transform");
		}
		if ( (this.letterASprite = letterA.GetComponent<SpriteRenderer>()) == null ){
			Debug.LogError("Couldn't find SpriteRenderer in the letterA child of Hero transform");
		}

		//Loading letter D
		if ((this.letterD = this.transform.Find ("heroD")) == null) {
			Debug.LogError("Couldn't find \"Letter D\" in the Hero transform");
		}	

		if ( (this.letterDSprite = letterD.GetComponent<SpriteRenderer>()) == null ){
			Debug.LogError("Couldn't find SpriteRenderer in the letterD child of Hero transform");
		}

		if ((spike = Resources.Load (spikeName, typeof(GameObject))) == null ) {
			Debug.LogError("Couldn't load a spike object");
		}

		if ((this.spikeCollider = ((GameObject)spike).gameObject.GetComponent<PolygonCollider2D> ()) == null) {
			Debug.LogError("Couldn't load spikeCollider");	
		}
	}
	
	// Use this for initialization
	void Start () {
		//letter setup
		initAttackType (attackType.Spike);
		//Setting up size stuff
		sizeToScaleRatio = this.transform.localScale.x / startSize;
		collSize = new Vector2 (this.collider.bounds.size.x, this.collider.bounds.size.y);
		updateCollSize ();
		Debug.Log ("Size to scale ratio = " + sizeToScaleRatio);
		setSize (startSize);
		//spike size setup
		spikeSize = new Vector2(spikeCollider.bounds.size.x, spikeCollider.bounds.size.y);
		//velocity setup
		velocity = new Vector2 (horSpeed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//Letter color stuff
		colorTimeElapsed += Time.deltaTime;
		if (atkType == attackType.Slam) {
			slamTimeElapsed += Time.deltaTime;
			if ( slamTimeElapsed >= slamAvailableTime ){
				slamTimeElapsed = 0f;
				changeAttackType(attackType.Spike);
			}
		}

		if (colorTimeElapsed >= colorChangeDelay) {
			LetterColorChange ();
			colorTimeElapsed = 0;
		}

		//Attack processing
		if ( Input.GetButtonDown("FireSpike") && atkType == attackType.Spike ){
			Debug.Log ("FireSpike button pressed, size = " + size + " cost = " + spikeSizeCost);
			if ( size > spikeSizeCost && firingSpike == false){
				firingSpike = true; // set that we're firing a spike
			} 
		} else if ( Input.GetButtonDown("Slam") && atkType == attackType.Slam){
			Debug.Log ("Slam button pressed, size = " + size + " cost = " + slamSizeCost);
			if ( size > slamSizeCost && slamming == false){
				setupSlam();
			}
		}
	}

	void FixedUpdate () {
		
		this.rigidbody.velocity = velocity;
		
		if (firingSpike) {
			firingSpike = false;
			decSize (spikeSizeCost);
			fireSpike ();
			//			Time.timeScale = 0.001f;
		} else if (slamming && !inSlam) {
			startSlam();
			slam ();
		}
	}
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Enemy") {
//			coll.gameObject.SendMessage("ApplyDamage", 10);
		}
	}

//------------------STANDARD-STUFF-END----------------------------------------


//------------------SPIKE-STUFF-START----------------------------------------

	void fireSpikeAtPos( Vector3 pos ){
		Debug.Log ("Creating a spike at " + pos);
		Instantiate(spike, pos, Quaternion.identity);
	}
	
	void fireSpike(){
		Debug.Log ("Firing a spike");
		Vector3 spikePos = new Vector3(this.transform.position.x+this.collSize.x/2+this.spikeSize.x/2+0.5f, 
		                               this.transform.position.y-this.collSize.y/2+this.spikeSize.y/2+0.1f, 
		                               0);
		fireSpikeAtPos (spikePos);
	}
//------------------SPIKE-STUFF-END----------------------------------------

//------------------SLAM-STUFF-START----------------------------------------

	void setupSlam(){
		slamming = true;
	}

	void startSlam(){
		inSlam = true;
		makeInvulnerable ();
		slam ();
	}

	void endSlam(){
		nullVelocityY ();
		inSlam = false;
		slamming = false;
		makeVulnerable ();
	}

	void incVelocityY(float inc){
		this.velocity.y += inc;
	}

	void decVelocityY(float dec){
		this.velocity.y -= dec;
	}

	void nullVelocityY(){
		this.velocity.y = 0;
	}
	
	void slamDecVelocity(){
		decVelocityY (2.5f * slamVelocity);
		Invoke ("endSlam", 1f);
	}

	void slam(){
		decSize(slamSizeCost);
		Debug.Log ("Slamming");
		incVelocityY (slamVelocity);
		Invoke ("slamDecVelocity", 1f);
	}
	
//------------------SLAM-STUFF-END----------------------------------------




	

	
}
