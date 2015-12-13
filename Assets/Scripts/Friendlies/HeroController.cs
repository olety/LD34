using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;

public class HeroController : MonoBehaviour {
	//Publics:
	public enum attackType { Null, Spike, Slam, Charge };
	public enum superAttackType { Null, Slam, Charge };
	public enum letterType { Null, A, D};
	public CameraProperties camProps;
	public LevelController level;
	//Editable in the unityEditor:
	public float horizontalSpeed = 4f;
	public float slamUpVelocity = 2f;
	public float slamDownVelocity = 100f;
	//Size stuff
	public float startSize = 10f;
	public float maxSize = 200f;
	public float defaultArmorPenetrationRate = 1f;
	//Time for the color change
	public float colorChangeDelay = 0.5f;
	//Slam stuff
	public float slamAvailableTime = 2f;
	public float slamSlowMoScale = 0.2f;
	public float slamSizeCost = 30f;
	//Charge stuff
	public float chargeArmorPenetrationRate = 0.75f; //by enemies, not them
	public float chargeSlowMoScale = 0.5f;
	public float chargeSizeCost = 20f;
	public float chargeDistanceMultiplier = 5f;
	public float chargeDur = 2f;
	public float chargeAvailableTime = 2.5f;
	public float chargeMarkLen = 1f;
	//Spike stuff
	public float spikeSizeCost = 10f;
	//Names
	public string spikeName = "Spike";
	public string chargeFillName = "chargeFill";
	public string chargeMarkName = "chargeMark";
	public string slamFillName = "slamFill";
	public string letterAName = "heroA";
	public string letterDName = "heroD";
	//Privates
	//Player stuff start
	Rigidbody2D rigidbody;
	BoxCollider2D collider;
	Vector2 velocity;
	attackType atkType = attackType.Spike;
	letterType lType = letterType.D;

	float armorPenetrationRate;
	//Vulnerability
	bool isVulnerable = true;
	//Player stuff end

	//Filling stuff start
	Transform slamFill;
	Transform chargeFill;
	Transform letterD;
	SpriteRenderer letterDSprite;
	Transform letterA;
	SpriteRenderer letterASprite;
	//Time count for the color change
	float colorTimeElapsed = 0f;
	//Filling stuff end

	//Slam stuff start
	float attackTimeElapsed = 0f; //Slam reset time
	bool inSlam = false;
	float slamDur;
	//Slam stuff end

	//Charge stuff start
	float chargeDistance;
	bool inCharge = false;
	Vector3 chargeVelocity;	
	Object chargeMark;
	GameObject tempChargeMark;
	Vector3 chargeMarkPos;
	//Charge stuff end

	//Spike stuff start
	PolygonCollider2D spikeCollider;
	Vector2 spikeSize;
	Object spike;
	bool firingSpike = false;
	//Spike stuff end

//------------------SIZE-STUFF-START----------------------------------------

	
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
//		Debug.Log ("Hero size inc requested, amount = " + amount);
		this.setSize (this.size + amount);
	}
	
	public void decSize ( float amount ){
//		Debug.Log ("Hero size dec requested, amount = " + amount);
		if (isVulnerable == false) {
//			Debug.Log("Hero's invulnerable, returning");
			return;
		}
		this.setSize (this.size - amount);
	}
	
	public void setSize ( float amount ){
//		Debug.Log ("Hero size set requested, amount = " + amount);
		if (amount <= 0f) {
			level.Lose();
			return;
		}
		this.size = Mathf.Clamp (amount, 0, this.maxSize);
		this.updateSize ();
	}
	
	void updateSize(){
		Vector3 newScale = new Vector3 (size * sizeToScaleRatio, size * sizeToScaleRatio, 0f);
//		Debug.Log ("Hero size update requested. NewScale = " + newScale + " x :" + newScale.x);
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
//		Debug.Log ("Hero collider update requested. x = " + collSize.x + " ,y = " + collSize.y); 
	}

//------------------SIZE-STUFF-END----------------------------------------


