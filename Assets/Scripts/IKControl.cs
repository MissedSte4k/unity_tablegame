using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour {

	protected Animator animator;

	public bool ikActive = false;
	public Transform rightHandObj;

	void Start()
	{
		animator = GetComponent<Animator> ();
	}

	void OnAnimatorIK(){
		animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
		animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
		animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
		animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);	
	}
}
