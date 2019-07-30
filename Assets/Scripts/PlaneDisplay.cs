using System.ComponentModel;
using UnityEngine;

public class PlaneDisplay : MonoBehaviour
{

    void Start()
    {
        mSettings = MonoSingleton<SettingsManager>.Instance.Settings;
        mSettings.PropertyChanged += OnSettingsChanged;
        transform.localPosition = mSettings.Field_Position;
        transform.localEulerAngles = mSettings.Field_Rotation;
        gameObject.SetActive(false);

        if (mSmooth)
            PlaneMaterial.EnableKeyword("SMOOTH");
        else
            PlaneMaterial.DisableKeyword("SMOOTH");
    }
    private Settings mSettings;

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mSettings.Field_Enabled):
                gameObject.SetActive(mCanShow && Enabled);
                break;
            case nameof(mSettings.Field_Position):
                transform.localPosition = mSettings.Field_Position;
                break;
            case nameof(mSettings.Field_Rotation):
                transform.localEulerAngles = mSettings.Field_Rotation;
                break;
        }
    }

    public bool Enabled => mSettings.Field_Enabled;

    public float IsoLineValue => mSettings.Field_IsoLine;

    public float Width => mSettings.Field_LineWidth;

    public float Transparency => mSettings.Field_Transparency;

    public bool Smooth => mSettings.Field_Smooth;
    private bool mSmooth = true;

    public bool CanShow
    {
        set
        {
            mCanShow = value;
            gameObject.SetActive(mCanShow && Enabled);
        }
    }
    private bool mCanShow = false;

    public Material PlaneMaterial;
    public Transform MainCamera;

    // Update is called once per frame
    void Update ()
	{
	    if (transform.InverseTransformPoint(MainCamera.position).y > 0)
	        transform.localScale = transform.localScale.Multiply(1, -1, 1);


	    PlaneMaterial.SetFloat("_Transparency", Transparency);
	    PlaneMaterial.SetFloat("_Width", Width);
	    PlaneMaterial.SetFloat("_PI", Mathf.PI);
	    PlaneMaterial.SetFloat("_PI_Inv", 1 / Mathf.PI);
        if (Smooth != mSmooth)
        {
            mSmooth = Smooth;
            if (mSmooth)
                PlaneMaterial.EnableKeyword("SMOOTH");
            else
                PlaneMaterial.DisableKeyword("SMOOTH");
        }
        PlaneMaterial.SetFloat("_ModValue", IsoLineValue);
    }
}