//------------------USEFUL-STUFF-START----------------------------------------

	void activateGameObject(GameObject obj){
		obj.SetActive (true);
	}
	
	void disableGameObject(GameObject obj){
		obj.SetActive (false);
	}

	void turnOnSlamFill(){
		if (slamFill) {
			slamFill.gameObject.SetActive (true);
		} else {
			Debug.LogError("Couldn't find \"SlamFill\" in the Hero transform");
		}
	}
	
	void turnOffSlamFill (){
		if (slamFill) {
			slamFill.gameObject.SetActive (false);
		} else {
			Debug.LogError("Couldn't find \"SlamFill\" in the Hero transform");
		}
	}

	void turnOnChargeFill(){
		if (chargeFill) {
			chargeFill.gameObject.SetActive (true);
		} else {
			Debug.LogError("Couldn't find \"ChargeFill\" in the Hero transform");
		}
	}
	
	void turnOffChargeFill (){
		if (chargeFill) {
			chargeFill.gameObject.SetActive (false);
		} else {
			Debug.LogError("Couldn't find \"ChargeFill\" in the Hero transform");
		}
	}
	
	void enableSlowMo ( float rate, float dur ){
		level.slowMo(rate);
		Invoke("unSlowMo", dur);
	}

	void enableSlowMo ( float rate ){
		level.slowMo(rate);
	}
	
	void disableSlowMo (){
		level.unSlowMo();
	}

	void makeInvulnerable(){
		turnOnSlamFill ();
		isVulnerable = false;
	}

	void makeVulnerable(){
		turnOffSlamFill ();
		isVulnerable = true;
	}

//------------------USEFUL-STUFF-END----------------------------------------


