using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Settings : INotifyPropertyChanged
{

    #region ~Visualisation~

    #region ~Camera~

    #region ~Camera_FOV~

    public float Camera_FOV
    {
        get { return mCamera_FOV; }
        set { SetProperty(ref mCamera_FOV, value); }
    }
    private float mCamera_FOV = 60;

    #endregion ~Camera_FOV~            

    #region ~Camera_RotatiuonSpeedMult~

    public float Camera_RotatiuonSpeedMult
    {
        get { return mCamera_RotatiuonSpeedMult; }
        set { SetProperty(ref mCamera_RotatiuonSpeedMult, value); }
    }
    private float mCamera_RotatiuonSpeedMult = 0;

    #endregion ~Camera_RotatiuonSpeedMult~            

    #region ~Camera_FilterTextures~

    public bool Camera_FilterTextures
    {
        get { return mCamera_FilterTextures; }
        set { SetProperty(ref mCamera_FilterTextures, value); }
    }
    private bool mCamera_FilterTextures = true;

    #endregion ~Camera_FilterTextures~            

    #endregion ~Camera~

    #region ~Raymarching~

    #region ~Raymarching_Enabled~

    public bool Raymarching_Enabled
    {
        get { return mRaymarching_Enabled; }
        set { SetProperty(ref mRaymarching_Enabled, value); }
    }
    private bool mRaymarching_Enabled = true;

    #endregion ~Raymarching_Enabled~            

    #region ~Raymarching_MaxSteps~

    public int Raymarching_MaxSteps
    {
        get { return mRaymarching_MaxSteps; }
        set { SetProperty(ref mRaymarching_MaxSteps, value); }
    }
    private int mRaymarching_MaxSteps = 256;

    #endregion ~Raymarching_MaxSteps~            

    #region ~Raymarching_MaxDistance~

    public float Raymarching_MaxDistance
    {
        get { return mRaymarching_MaxDistance; }
        set { SetProperty(ref mRaymarching_MaxDistance, value); }
    }
    private float mRaymarching_MaxDistance = 10;

    #endregion ~Raymarching_MaxDistance~            

    #region ~Raymarching_Cut~

    public bool Raymarching_Cut
    {
        get { return mRaymarching_Cut; }
        set { SetProperty(ref mRaymarching_Cut, value); }
    }
    private bool mRaymarching_Cut = true;

    #endregion ~Raymarching_Cut~            

    #endregion ~Raymarching~

    #region ~Field~

    #region ~Field_Enabled~

    public bool Field_Enabled
    {
        get { return mField_Enabled; }
        set { SetProperty(ref mField_Enabled, value); }
    }
    private bool mField_Enabled = true;

    #endregion ~Field_Enabled~            

    #region ~Field_IsoLine~

    public float Field_IsoLine
    {
        get { return mField_IsoLine; }
        set { SetProperty(ref mField_IsoLine, value); }
    }
    private float mField_IsoLine = 1;

    #endregion ~Field_IsoLine~            

    #region ~Field_Transparency~

    public float Field_Transparency
    {
        get { return mField_Transparency; }
        set { SetProperty(ref mField_Transparency, value); }
    }
    private float mField_Transparency = 1;

    #endregion ~Field_Transparency~            

    #region ~Field_LineWidth~

    public float Field_LineWidth
    {
        get { return mField_LineWidth; }
        set { SetProperty(ref mField_LineWidth, value); }
    }
    private float mField_LineWidth = 0.2f;

    #endregion ~Field_LineWidth~            

    #region ~Field_Smooth~

    public bool Field_Smooth
    {
        get { return mField_Smooth; }
        set { SetProperty(ref mField_Smooth, value); }
    }
    private bool mField_Smooth = true;

    #endregion ~Field_Smooth~            

    #region ~Field_Position~

    public Vector3 Field_Position
    {
        get { return mField_Position; }
        set { SetProperty(ref mField_Position, value); }
    }
    private Vector3 mField_Position = new Vector3(0.5206f, 0.5f, 0.4412f);

    #endregion ~Field_Position~            

    #region ~Field_Rotation~

    public Vector3 Field_Rotation
    {
        get { return mField_Rotation; }
        set { SetProperty(ref mField_Rotation, value); }
    }
    private Vector3 mField_Rotation = new Vector3(90f, 0f, 18.4f);

    #endregion ~Field_Rotation~            

    #endregion ~Field~

    #endregion ~Visualisation~

    #region ~Geometry~

    #region ~Geometry_UpdateWorld~

    public bool Geometry_UpdateWorld
    {
        get { return mGeometry_UpdateWorld; }
        set { SetProperty(ref mGeometry_UpdateWorld, value); }
    }
    private bool mGeometry_UpdateWorld = true;

    #endregion ~Geometry_UpdateWorld~            

    #region ~Geometry_Figures~

    public List<FigureDefinition> Geometry_Figures
    {
        get { return mGeometry_Figures; }
        set { SetProperty(ref mGeometry_Figures, value); }
    }
    private List<FigureDefinition> mGeometry_Figures;

    #endregion ~Geometry_Figures~            

    #endregion ~Geometry~

    #region ~Calculations~

    #region ~Iterations~

    #region ~Iterations_Interval~

    public float Iterations_Interval
    {
        get { return mIterations_Interval; }
        set { SetProperty(ref mIterations_Interval, value); }
    }
    private float mIterations_Interval = 0;

    #endregion ~Iterations_Interval~            

    #region ~Iterations_HardFormula~

    public bool Iterations_HardFormula
    {
        get { return mIterations_HardFormula; }
        set { SetProperty(ref mIterations_HardFormula, value); }
    }
    private bool mIterations_HardFormula = false;

    #endregion ~Iterations_HardFormula~            

    #region ~Iterations_MaxSteps~

    public int Iterations_MaxSteps
    {
        get { return mIterations_MaxSteps; }
        set { SetProperty(ref mIterations_MaxSteps, value); }
    }
    private int mIterations_MaxSteps = 16;

    #endregion ~Iterations_MaxSteps~            

    #endregion ~Iterations~

    #region ~Data~

    #region ~Data_N~

    public int Data_N
    {
        get { return mData_N; }
        set { SetProperty(ref mData_N, value); }
    }
    private int mData_N = 256;

    #endregion ~Data_N~            

    #region ~Data_StartValue~

    public float Data_StartValue
    {
        get { return mData_StartValue; }
        set { SetProperty(ref mData_StartValue, value); }
    }
    private float mData_StartValue = 10;

    #endregion ~Data_StartValue~            

    #endregion ~Data~

    #endregion ~Calculations~

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = "")
    {
        if (Equals(storage, value))
            return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion INotifyPropertyChanged
}
