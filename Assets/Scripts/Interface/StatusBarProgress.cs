using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarProgress : MonoBehaviour
{
	public GameObject innerMask;
	
	public void setValue(float value)
	{
		var yScale = innerMask.transform.localScale.y;
		var zScale = innerMask.transform.localScale.z;
		innerMask.transform.localScale = new Vector3(1 - value, yScale, zScale);
	}
}
