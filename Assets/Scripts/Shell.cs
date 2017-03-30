using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    public Rigidbody shellRigidBody;
    public float ejectionForceMin;
    public float ejectionForceMax;

    float shellLifeTime = 3;
    float shellFadeTime = 2;


	void Start () {
        float ejectionForce = Random.Range(ejectionForceMin, ejectionForceMax);
        shellRigidBody.AddForce(transform.right * ejectionForce);
        shellRigidBody.AddTorque(Random.insideUnitSphere * ejectionForce);
        StartCoroutine(shellFadeOut());
	}

    IEnumerator shellFadeOut() {
        yield return new WaitForSeconds(shellLifeTime);

        float fadePercentage = 0;
        float fadeSpeed = 1 / shellFadeTime;
        Material shellMaterial = GetComponent<Renderer>().material;
        Color shellInitailColor = shellMaterial.color;

        while (fadePercentage < 1) {
            fadePercentage += Time.deltaTime * fadeSpeed;
            shellMaterial.color = Color.Lerp(shellInitailColor, Color.clear, fadePercentage);
            yield return null;
        }
        Destroy(gameObject);
    }
}
