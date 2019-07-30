using UnityEngine;

public class DisableAtStart : MonoBehaviour {
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    gameObject.SetActive(false);
	    Destroy(this);
	}
}
