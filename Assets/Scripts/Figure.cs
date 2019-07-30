using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class Figure : MonoBehaviour {

    public void Init(FigureDefinition figureDefinition)
    {
        mFigureDefinition = figureDefinition;
        mFigureDefinition.PropertyChanged += OnValueChanged;
        mFigureDefinition.DoneAction += OnDoneAction;

        gameObject.SetActive(mFigureDefinition.Enabled);
        name = mFigureDefinition.Type.ToString();
        transform.position = mFigureDefinition.Position;
        transform.eulerAngles = mFigureDefinition.Rotation;

        Type = mFigureDefinition.Type;
        Params = mFigureDefinition.Params;
    }

    public FigureDefinition GetNewFigureDefinition()
    {
        var figureDefinition = new FigureDefinition();
        figureDefinition.Enabled = enabled;
        figureDefinition.Type = Type;
        figureDefinition.Params = Params;
        figureDefinition.Position = transform.position;
        figureDefinition.Rotation = transform.eulerAngles;
        return figureDefinition;
    }

    private void OnValueChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mFigureDefinition.Enabled):
                gameObject.SetActive(mFigureDefinition.Enabled);
                break;
            case nameof(mFigureDefinition.Type):
                Type = mFigureDefinition.Type;
                break;
            case nameof(mFigureDefinition.Position):
                transform.position = mFigureDefinition.Position;
                break;
            case nameof(mFigureDefinition.Rotation):
                transform.eulerAngles = mFigureDefinition.Rotation;
                break;
            case nameof(mFigureDefinition.Params):
                Params = mFigureDefinition.Params;
                break;
        }
    }

    private void OnDoneAction(FigureAction action)
    {
        switch (action)
        {
            case FigureAction.Deleted:
                Destroy(gameObject);
                break;
            case FigureAction.MovedUP:
                transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                break;
            case FigureAction.MovedDown:
                transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                break;
            case FigureAction.MoveFirsrt:
                transform.SetAsFirstSibling();
                break;
            case FigureAction.MoveLast:
                transform.SetAsLastSibling();
                break;
        }
    }

    void OnDestroy()
    {
        if (mFigureDefinition != null)
        {
            mFigureDefinition.PropertyChanged -= OnValueChanged;
            mFigureDefinition.DoneAction -= OnDoneAction;
        }
    }

    public FigureType Type;
    public Matrix4x4 Params;

    private FigureDefinition mFigureDefinition;
}


public class FigureDefinition
{
    #region ~Enabled~

    public bool Enabled
    {
        get { return mEnabled; }
        set { SetProperty(ref mEnabled, value); }
    }
    private bool mEnabled = true;

    #endregion ~Enabled~            

    #region ~Type~

    public FigureType Type
    {
        get { return mType; }
        set { SetProperty(ref mType, value); }
    }
    private FigureType mType = FigureType.Box;

    #endregion ~Type~                      

    #region ~Position~

    public Vector3 Position
    {
        get { return mPosition; }
        set { SetProperty(ref mPosition, value); }
    }
    private Vector3 mPosition;

    #endregion ~Position~            

    #region ~Rotation~

    public Vector3 Rotation
    {
        get { return mRotation; }
        set { SetProperty(ref mRotation, value); }
    }
    private Vector3 mRotation;

    #endregion ~Rotation~            

    #region ~Params~

    public Matrix4x4 Params
    {
        get { return mParams; }
        set { SetProperty(ref mParams, value); }
    }
    private Matrix4x4 mParams;

    #endregion ~Params~                     

    public event Action<FigureAction> DoneAction;

    public void DoAction(FigureAction action) => DoneAction?.Invoke(action);

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

public enum FigureType
{
    [Description("Мягкое объединение")]
    Blend = -4,
    [Description("Отсечение")]
    Substract = -3,
    [Description("Пересечение")]
    Intersect = -2,
    [Description("Объединение")]
    Union = -1,
    [Description("Параллелепипед")]
    Box = 0,
    [Description("Эллипс")]
    Sphere = 1,
    [Description("Циллиндр")]
    Cilynder = 2,
    [Description("Тор")]
    Torus = 3
}

public enum FigureAction
{
    Deleted,
    MovedUP,
    MovedDown,
    MoveFirsrt,
    MoveLast
}
