using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour 
{
	public int rotationOffset = 0;
	public float rot = 0;
	public CharacterController2D characterController;

	private void FixedUpdate() {
		// subtracting the position of the player from the mouse position
		Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
		difference.Normalize ();		// normalizing the vector. Meaning that all the sum of the vector will be equal to 1

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;	// find the angle in degrees
		
		/*
			First, the arm is a child of player so the arm gets flipped as well, 
			so i will need to account for that. 
		*/

		if (Mathf.Abs(rotZ) > 90f && characterController.m_FacingRight)
		{
            characterController.Flip();
		}
        
		else if (Mathf.Abs(rotZ) < 90f && !characterController.m_FacingRight) 
		{
            characterController.Flip();
		}
 
         // Since player's local scale is fliped
         // you'll need to adjust the rotation too
		if (!characterController.m_FacingRight) {
			rotZ += 180;
		}		
		
		rot = rotZ;
		transform.rotation = Quaternion.Euler (0f, 0f, rotZ + rotationOffset);	
	}
}
