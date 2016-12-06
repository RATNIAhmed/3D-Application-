using UnityEngine;
using System.Collections;

public class Validation : MonoBehaviour {

    public GameObject correct;

	// Use this for initialization
	void Start () {
        if (correct == null)
            Debug.LogError("needed correct gameobject to validate");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool isValidComponentTarget()
    {
        return (this.transform.childCount == 1) && this.transform.GetChild(0).gameObject.GetInstanceID() == correct.GetInstanceID()
            && this.transform.GetChild(0).GetComponent<ObjectMovement>().isOnDesiredRotation();
    }
}
