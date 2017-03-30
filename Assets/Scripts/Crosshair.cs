using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color colorOnTarget;
    Color colorOffTarget;

    private void Start() {
        Cursor.visible = false;
        colorOffTarget = dot.color;
    }

	void Update () {
        transform.Rotate(Vector3.forward * Time.deltaTime * -40);
	}

    public void TargetLock(Ray ray) {
        if (Physics.Raycast(ray, 100, targetMask)) {
            dot.color = colorOnTarget;
        }
        else {
            dot.color = colorOffTarget;
        }
    }
}