//------------------PICKUP-STUFF-START----------------------------------------

	void processPickup ( PickupController.pickupMessage msg){
		Debug.Log ("Hero picked up a pickup. size = " + msg.size + " type = " + msg.type);
		incSize (msg.size);
		if (msg.type == PickupController.pickupType.Transformation &&
		    inCharge == false && firingSpike  == false && inSlam == false &&
		    atkType == attackType.Spike) {
			Debug.Log("Processing pickup");
			changeAttackType(superAttackToPlain(PickupController.GetRandomEnum<superAttackType>()));
		}
	}

	attackType superAttackToPlain (superAttackType type){
		switch (type) {
			case superAttackType.Slam : 
				return attackType.Slam;
				break;
			case superAttackType.Charge : 
				return attackType.Charge;
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
			Debug.Log("Setting attackType to the same value. Quitting the function");
			return;
		}
		changeLetterType (newAtkType);
		this.atkType = newAtkType;
	}


	void activateLetterA(){
		if (letterA.gameObject.activeSelf == true) {
			Debug.Log("Letter A is already activated");
			return;
		}
		lType = letterType.A;
		activateGameObject(letterA.gameObject);
		disableGameObject(letterD.gameObject);
	}

	void activateLetterD(){
		if (letterD.gameObject.activeSelf == true) {
			Debug.Log("Letter D is already activated");
			return;
		}
		lType = letterType.D;
		activateGameObject(letterD.gameObject);
		disableGameObject(letterA.gameObject);
	}

	void changeLetterType ( attackType newAtkType ){
		if (newAtkType == attackType.Slam) {
			activateLetterA ();
			turnOffChargeFill();
		} else if (newAtkType == attackType.Spike) {
			turnOffChargeFill();
			activateLetterD ();
		} else if (newAtkType == attackType.Charge) {
			activateLetterD();
			turnOnChargeFill();
		} else {
			Debug.LogError("Invalid attack type in changeLetterType in " + this.name);
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
			decSize (msg.size * Mathf.Clamp01(armorPenetrationRate));
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
		if ((this.slamFill = this.transform.Find (slamFillName)) == null) {
			Debug.LogError("Couldn't find \"Fill\" in the Hero transform");
		}

		//Loading children
		if ((this.chargeFill = this.transform.Find (chargeFillName)) == null) {
			Debug.LogError("Couldn't find \"Fill\" in the Hero transform");
		}

		//Loading letter A
		if ((this.letterA = this.transform.Find (letterAName)) == null) {
			Debug.LogError("Couldn't find \"Letter A\" in the Hero transform");
		}
		if ( (this.letterASprite = letterA.GetComponent<SpriteRenderer>()) == null ){
			Debug.LogError("Couldn't find SpriteRenderer in the letterA child of Hero transform");
		}

		//Loading letter D
		if ((this.letterD = this.transform.Find (letterDName)) == null) {
			Debug.LogError("Couldn't find \"Letter D\" in the Hero transform");
		}	

		if ( (this.letterDSprite = letterD.GetComponent<SpriteRenderer>()) == null ){
			Debug.LogError("Couldn't find SpriteRenderer in the letterD child of Hero transform");
		}
		//Loading spike
		if ((spike = Resources.Load (spikeName, typeof(GameObject))) == null ) {
			Debug.LogError("Couldn't load a spike object from Resources folder");
		}
		
		if ((this.spikeCollider = ((GameObject)spike).gameObject.GetComponent<PolygonCollider2D> ()) == null) {
			Debug.LogError("Couldn't load spikeCollider from Resources folder");	
		}
		//Loading chargeMark
		if ((chargeMark = Resources.Load (chargeMarkName, typeof(GameObject))) == null ) {
			Debug.LogError("Couldn't load a chargeMark object from Resources folder");
		}

	}
	
	// Use this for initialization
	void Start () {
		//Setting up armorPenetrationRate
		armorPenetrationRate = defaultArmorPenetrationRate;
		//Charge setup
		chargeVelocity = new Vector3 ();
		//CameraProperties setup
		camProps = new CameraProperties();
		//letter setup
		initAttackType (attackType.Charge);//(attackType.Spike);
		//Setting up size stuff
		sizeToScaleRatio = this.transform.localScale.x / startSize;
		collSize = new Vector2 (this.collider.bounds.size.x, this.collider.bounds.size.y);
		updateCollSize ();
		Debug.Log ("Size to scale ratio = " + sizeToScaleRatio);
		setSize (startSize);
		//spike size setup
		spikeSize = new Vector2(spikeCollider.bounds.size.x, spikeCollider.bounds.size.y);
		//velocity setup
		velocity = new Vector2 (horizontalSpeed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//Check if it's inside the cam boundaries
		
		if (this.transform.position.y < camProps.BottomLeft.y) {
			Debug.LogError ("Player fell through the level. Killing it");
			killThis ();
		}


		//Letter color stuff
		colorTimeElapsed += Time.deltaTime;	
		attackTimeElapsed += Time.deltaTime;



		if (colorTimeElapsed >= colorChangeDelay) {
			LetterColorChange ();
			colorTimeElapsed = 0;
		}

		//Attack processing
		if (inSlam == false && inCharge == false && firingSpike == false) {


			if (Input.GetButtonDown ("FireD")) {
				Debug.Log ("FireD button pressed, size = " + size + "atkType = " + atkType);
				if (atkType == attackType.Spike) {
					if (size > spikeSizeCost) {
						firingSpike = true; // set that we're firing a spike
						//-Comment-everything-below-if-placing-it-in-fixedUpdate------- 
						firingSpike = false; 
						decSize (spikeSizeCost); 
						fireSpike (); 
						//------------------------------------------------------------
					} 
				} else if (atkType == attackType.Charge && size > chargeSizeCost) {
					startCharge ();
				} 
			} else if (Input.GetButtonDown ("FireA")) {
				Debug.Log ("FireA button pressed, size = " + size + "atkType = " + atkType);
				if (atkType == attackType.Slam && size > slamSizeCost) {
					//-Comment-everything-below-if-placing-it-in-fixedUpdate------- 
					startSlam ();
					//------------------------------------------------------------
				}
			}
		}
		//Cooldowns
		if (atkType == attackType.Slam) {
			if (attackTimeElapsed >= slamAvailableTime) {
				attackTimeElapsed = 0f;
				changeAttackType (attackType.Spike);
			}
		} else if (atkType == attackType.Charge) {
			if (attackTimeElapsed >= chargeAvailableTime) {
				attackTimeElapsed = 0f;
				changeAttackType (attackType.Spike);
			}
		}

	}


	void FixedUpdate () {
		if (inCharge == false) {
			this.rigidbody.velocity = velocity;
		} else {
			this.rigidbody.velocity = chargeVelocity;			
		}
	/*	if (firingSpike) {
			firingSpike = false;
			decSize (spikeSizeCost);
			fireSpike ();
			//			Time.timeScale = 0.001f;
		} else if (slamming && !inSlam) {
			startSlam();
		} */
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
	
	
	void startSlam(){
		inSlam = true;
		makeInvulnerable ();
		slam ();
	}
	
	void endSlam(){
		nullVelocityY ();
		inSlam = false;
		disableSlowMo ();
		makeVulnerable ();
		changeAttackType (attackType.Spike);
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
		slamDur = 0.1f + (camProps.CamHeight  - this.transform.position.y - this.collSize.y*2) / slamDownVelocity;
		decVelocityY (slamDownVelocity);
		Invoke ("endSlam", slamDur);
	}
	
	void slam(){
		//Uncomment 2 for staying insode the screen
		slamDur = (camProps.CamHeight  - this.transform.position.y - this.collSize.y*2) / slamUpVelocity;
		Debug.Log ("Slamming");
		decSize(slamSizeCost);
		incVelocityY (slamUpVelocity);
		enableSlowMo (slamSlowMoScale);
		Invoke ("slamDecVelocity", slamDur);
	}
	
//------------------SLAM-STUFF-END----------------------------------------


//------------------CHARGE-STUFF-START----------------------------------------
	
	void setChargeVelocity ( float dur, float len ){
		chargeVelocity.x = len / dur;
		chargeVelocity.y = 0;
		chargeVelocity.z = 0;
	}

	void updateChargeDistance(){
		chargeDistance = collSize.x * chargeDistanceMultiplier;
	}

	void killTempChargeMark(){
		if (tempChargeMark.gameObject != null) {
			Debug.Log("Destroying tempChargeMark");
			Destroy (tempChargeMark.gameObject);
		}
	}
	
	void startChargeVisuals(){
		enableSlowMo (chargeSlowMoScale);		
		//Setting up a tempChargeMark
		if (chargeMark != null) {
			chargeMarkPos = new Vector3 (this.transform.position.x + chargeDistance,
			                             this.transform.position.y,
			                             0);
			Debug.Log ("Creating a tempChargeMark at " + chargeMarkPos);
			tempChargeMark = (GameObject)(Instantiate (chargeMark, chargeMarkPos, Quaternion.identity) as GameObject);
		}
		//Ended setting up a chargeMark
	}

	void endChargeVisuals (){
//		turnOffChargeFill ();
		disableSlowMo ();		
		killTempChargeMark ();
	}

	void startCharge(){			
		inCharge = true;
		updateChargeDistance();
		startChargeVisuals ();
		armorPenetrationRate = chargeArmorPenetrationRate;
		Debug.Log ("Starting a charge, duration = " + chargeDur + "Distance = " + chargeDistance);
		setChargeVelocity (chargeDur, chargeDistance);
		InvokeRepeating ("endCharge", chargeDur, 0.1f);
	}

	void endCharge(){
		if (transform.position.x < chargeMarkPos.x) {
			if ( transform.position.x >= chargeMarkPos.x-chargeMarkLen/2-collSize.x/2){
				killTempChargeMark();
			}
			return;
		}
		endChargeVisuals ();
		Debug.Log ("Ending a charge");
		armorPenetrationRate = defaultArmorPenetrationRate;
		inCharge = false;
		changeAttackType (attackType.Spike);
		CancelInvoke ();
	}

	void charge(){
		startCharge ();
	}
	
//------------------CHARGE-STUFF-END----------------------------------------

	void killThis(){

		level.Lose();

	}

}
