using UnityEngine;
using System.Collections;
using UnityEngine.Sprites;

public class HeroController : MonoBehaviour {
	//Publics:
	public bool easyMode = false;
	public enum attackType { Null, Spike, Slam, Charge };
	public enum superAttackType { Null, Slam, Charge };
	public enum letterType { Null, A, D};
	public CameraProperties camProps;
	public LevelController level;
	//Editable in the unityEditor:
	public float horizontalSpeed = 4f;
	//Size stuff
	public float startSize = 10f;
	public float maxSize = 200f;
	public float defaultArmorPenetrationRate = 1f;
	//Time for the color change
	public float colorChangeDelay = 0.5f;
	//Slam stuff
	public float slamHeightMultiplier = 3f;
	public float slamRangeMultiplier = 4f;
	public float slamUpDur = 2f;
	public float slamDownDur = 0.5f;
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
	public string chargeFillFullName = "chargeFillFull";
	public string chargeMarkName = "chargeMark";
	public string slamFillName = "slamFill";
	public string slamFillFullName = "slamFillFull";
	public string letterAName = "heroA";
	public string letterDName = "heroD";
	//Privates
	//Player stuff start
	Rigidbody2D rigidbody;
	BoxCollider2D collider;
	Vector2 velocity;
	attackType atkType = attackType.Spike;
	letterType lType = letterType.D;
//	bool isInSuperMode = false;
	float armorPenetrationRate;
	float defaultGravity;
	//Vulnerability
//	bool isVulnerable = true;
	//Player stuff end

	//Filling stuff start
	Transform letterD;
	SpriteRenderer letterDSprite;
	Transform letterA;
	SpriteRenderer letterASprite;
	//Time count for the color change
	float colorTimeElapsed = 0f;
	//Filling stuff end

	//Slam stuff start
	Transform slamFill;
	Transform slamFillFull;
	float attackTimeElapsed = 0f; //Slam reset time
	bool inSlam = false;
	GameObject tempSlamMark1, tempSlamMark2;
	Vector3 slamMarkPos1, slamMarkPos2;
	Vector3 slamVelocity;
	Vector3 slamVelocityUp;
	Vector3 slamVelocityDown;
	//Slam stuff end

	//Charge stuff start
	Transform chargeFill;
	Transform chargeFillFull;
	float chargeDistance;
	bool inCharge = false;
	Vector3 chargeVelocity;	
	Object chargeMark;
	GameObject tempChargeMark;
	Vector3 chargeMarkPos;

	//Charge stuff end

