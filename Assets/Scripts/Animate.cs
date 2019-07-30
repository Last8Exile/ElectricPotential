using System;
using UnityEngine;

public class Animate : MonoBehaviour {

    [SerializeField]
    private Transform mFrames;
    private KeyFrame[] mKeyFrames;
    private int mCurrentFrame;

    public float Length;



	// Use this for initialization
	void Start ()
	{
	    mKeyFrames = mFrames.GetComponentsInChildren<KeyFrame>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var animTime = Time.time % Length;
	    var leftFrame = Array.BinarySearch(mKeyFrames, animTime);
        if (leftFrame < 0)
	        leftFrame = -leftFrame - 2;
	    var lerpCoeff = LetpTime(leftFrame, animTime);

        transform.position = Vector3.Lerp(mKeyFrames[leftFrame].transform.position, mKeyFrames[leftFrame + 1].transform.position, lerpCoeff);
	    transform.rotation = Quaternion.Lerp(mKeyFrames[leftFrame].transform.rotation, mKeyFrames[leftFrame + 1].transform.rotation, lerpCoeff);
	}

    private float LetpTime(int leftFrame, float time)
    {
        var leftTime = mKeyFrames[leftFrame].Time;
        var rightTime = mKeyFrames[leftFrame + 1].Time;
        return (time - leftTime) / (rightTime - leftTime);
    }
}
