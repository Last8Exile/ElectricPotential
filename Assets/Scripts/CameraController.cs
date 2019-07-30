using System.ComponentModel;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    void Start()
    {
        mSettings = MonoSingleton<SettingsManager>.Instance.Settings;
        mSettings.PropertyChanged += OnSettingsChanged;
    }
    private Settings mSettings;

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mSettings.Camera_FOV):
                mCamera.fieldOfView = mSettings.Camera_FOV;
                break;
        }
    }

    [SerializeField]
    private Camera mCamera = null;

    [SerializeField]
    private Vector3 mTranslateSpeed;

    private float mTranslateSpeedMult = 0;

    [Range(1, 10)]
    public float TranslateSpeedMultSpeed = 7;

    [SerializeField]
    private Vector2 mRotationSpeed;

    [SerializeField]
    private float mMinimumY = -80f;

    [SerializeField]
    private float mMaximumY = 80f;

    float mRotationY = 0F;

    public float RotatiuonSpeedMult => mSettings.Camera_RotatiuonSpeedMult;

/*
[SerializeField]
private float mDefaultFOV = 60;
[SerializeField]
private float mMinFOV = 10;
[SerializeField]
private float mMaxFOV = 150;

[SerializeField]
private float mFOVSpeed = 1f;
[SerializeField]
private float mSizeSpeed = 0.25f;
*/


    // Update is called once per frame
    void Update ()
    {
        transform.Translate(new Vector3(Input.GetAxis("X"), Input.GetAxis("Y"), Input.GetAxis("Z")).Multiply(mTranslateSpeed * Mathf.Pow(2, mTranslateSpeedMult / TranslateSpeedMultSpeed)));
	    if (Input.GetMouseButton(1))
	    {
	        var rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mRotationSpeed.x * Mathf.Pow(2,RotatiuonSpeedMult);

            mRotationY += Input.GetAxis("Mouse Y") * mRotationSpeed.y * Mathf.Pow(2, RotatiuonSpeedMult);
	        mRotationY = Mathf.Clamp(mRotationY, mMinimumY, mMaximumY);

            transform.localEulerAngles = new Vector3(-mRotationY, rotationX, 0);
        }


        mTranslateSpeedMult += Input.mouseScrollDelta.y;
	    if (Input.GetMouseButton(2))
	    {
	        mTranslateSpeedMult = 0; 
	    }

        // Угол обзора через колесо мыши
        /*
	    if (mCamera.orthographic)
	        mCamera.orthographicSize -= Input.mouseScrollDelta.y * mSizeSpeed;
	    else
	        mCamera.fieldOfView = Mathf.Clamp(mCamera.fieldOfView - Input.mouseScrollDelta.y * mFOVSpeed, mMinFOV, mMaxFOV);
        

	    if (Input.GetMouseButton(2))
	    {
	        mCamera.fieldOfView = mDefaultFOV;
	    }
        */
    }
}