	//Spike stuff start
	SpriteRenderer spikeRenderer;
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
//		if (isVulnerable == false) {
//			Debug.Log("Hero's invulnerable, returning");
//			return;
//		}
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
	
//	void setIsInSuperModeFalse(){
//		isInSuperMode = false;
//	}
//
//	void setIsInSuperModeTrue(){
//		isInSuperMode = true;
//	}

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
			Debug.LogError("Couldn't find \"slamFillFull\" in the Hero transform");
		}
	}

	
	void turnOnSlamFillFull(){
		if (slamFillFull) {
			slamFillFull.gameObject.SetActive (true);
		} else {
			Debug.LogError("Couldn't find \"slamFillFull\" in the Hero transform");
		}
	}
	
	void turnOffSlamFillFull (){
		if (slamFillFull) {
			slamFillFull.gameObject.SetActive (false);
		} else {
			Debug.LogError("Couldn't find \"slamFillFull\" in the Hero transform");
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

	void turnOnChargeFillFull(){
		if (chargeFillFull) {
			chargeFillFull.gameObject.SetActive (true);
		} else {
			Debug.LogError("Couldn't find \"ChargeFill\" in the Hero transform");
		}
	}
	
	void turnOffChargeFillFull (){
		if (chargeFillFull) {
			chargeFillFull.gameObject.SetActive (false);
		} else {
			Debug.LogError("Couldn't find \"ChargeFillFull\" in the Hero transform");
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
//		turnOnSlamFill ();
		armorPenetrationRate = 0f;
	}

	void makeVulnerable(){
//		turnOffSlamFill ();
		armorPenetrationRate = defaultArmorPenetrationRate;
	}

//------------------USEFUL-STUFF-END----------------------------------------

	
	
//------------------LETTER-AND ATTACK-START----------------------------------------
	attackType superAttackToPlain (superAttackType type){
		switch (type) {
		case superAttackType.Slam : 
			return attackType.Slam;
//			break;
		case superAttackType.Charge : 
			return attackType.Charge;
//			break;
		default: 
			Debug.LogError("Couldn't determine SuperAttackType");
			return attackType.Null;
//			break;
		}
	}
	
	void initAttackType( attackType newAtkType ){
		if (newAtkType == attackType.Null) {
			Debug.LogError ("[INIT] Setting attack type to null. stopping the function ");
			return;
		}	
		/*if (newAtkType == attackType.Spike) {
			setIsInSuperModeFalse();
		}*/
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
		/*if (newAtkType == attackType.Spike) {
			setIsInSuperModeFalse();
		}*/
		changeLetterType (newAtkType);
		this.atkType = newAtkType;
	}
	
	
	void activateLetterA(){
		if (letterA.gameObject.activeSelf == true) {
			Debug.Log("Letter A is already activated");
			return;
		}
		if (lType == letterType.D) {
			disableGameObject(letterD.gameObject);
		}
		lType = letterType.A;
		activateGameObject(letterA.gameObject);
	}
	
	void activateLetterD(){
		if (letterD.gameObject.activeSelf == true) {
			Debug.Log("Letter D is already activated");
			return;
		}
		if (lType == letterType.A) {
			disableGameObject(letterA.gameObject);
		}
		lType = letterType.D;
		activateGameObject(letterD.gameObject);
	}

	void deactivateAllLetters(){ //this should only hide them, not change type
		if (lType == letterType.A) {
			disableGameObject(letterA.gameObject);
		} else if (lType == letterType.D) {
			disableGameObject(letterD.gameObject);
		} else {
			Debug.LogError("Unknown Letter Type in deactivateAllLetters in HeroController");
		}
	}
	
	void activateCurrentLetter(){
		if (lType == letterType.A) {
			activateLetterA ();
		} else if (lType == letterType.D) {
			activateLetterD();
		} else {
			Debug.LogError("Unknown Letter Type in activateCurrentLevver in HeroController");
		}
	}
		
	void changeLetterType ( attackType newAtkType ){
		if (newAtkType == attackType.Slam) {
			activateLetterA ();
			turnOffChargeFill();
			turnOnSlamFill();
		} else if (newAtkType == attackType.Spike) {
			turnOffChargeFill();
			turnOffSlamFill();
			activateLetterD ();
		} else if (newAtkType == attackType.Charge) {
			activateLetterD();
			turnOnChargeFill();
			turnOffSlamFill(); // this is useless but if something unplanned happens, it'll be here
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
	
//------------------LETTER-AND ATTACK-END----------------------------------------

//------------------PICKUP-STUFF-START----------------------------------------

	void processPickup ( PickupController.pickupMessage msg){
		Debug.Log ("Hero picked up a pickup. size = " + msg.size + " type = " + msg.type);
		incSize (msg.size);
		if (msg.type == PickupController.pickupType.Transformation &&
		    !inSlam && !inCharge && atkType == attackType.Spike) {
			Debug.Log("Processing pickup");
			changeAttackType(superAttackToPlain(PickupController.GetRandomEnum<superAttackType>()));
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
			Debug.LogError("Couldn't find \"slamFillName\" in the Hero transform");
		}
		
		if ((this.slamFillFull = this.transform.Find (slamFillFullName)) == null) {
			Debug.LogError("Couldn't find \"slamFillFullName\" in the Hero transform");
		}

		if ((this.chargeFill = this.transform.Find (chargeFillName)) == null) {
			Debug.LogError("Couldn't find \"chargeFillName\" in the Hero transform");
		}

		if ((this.chargeFillFull = this.transform.Find (chargeFillFullName)) == null) {
			Debug.LogError("Couldn't find \"chargeFillFullName\" in the Hero transform");
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
		
		if ((this.spikeRenderer = ((GameObject)spike).gameObject.GetComponent<SpriteRenderer> ()) == null) {
			Debug.LogError("Couldn't load spikeCollider from Resources folder");	
		}
		//Loading chargeMark
		if ((chargeMark = Resources.Load (chargeMarkName, typeof(GameObject))) == null ) {
			Debug.LogError("Couldn't load a chargeMark object from Resources folder");
		}

	}
	
	// Use this for initialization
	void Start () {
		//setting up default gravity
		this.defaultGravity = this.rigidbody.gravityScale;
		//Setting up slamVelocity
		slamVelocity = new Vector3 ();
		slamVelocityUp = new Vector3 ();
		slamVelocityDown = new Vector3 ();
		//Setting up slamMarkPos
		slamMarkPos1 = new Vector3 ();
		slamMarkPos2 = new Vector3 ();
		//Setting up armorPenetrationRate
		armorPenetrationRate = defaultArmorPenetrationRate;
		//Charge setup
		chargeVelocity = new Vector3 ();
		//CameraProperties setup
		camProps = new CameraProperties();
		//letter setup
		initAttackType (attackType.Spike);//(attackType.Spike);
		//Setting up size stuff
		sizeToScaleRatio = this.transform.localScale.x / startSize;
		collSize = new Vector2 (this.collider.bounds.size.x, this.collider.bounds.size.y);
		updateCollSize ();
		Debug.Log ("Size to scale ratio = " + sizeToScaleRatio);
		setSize (startSize);
		//spike size setup
		spikeSize = new Vector2(spikeRenderer.bounds.size.x, spikeRenderer.bounds.size.y);
//		Debug.Log (spikeSize);
		//velocity setup
		velocity = new Vector2 (horizontalSpeed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//Check if it's inside the cam boundaries
		
		if (this.transform.position.x < level.minLevelCoords.x ||
		    this.transform.position.y < level.minLevelCoords.y ||
		    this.transform.position.x > level.maxLevelCoords.x ||
		    this.transform.position.y > level.maxLevelCoords.y  ) {
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
		if (!inSlam && !inCharge) {
			if (Input.GetButtonDown ("FireD")) {
				Debug.Log ("FireD button pressed, size = " + size + "atkType = " + atkType);
				if (atkType == attackType.Spike) {
					if (size > spikeSizeCost) {
//						firingSpike = true; // set that we're firing a spike
						decSize (spikeSizeCost); 
						fireSpike();
					} 
				} else if (atkType == attackType.Charge && size > chargeSizeCost) {
					startCharge ();
//					setIsInSuperModeTrue();
				} 
			} else if (Input.GetButtonDown ("FireA")) {
				Debug.Log ("FireA button pressed, size = " + size + "atkType = " + atkType);
				if (atkType == attackType.Slam && size > slamSizeCost) {
					startSlam ();
//					setIsInSuperModeTrue();
				}
			}
			//Cooldowns
			if (atkType == attackType.Slam) {
				if (attackTimeElapsed >= slamAvailableTime) {
					attackTimeElapsed = 0f;
					changeAttackType (attackType.Spike);
//					setIsInSuperModeFalse();
				}
			} else if (atkType == attackType.Charge) {
				if (attackTimeElapsed >= chargeAvailableTime) {
					attackTimeElapsed = 0f;
					changeAttackType (attackType.Spike);
//					setIsInSuperModeFalse();
				}
			}
		}

	}


	void FixedUpdate () {
		this.rigidbody.velocity = velocity;
		if (inCharge) {
			this.rigidbody.velocity = chargeVelocity;			
		} else if (inSlam) {
			this.rigidbody.velocity = slamVelocity;
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
		Vector3 spikePos = new Vector3(this.transform.position.x+this.collSize.x/2+this.spikeSize.x/2, 
		                               this.transform.position.y-this.collSize.y/2+this.spikeSize.y/2, 
		                               0);
		fireSpikeAtPos (spikePos);
	}
//------------------SPIKE-STUFF-END----------------------------------------

//------------------SLAM-STUFF-START----------------------------------------

	void setFirstSlamMark(){
		slamMarkPos1.Set (this.transform.position.x + slamRangeMultiplier*collSize.x,
		                 this.transform.position.y + slamHeightMultiplier*collSize.y,
		                 0);
//		Debug.LogError ("Creating slamMark1 at  " + slamMarkPos1);
		tempSlamMark1 = Instantiate (chargeMark, slamMarkPos1, Quaternion.identity) as GameObject;
	}
	
	void setSecondSlamMark(){
		slamMarkPos2.Set (this.transform.position.x + slamRangeMultiplier*collSize.x,
		                 this.transform.position.y,
		                 0);
//		Debug.LogError ("Creating slamMark2 at  " + slamMarkPos2);
		tempSlamMark2 = Instantiate (chargeMark, slamMarkPos2, Quaternion.identity) as GameObject;
	}

	void killFirstTempSlamMark (){
		if (tempSlamMark1.gameObject != null) {
			Debug.Log("Destroying tempChargeMark1");
			Destroy (tempSlamMark1.gameObject);
		}
	}

	void killSecondTempSlamMark (){
		if (tempSlamMark2.gameObject != null) {
			Debug.Log("Destroying tempChargeMark2");
			Destroy (tempSlamMark2.gameObject);
		}
	}


	void setSlamVelocity(float len, float height, float timeUp, float timeDown ){
		slamVelocityUp.Set(len/timeUp, height/timeUp, 0f);
		slamVelocityDown.Set(0f, -height/timeDown, 0f);
//		Debug.LogError ("slamVelocityUp = " + slamVelocityUp);
	}

	void startSlamVisuals(){
		enableSlowMo (slamSlowMoScale);
		setFirstSlamMark ();
		setSecondSlamMark ();
		turnOffSlamFill ();
		turnOnSlamFillFull ();
		deactivateAllLetters ();
	}

	void endSlamVisuals(){
		killSecondTempSlamMark ();
		turnOffSlamFillFull ();
		activateCurrentLetter ();

	}

	void endSlam(){
		if (transform.position.y > slamMarkPos2.y) {
			if (transform.position.y <= slamMarkPos2.y + chargeMarkLen / 2 + collSize.y / 2) {
				killSecondTempSlamMark();
			}
			return;
		}
		
		inSlam = false;
		endSlamVisuals ();
		makeVulnerable ();
		killFirstTempSlamMark ();
		killSecondTempSlamMark ();
//		this.rigidbody.gravityScale = defaultGravity; returning it in the midSlam
		changeAttackType (attackType.Spike);
		CancelInvoke ();
	}

	void midSlam(){
		if (transform.position.x < slamMarkPos1.x) {
			if (transform.position.x >= slamMarkPos1.x - chargeMarkLen / 2 - collSize.x / 2) {
//				Debug.LogError(transform.position.x);
//				Debug.LogError("chargeMarkLen = " + chargeMarkLen + " slamMarkPos1.x = " + slamMarkPos1.x + " Collsize.x = " + collSize.x);
				killFirstTempSlamMark();
			}
			return;
		}
		killFirstTempSlamMark ();
		this.rigidbody.gravityScale = defaultGravity;
		slamVelocity = slamVelocityDown;
		disableSlowMo ();
		InvokeRepeating ("endSlam", 0.01f, 0.05f);
	}

	void startSlam(){
		Debug.Log ("Slamming");
		inSlam = true;
		this.rigidbody.gravityScale = 0f;
		startSlamVisuals ();
		decSize(slamSizeCost);
		makeInvulnerable ();
//		Debug.LogError ("SlamMarkPos1 = " + slamMarkPos1 + " this = " + this.transform.position);
		setSlamVelocity (slamMarkPos1.x - this.transform.position.x, 
		                 slamMarkPos1.y - this.transform.position.y, 
		                 slamUpDur, 
		                 slamDownDur);
		slamVelocity = slamVelocityUp;
		InvokeRepeating ("midSlam", 0.01f, 0.05f);
	}

	void slam(){
		startSlam ();
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
		deactivateAllLetters ();
		turnOffChargeFill ();
		turnOnChargeFillFull ();
		//Setting up a tempChargeMark
		if (chargeMark != null) {
			chargeMarkPos.Set (this.transform.position.x + chargeDistance,
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
		turnOffChargeFillFull ();
		activateCurrentLetter ();
		killTempChargeMark ();
	}

	void startCharge(){			
		inCharge = true;
		updateChargeDistance();
		startChargeVisuals ();
		armorPenetrationRate = chargeArmorPenetrationRate;
		Debug.Log ("Starting a charge, duration = " + chargeDur + "Distance = " + chargeDistance);
		setChargeVelocity (chargeDur, chargeDistance);
		InvokeRepeating ("endCharge", chargeDur, 0.01f);
	}

	void endCharge(){
		if (transform.position.x < chargeMarkPos.x) {
			if (transform.position.x >= chargeMarkPos.x - chargeMarkLen / 2 - collSize.x / 2) {
				killTempChargeMark ();
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
