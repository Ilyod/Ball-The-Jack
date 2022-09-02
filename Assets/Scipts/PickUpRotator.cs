using UnityEngine;
using System.Collections;

public class PickUpRotator : MonoBehaviour
{

	// Before rendering each frame..
	void Update()
	{
		// Rotate the game object that this script is attached to by 30 in the X axis,
		// 30 in the Y axis and 45 in the Z axis, multiplied by deltaTime in order to make it per second
		// rather than per frame.
		transform.Rotate(new Vector3(40, 30, 45) * Time.deltaTime);
	}
}