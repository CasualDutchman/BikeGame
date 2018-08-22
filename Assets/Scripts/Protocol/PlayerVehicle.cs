/*
using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(LodeProtocol))]
public class PlayerVehicle : MonoBehaviour
{
	// Wheel references
	[System.Serializable]
	private class Wheels
	{
		public GameObject frontObj = null;
		public GameObject backLeftObj = null;
		public GameObject backRightObj = null;
		public WheelCollider frontLeftColl = null;
		public WheelCollider frontRightColl = null;
		public WheelCollider backLeftColl = null;
		public WheelCollider backRightColl = null;

		public GameObject[] GetObjects()
		{
			return new GameObject[] { frontObj, backLeftObj, backRightObj };
		}

		public WheelCollider[] GetColliders()
		{
			return new WheelCollider[] { frontLeftColl, frontRightColl, backLeftColl, backRightColl };
		}
	}

	// Object references
	//public WorldMap map;
	//public TradeMenu trade;
	//public Tutorial tutorial;
	[SerializeField] private Animator animControls;
	[SerializeField] private Animator animRoof;
	[SerializeField] private GameObject particleEngine;
#if UNITY_EDITOR
	private new Rigidbody rigidbody;
#else
	private Rigidbody rigidbody;
#endif
	private AudioSource audioSource;

	[SerializeField] private Wheels wheels = null;
	public GameObject[] cargoObj; // Chest, Crate2, Crate1, Crate, Barrel, Crate3, Crate4, Barrel1, Barrel2, Crate5
	//[SerializeField] private VirtualGamepad pad;

	public AudioClip audioClick;
	public AudioClip audioHitRock;
	public AudioClip audioHitWood;

	private AudioSource sourceEngine;

	// Player information
	private LodeProtocol protocol = null;
	private Protocol sendPower = null;
	//public Inventory inventory = new Inventory(10);
	public int money = 150;
	public float health = 100f;
	private float shownHealth = 100f;

	// Vehicle information
	[SerializeField] private Vector3 centerOfMass = Vector3.zero;
	[SerializeField] private float motorMultiplier = 2000f;
	[SerializeField] private float reverseMultiplier = -200f;
	[SerializeField] private float brakeMultiplier = 2500f;
	[SerializeField] private float brakeFade = 3f;
	[SerializeField] private float steeringAngle = 25f;
	private float frontWheelAngle = 0;

	private float respawnProgress = 0f;
	//[HideInInspector] public Bezier.CurvePoint currentRespawn;
	//[HideInInspector] public WorldSection currentSection = null;

	[HideInInspector] public float ergoSpeed = 0;

	// Onderzoek
	//[HideInInspector] public WorldGeneration world;
	[HideInInspector] public List<GameObject> hits; 

	private void Start()
	{
		protocol = GetComponent<LodeProtocol>();
		if (protocol == null)
		{
			Debug.LogError("Lode Protocol script not found!");
			return;
		}
		
		protocol.Connect();

		protocol.ResponseReceived += ProtocolUpdate;
		protocol.AddCall(LodeProtocol.GET_SPEED_FLOAT);
		sendPower = protocol.AddCall(LodeProtocol.SET_POWER, 26);

		rigidbody = GetComponent<Rigidbody>();
		rigidbody.centerOfMass = centerOfMass;

		//currentRespawn = new Bezier.CurvePoint(transform.position, transform.rotation);
		sourceEngine = GetComponent<AudioSource>();

		//tutorial.SetTutorialPart(tutorial.pReverse);

		// Enable the Oculus
		//VRSettings.loadedDevice = VRDeviceType.Oculus;
		//VRSettings.enabled = true;
	}


	// Protocol Update
	private void ProtocolUpdate(Protocol usedProt, string data)
	{
		switch (usedProt.command)
		{
			case LodeProtocol.GET_SPEED_FLOAT:
				try
				{
					ergoSpeed = int.Parse(data) / 10f;
				}
				catch (System.Exception)
				{
					string t = "";
					foreach (char c in data.ToCharArray())
					{
						t += (int)c + " ";
					}
					Debug.LogWarning("test: " + data + "|" + data.Length + "|" + t + "|" + data.ToCharArray().Length);
					Debug.Log("protocol " + usedProt.command);
				}
				break;
		}
	}

	void Update()
	{
#if !UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
			return;
		}
#endif

		if (Input.GetKeyDown(KeyCode.R))
		{
			//InputTracking.Recenter();
			return;
		}

		//if (protocol == null || wheels == null || 
			//(map != null && map.gameObject.activeInHierarchy) ||
			//(trade != null && trade.gameObject.activeInHierarchy)) return;

		if (Input.GetButton("Button1"))
		{
			respawnProgress += Time.deltaTime;


			if (respawnProgress < 3)
			{
				//pad.SetGamepadText(1, "Herladen: " + string.Format("{0:0.0}", 3 - respawnProgress) + "s");
				return;
			}

			if (respawnProgress > 100) return;
			
			//pad.button1.objText.gameObject.SetActive(false);
			respawnProgress += 100f;
			health = 100f;

			//transform.position = currentRespawn.position;
			//transform.rotation = currentRespawn.rotation;
			rigidbody.velocity = Vector3.zero;

			//if (currentSection == null) return;

			//int st = currentSection.obstacles.Length / 10;
			//int en = currentSection.obstacles.Length / 2;
			//Vector2 pos = new Vector2(currentRespawn.position.x, currentRespawn.position.z);

			for (; st < en; st++)
			{
				//GameObject obj = currentSection.obstacles[st];
				if (obj == null) continue;

				if (Vector2.Distance(pos, new Vector2(obj.transform.position.x, obj.transform.position.z)) < 10f)
				{
					Destroy(obj);
				}
			}
			return;
		}
		else if (respawnProgress > 0f)
		{
			pad.button1.objText.gameObject.SetActive(false);
			respawnProgress = 0f;
		}

		if (Input.GetButtonDown("Button4"))
		{
			int animID = Animator.StringToHash("OpenRoof");
			animRoof.SetBool(animID, !animRoof.GetBool(animID));
			return;
		}
	}

	void FixedUpdate()
	{
		if (protocol == null || wheels == null) return;

		if (pad != null)
		{
			pad.SetGamepadText(2, "Snelheid\n" + Mathf.FloorToInt(rigidbody.velocity.magnitude * 3.6f) + " km/u");

			shownHealth = Mathf.Max(Mathf.MoveTowards(shownHealth, health, 1f), 0f);
			bool isBroken = (shownHealth <= 0);
			if (isBroken)
			{
				pad.SetGamepadText(3, "Schade\n<color=#CC0000FF>kapot</color>");
			}
			else
			{
				pad.SetGamepadText(3, "Schade\n" + (100 - Mathf.CeilToInt(shownHealth)) + "%");
			}

			if (particleEngine.activeSelf != isBroken)
			{
				particleEngine.SetActive(isBroken);
			}

			// Tutorial
			if (tutorial.current == tutorial.pNone)
			{
				if (tutorial.pReverse.shown && !tutorial.pSteering.shown && Vector3.Distance(currentRespawn.position, transform.position) > 6f)
				{
					tutorial.SetTutorialPart(tutorial.pSteering);
				}
				else if (tutorial.pSteering.shown && !tutorial.pSpeed.shown)
				{
					tutorial.SetTutorialPart(tutorial.pSpeed);
				}
				else if (tutorial.pSpeed.shown && !tutorial.pHealth.shown)
				{
					tutorial.SetTutorialPart(tutorial.pHealth);
				}
				else if (tutorial.pHealth.shown && !tutorial.pRespawn.shown)
				{
					tutorial.SetTutorialPart(tutorial.pRespawn);
				}
			}
		}

		// Play animations if the state changes
		if ((map != null && map.gameObject.activeInHierarchy) ||
			(trade != null && trade.gameObject.activeInHierarchy))
		{
			animControls.SetBool("CloseControls", true);
			animRoof.SetBool("OpenRoof", false);
			SetWheelTorques(0f, brakeMultiplier);
			rigidbody.velocity = Vector3.zero;
			return;
		}
		else if (animControls.GetBool("CloseControls"))
		{
			animControls.SetBool("CloseControls", false);
			animRoof.SetBool("OpenRoof", true);
		}



		world.averageUpdates++;
		world.averageErgoSpeed += ergoSpeed;

		// Move vehicle
		float speedInput = ergoSpeed + (Input.GetKey(KeyCode.Space) ? 100f : 0f);
		float steerInput = Input.GetAxisRaw("Horizontal") * steeringAngle;


		MoveVehicle(steerInput, speedInput);

		sourceEngine.pitch = (rigidbody.velocity.magnitude / 10);
	}

	private void MoveVehicle(float steerInput, float speedInput)
	{
		wheels.frontLeftColl.steerAngle = wheels.frontRightColl.steerAngle = steerInput; //Mathf.LerpAngle(wheels.frontLeftColl.steerAngle, steerInput, 0.025f + 1.5f * Time.fixedDeltaTime); ;

		//string text = speedInput + "|" + (Mathf.Round(wheels.frontLeftColl.steerAngle * 10) / 10);
		//text += "\n" + (Mathf.Round(wheels.frontLeftColl.rpm * 10) / 10f) + "|" + (Mathf.Round(wheels.backLeftColl.rpm * 10) / 10f) + "\n" + (Mathf.Round(rigidbody.velocity.magnitude*10f) / 10f) ;
		//if (world.adaptiveDifficulty)
		//{ text += "\n" + world.adaptiveHeight + "|" + world.adaptiveObstacle + "|" + (Mathf.Round(world.averageErgoSpeed * 10) / 10); }
		//pad.SetGamepadText(3, text);
		
		if (health <= 0 || tutorial.current != tutorial.pNone)
		{
			SetWheelTorques(0f, brakeMultiplier);
		}
		else
		{
			float speed = rigidbody.velocity.magnitude;
			float max_speed = RPMToMpS(speedInput, 1f);
			float engine = motorMultiplier;

			if (Input.GetButton("TriggerLeft") || Input.GetButton("TriggerRight"))
			{
				engine = -engine;

				if (speed < 2.5f)
				{
					rigidbody.AddRelativeForce(Vector3.back * 5000f, ForceMode.Force);
				}
			}

			if (max_speed < brakeFade)
			{
				max_speed = Mathf.Max(max_speed - (brakeFade - max_speed) * 1.25f, 0);
            }

			//Debug.Log(speed + " | " + max_speed);
			if (speed < max_speed && max_speed > 0)
			{
				SetWheelTorques(engine, 0f);
			}
			else
			{
				SetWheelTorques(0f, brakeMultiplier);
			}
		}

		// Back wheels
		SetWheelModel(wheels.backLeftColl, wheels.backLeftObj);
		SetWheelModel(wheels.backRightColl, wheels.backRightObj);

		// Front wheel positioning
		Vector3 leftPos, rightPos;
		Quaternion quat;
		wheels.frontLeftColl.GetWorldPose(out leftPos, out quat);
		wheels.frontRightColl.GetWorldPose(out rightPos, out quat);

		wheels.frontObj.transform.position = (rightPos + ((leftPos - rightPos) / 2));

		// Front wheel rotations
		int touching = 0;
		Vector3 forward = Vector3.zero;
		WheelHit leftHit, rightHit;

		if (wheels.backLeftColl.GetGroundHit(out leftHit))
		{
			forward += leftHit.forwardDir;
			touching++;
		}

		if (wheels.backRightColl.GetGroundHit(out rightHit))
		{
			forward += rightHit.forwardDir;
			touching++;
		}

		// Set front wheel
		frontWheelAngle = Mathf.LerpAngle(frontWheelAngle, steerInput, 0.025f + 1.5f * Time.fixedDeltaTime);
		//(wheels.frontLeftColl.steerAngle * 0.75f);

		float angle = transform.rotation.eulerAngles.x;
		if (sendPower != null)
		{
			sendPower.parameter = 80 + Mathf.RoundToInt((angle > 270) ? (360 - angle) * 4 : 0);
		}

		if (touching == 0)
		{
			forward = Quaternion.Euler(Vector3.up * frontWheelAngle) * transform.forward;
			Vector3 rotation = Vector3.Cross(leftPos - rightPos, forward);
			wheels.frontObj.transform.rotation = Quaternion.LookRotation(forward, rotation);

			// fix
			wheels.frontObj.transform.Rotate(-90f, 0f, 0f);
			return;
		}

		forward /= touching;

		Vector3 steering = Quaternion.Euler(Vector3.up * frontWheelAngle) * forward;
		Vector3 normal = Vector3.Cross(leftPos - rightPos, forward);
		wheels.frontObj.transform.rotation = Quaternion.LookRotation(steering, normal);

		// fix
		wheels.frontObj.transform.Rotate(-90f, 0f, 0f);

		Debug.DrawLine(leftPos, rightPos, Color.cyan);
		Debug.DrawRay(rightPos + ((leftPos - rightPos) / 2), forward, Color.cyan);
		Debug.DrawRay(rightPos + ((leftPos - rightPos) / 2), normal, Color.cyan);
		Debug.DrawRay(rightPos + ((leftPos - rightPos) / 2), steering, Color.red);
		
		Debug.DrawRay(rigidbody.worldCenterOfMass + (transform.forward / 2), -transform.forward, Color.red);
		Debug.DrawRay(rigidbody.worldCenterOfMass + (transform.right / 2), -transform.right, Color.red);
		Debug.DrawRay(rigidbody.worldCenterOfMass + (transform.up / 2), -transform.up, Color.red);
	}

	// On collision with an obstacle
	void OnCollisionEnter(Collision coll)
	{
		health -= (coll.relativeVelocity.magnitude / 1.5f);
		
		switch (coll.gameObject.tag)
		{
			case "Rock":
				AudioSource.PlayClipAtPoint(audioHitRock, coll.contacts[0].point, Mathf.Min(coll.impulse.magnitude * Time.fixedDeltaTime * 0.02f, 0.5f));
				break;

			case "Wood":
				AudioSource.PlayClipAtPoint(audioHitWood, coll.contacts[0].point, Mathf.Min(coll.impulse.magnitude * Time.fixedDeltaTime * 0.02f, 0.5f));
				break;
		}

		if (!world.adaptiveDifficulty) return;

		if (hits == null)
		{
			hits = new List<GameObject>();
		}

		if (!hits.Contains(coll.gameObject))
		{
			hits.Add(coll.gameObject);
		}
	}

	private void SetWheelTorques(float motor, float brake)
	{
		WheelCollider[] allWheelColls = wheels.GetColliders();
		for (int w = 0; w < allWheelColls.Length; w++)
		{
			allWheelColls[w].motorTorque = motor;// (speed < max_speed) ? motor : 0f;
			allWheelColls[w].brakeTorque = brake;
		}
	}

	// Get meter per second from RPM
	private float RPMToMpS(float rpm, float radius)
	{
		return ((2 * Mathf.PI * radius) / 60f) * rpm;
	}

	public IEnumerator ImpactExplosion(Vector3 exploPos, float range, float delay, float damage)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}

		Collider[] colls = Physics.OverlapSphere(exploPos, range, LayerMask.GetMask("Player"));
		
		if (colls.Length > 0)
		{
			rigidbody.AddExplosionForce(5000f, exploPos, 10f + 2f, 2000f, ForceMode.Impulse);
			health -= (5 + (colls.Length * damage));
		}
	}

	private void SetWheelModel(WheelCollider coll, GameObject obj)
	{
		Vector3 position;
		Quaternion quat;
		coll.GetWorldPose(out position, out quat);
		obj.transform.position = position;

		WheelHit hit;
		if (!coll.GetGroundHit(out hit)) return;

		Vector3 rot = Quaternion.LookRotation(hit.forwardDir, hit.normal).eulerAngles;
		obj.transform.rotation = Quaternion.Euler(rot);

		Vector3 local = obj.transform.localRotation.eulerAngles;
		local.x -= 90f;
		local.z = 0f;
		obj.transform.localRotation = Quaternion.Euler(local);

	}
} */