﻿using UnityEngine;

public class RotateComp : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * 10);
	}
}