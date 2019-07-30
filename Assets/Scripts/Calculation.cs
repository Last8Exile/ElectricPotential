using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Calculation : MonoBehaviour
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
            case nameof(mSettings.Camera_FilterTextures):
                if (Display != null)
                    Display.filterMode = FilterMode;
                break;
            case nameof(mSettings.Raymarching_Cut):
                if (mCutFigure != null)
                    mCutFigure.SetActive(mSettings.Raymarching_Cut);
                break;
        }
    }

    public int N => mSettings.Data_N;
    public float StartPotential => mSettings.Data_StartValue;
    public FilterMode FilterMode => mSettings.Camera_FilterTextures ? FilterMode.Trilinear : FilterMode.Point;
    public bool HardIteration => mSettings.Iterations_HardFormula;
    public int MaxSteps => mSettings.Iterations_MaxSteps;
    public float Interval => mSettings.Iterations_Interval;
    public bool Cut => mSettings.Raymarching_Cut;


    public RenderTexture RenTexture, Buffer, Display;
    public ComputeShader Init, Iteraition, Copy, IterationHard;
    public Material PlaneMaterial;

    [SerializeField][ShowOnly] private int mFiguresCount;
    private Figure[] mFigures;
    [SerializeField]
    private Transform mTree;

    [SerializeField]
    private GameObject mCutPrefab;
    [SerializeField]
    private Transform mCutAnchor;
    [SerializeField]
    private PlaneDisplay mPlane;


    private float mLastTime;
    private bool mStart;
    private GameObject mCutFigure;

    private ComputeBuffer mBuffer;



    public void StartCalc()
    {
        StartCoroutine(InitModel());
    }

    public struct ShaderFigure
    {
        public int Type;
        public Matrix4x4 Matr;
        public Matrix4x4 Params;
    };

    public IEnumerator InitModel()
    {
        RenTexture = new RenderTexture(N, N, 24, RenderTextureFormat.RFloat)
        {
            enableRandomWrite = true,
            volumeDepth = N,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        RenTexture.Create();

        Buffer = new RenderTexture(N, N, 24, RenderTextureFormat.RFloat)
        {
            enableRandomWrite = true,
            volumeDepth = N,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        Buffer.Create();

        Display = new RenderTexture(N, N, 24, RenderTextureFormat.RFloat)
        {
            enableRandomWrite = true,
            volumeDepth = N,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            filterMode = FilterMode,
            wrapMode = TextureWrapMode.Clamp
        };
        Display.Create();


        yield return new WaitForSeconds(1);


        PlaneMaterial.SetTexture("_MainTex", Display);
        PlaneMaterial.SetFloat("_MaxValue", StartPotential);
        


        var initKernelId = Init.FindKernel("CSMain");
        Init.SetTexture(initKernelId, "_Result", RenTexture);
        Init.SetFloat("_InitialValue", StartPotential);
        Init.SetFloat("_NodesCount", N);

        mFigures = mTree.GetComponentsInChildren<Figure>();
        mFiguresCount = mFigures.Length;

        //Init.SetInt("_MaxStep", mMaxSteps);
        //Init.SetFloat("_MaxDist", MaxDistance);
        Init.SetInt("_FiguresCount", mFiguresCount);

        mBuffer = new ComputeBuffer(mFiguresCount, 132);
        var data = mFigures.Select((x) => new ShaderFigure
        {
            Type = (int) x.Type,
            Matr = x.transform.worldToLocalMatrix,
            Params = x.Params
        }).ToArray();
        mBuffer.SetData(data);

        Init.SetBuffer(initKernelId, "_Figures", mBuffer);
        var n4 = N / 4;
        Init.Dispatch(initKernelId, n4, n4, n4);
        //buffer.Dispose();

        yield return new WaitForSeconds(1);


        mIterationKernelId = Iteraition.FindKernel("CSMain");
        Iteraition.SetTexture(mIterationKernelId, "_Source", RenTexture);
        Iteraition.SetTexture(mIterationKernelId, "_Result", Buffer);


        mIterationHardKernelId = IterationHard.FindKernel("CSMain");
        IterationHard.SetTexture(mIterationKernelId, "_Source", RenTexture);
        IterationHard.SetTexture(mIterationKernelId, "_Result", Buffer);
        IterationHard.SetFloat("_InitialValue", StartPotential);
        IterationHard.SetFloat("_NodesCount", N);
        IterationHard.SetInt("_MaxStep", MaxSteps);
        IterationHard.SetFloat("_Dist", 1f / (N - 1));
        //IterationHard.SetFloat("_MaxDist", MaxDistance);
        IterationHard.SetInt("_FiguresCount", mFiguresCount);
        IterationHard.SetBuffer(mIterationHardKernelId, "_Figures", mBuffer);

        mCopyKernelId = Copy.FindKernel("CSMain");
        Copy.SetTexture(mCopyKernelId, "_Source", Buffer);
        Copy.SetTexture(mCopyKernelId, "_Result", RenTexture);
        Copy.SetTexture(mCopyKernelId, "_ResultAbs", Display);


        yield return new WaitForSeconds(1);

        mCutFigure = Instantiate(mCutPrefab, mTree);
        mCutFigure.SetActive(Cut);
        mCutFigure.GetComponent<Synch>().From = mCutAnchor;

        mPlane.CanShow = true;

        mStart = true;
        yield break;
    }

    void Update()
    {
        if (!mStart)
            return;
        if (Time.time < mLastTime + Interval)
            return;

        mLastTime = Time.time;
        if (HardIteration)
            DoHardIteration();
        else
            DoIteration();
    }

    private void OnDestroy()
    {
        mBuffer?.Dispose();
        if (RenTexture != null)
            Destroy(RenTexture);
        if (Buffer != null)
            Destroy(Buffer);
        if (Display != null)
            Destroy(Display);
    }

    private int mIterationKernelId, mCopyKernelId, mIterationHardKernelId;

    private void DoIteration()
    {
        int n4 = N / 4;
        int n8 = n4 / 2;
        Iteraition.Dispatch(mIterationKernelId, n4, n4, n4);
        Copy.Dispatch(mCopyKernelId, n8, n8, n8);
    }

    private void DoHardIteration()
    {
        int n4 = N / 4;
        int n8 = n4 / 2;
        IterationHard.Dispatch(mIterationHardKernelId, n4, n4, n4);
        Copy.Dispatch(mCopyKernelId, n8, n8, n8);
    }
}
