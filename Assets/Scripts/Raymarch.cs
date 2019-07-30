using UnityEngine;
using System.ComponentModel;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/Raymarch (Generic)")]
public class Raymarch : SceneViewFilter
{

    void Start()
    {
        mSettings = MonoSingleton<SettingsManager>.Instance.Settings;
        mSettings.PropertyChanged += OnSettingsChanged;
        if (Application.isPlaying)
            enabled = Enabled;
    }
    private Settings mSettings;

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mSettings.Raymarching_Enabled):
                enabled = Enabled;
                break;
        }
    }

    public bool Enabled => mSettings.Raymarching_Enabled;

    [SerializeField]
    private Shader mEffectShader;
    [SerializeField]
    private Transform mSunLight;

    public int MaxSteps => mSettings.Raymarching_MaxSteps;

    public float MaxDistance => mSettings.Raymarching_MaxDistance;

    public bool UpdateField => mSettings.Geometry_UpdateWorld;

    public Material EffectMaterial
    {
        get
        {
            if (!mEffectMaterial && mEffectShader)
            {
                mEffectMaterial = new Material(mEffectShader);
                mEffectMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return mEffectMaterial;
        }
    }
    private Material mEffectMaterial;

    public Camera CurrentCamera
    {
        get
        {
            if (!mCurrentCamera)
                mCurrentCamera = GetComponent<Camera>();
            return mCurrentCamera;
        }
    }
    private Camera mCurrentCamera;

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!EffectMaterial)
        {
            Graphics.Blit(source, destination); // do nothing
            return;
        }

        EffectMaterial.SetMatrix("_FrustumCornersES", GetFrustumCorners(CurrentCamera));
        EffectMaterial.SetMatrix("_CameraInvViewMatrix", CurrentCamera.cameraToWorldMatrix);
        EffectMaterial.SetVector("_CameraWS", CurrentCamera.transform.position);
        EffectMaterial.SetVector("_LightDir", mSunLight ? mSunLight.forward : Vector3.down);
        EffectMaterial.SetInt("_MaxStep", Application.isPlaying ? MaxSteps : 256);
        EffectMaterial.SetFloat("_MaxDist", Application.isPlaying ? MaxDistance : 10);

        Extensions.CustomGraphicsBlit(source, destination, EffectMaterial, 0); // use given effect shader as image effect
    }

    [SerializeField][ShowOnly]
    private int mFiguresCount;

    private Figure[] mFigures;
    public Transform Tree;

    void Update()
    {
        if (!EffectMaterial || Application.isPlaying && !UpdateField)
        {
            return;
        }

        if (Tree == null)
            return;
        mFigures = Tree.GetComponentsInChildren<Figure>();
        mFiguresCount = mFigures.Length;

        EffectMaterial.SetInt("_Size", mFiguresCount);

        var numbers = new float[64];
        var parameters = new Matrix4x4[64];
        var transforms = new Matrix4x4[64];
        for (int i = 0; i < mFiguresCount; i++)
        {
            numbers[i] = (float) mFigures[i].Type;
            parameters[i] = mFigures[i].Params;
            transforms[i] = mFigures[i].transform.worldToLocalMatrix;
        }

        EffectMaterial.SetFloatArray("_Numbers", numbers);
        EffectMaterial.SetMatrixArray("_Transforms", transforms);
        EffectMaterial.SetMatrixArray("_Params", parameters);
    }

    /// \brief Stores the normalized rays representing the camera frustum in a 4x4 matrix.  Each row is a vector.
    /// 
    /// The following rays are stored in each row (in eyespace, not worldspace):
    /// Top Left corner:     row=0
    /// Top Right corner:    row=1
    /// Bottom Right corner: row=2
    /// Bottom Left corner:  row=3
    private Matrix4x4 GetFrustumCorners(Camera cam)
    {
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        Matrix4x4 frustumCorners = Matrix4x4.identity;

        float fovWHalf = camFov * 0.5f;

        float tan_fov = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 toRight = Vector3.right * tan_fov * camAspect;
        Vector3 toTop = Vector3.up * tan_fov;

        Vector3 topLeft = (-Vector3.forward - toRight + toTop);
        Vector3 topRight = (-Vector3.forward + toRight + toTop);
        Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
        Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

        frustumCorners.SetRow(0, topLeft);
        frustumCorners.SetRow(1, topRight);
        frustumCorners.SetRow(2, bottomRight);
        frustumCorners.SetRow(3, bottomLeft);

        return frustumCorners;
    }
}