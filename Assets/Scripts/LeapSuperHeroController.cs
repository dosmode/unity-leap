using UnityEngine;
using System.Collections;
using Leap;

public class LeapSuperHeroController : MonoBehaviour {
	
	Controller m_leapController;
	bool m_charging = false;
	bool m_gliding = false;
	float m_chargingTime = 0.0f;
	float m_timeSinceLastRelease = 0.0f;
	float m_minimumChargeReleaseTime = 2.0f;
	
/*	void Start () {
		m_leapController = new Controller();
		//transform.parent.animation["jump_pose"].time = 0.5f;
		transform.parent.GetComponent<Animation>()["jump_pose"].speed = 0.1f;
		transform.parent.GetComponent<Animation>()["jump_pose"].wrapMode = WrapMode.ClampForever;
	}
	*/
	
	void LeapNavigation() {
		Frame frame = m_leapController.Frame ();
	
	}
	void Start () {
		m_leapController = new Controller();
		if (transform.parent == null) {
			Debug.LogError("LeapFly must have a parent object to control"); 
		}
	}


	//starting with LeapFly.cs
	Hand GetLeftMostHand(Frame f) {
		float smallestVal = float.MaxValue;
		Hand h = null;
		for(int i = 0; i < f.Hands.Count; ++i) {
			if (f.Hands[i].PalmPosition.ToUnity().x < smallestVal) {
				smallestVal = f.Hands[i].PalmPosition.ToUnity().x;
				h = f.Hands[i];
			}
		}
		return h;
	}
	
	Hand GetRightMostHand(Frame f) {
		float largestVal = -float.MaxValue;
		Hand h = null;
		for(int i = 0; i < f.Hands.Count; ++i) {
			if (f.Hands[i].PalmPosition.ToUnity().x > largestVal) {
				largestVal = f.Hands[i].PalmPosition.ToUnity().x;
				h = f.Hands[i];
			}
		}
		return h;
	}
	
	void FixedUpdate () {
		
		Frame frame = m_leapController.Frame();
		
		if (frame.Hands.Count >= 2) {
			Hand leftHand = GetLeftMostHand(frame);
			Hand rightHand = GetRightMostHand(frame);
			
			// takes the average vector of the forward vector of the hands, used for the
			// pitch of the plane.
			Vector3 avgPalmForward = (frame.Hands[0].Direction.ToUnity() + frame.Hands[1].Direction.ToUnity()) * 0.5f;
			
			Vector3 handDiff = leftHand.PalmPosition.ToUnityScaled() - rightHand.PalmPosition.ToUnityScaled();
			
			Vector3 newRot = transform.parent.localRotation.eulerAngles;
			newRot.z = -handDiff.y * 20.0f;
		
			 //adding the rot.z as a way to use banking (rolling) to turn.
			newRot.y += handDiff.z * 3.0f - newRot.z * 0.03f * transform.parent.GetComponent<Rigidbody>().velocity.magnitude;
			newRot.x = -(avgPalmForward.y - 0.1f) * 100.0f;
			
			float forceMult = 10.0f;
			
			// if closed fist, then stop the plane and slowly go backwards.
			if (frame.Fingers.Count < 3) {
				forceMult = -3.0f;
			}
			
			transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, Quaternion.Euler(newRot), 0.1f);
			transform.parent.GetComponent<Rigidbody>().velocity = transform.parent.forward * forceMult;
		}
		
	}

	void DetectGliding() {
		RaycastHit hit;
		m_gliding = true;
		if (Physics.Raycast(new Ray(transform.parent.position, -Vector3.up), 5.0f)) {
			// set alignment of the character.
			Quaternion target = Quaternion.Euler(new Vector3(0, transform.parent.rotation.eulerAngles.y, 0));
			transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, target, 0.1f);
			m_gliding = false;
		}
	}


	//End
	
	/*void FixedUpdate () {
		DetectGliding();
		LeapNavigation();
		
		if (transform.parent.GetComponent<Rigidbody>().velocity.magnitude > 5.5f && !m_gliding && !m_charging) {
			transform.parent.GetComponent<Animation>().CrossFade("walk");
			transform.parent.GetComponent<Animation>()["walk"].speed = transform.parent.GetComponent<Rigidbody>().velocity.magnitude * 0.4f;
		} else if (transform.parent.GetComponent<Rigidbody>().velocity.magnitude > 0.5f && !m_gliding && !m_charging) {
			transform.parent.GetComponent<Animation>().CrossFade("walk");
			transform.parent.GetComponent<Animation>()["walk"].speed = 1f;
		} else if (!m_charging) {
			transform.parent.GetComponent<Animation>().CrossFade("idle");	
		}
	}
	*/
}
