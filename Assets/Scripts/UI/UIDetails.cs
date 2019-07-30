using UnityEngine;
using UnityEngine.UI;

public class UIDetails : MonoBehaviour {

    public void Init(FigureDefinition figureDefinition)
    {
        mFigureDefinition = figureDefinition;
    }

    private FigureDefinition mFigureDefinition;

    public int Type
    {
        get { return (int)mFigureDefinition.Type + 4; }
        set { mFigureDefinition.Type = (FigureType)(value - 4); }
    }

    public Vector3 Position
    {
        get { return mFigureDefinition.Position; }
        set { mFigureDefinition.Position = value; }
    }

    public Vector3 Rotation
    {
        get { return mFigureDefinition.Rotation; }
        set { mFigureDefinition.Rotation = value; }
    }

    public float Scale
    {
        get { return mFigureDefinition.Params.GetRow(0).w; }
        set
        {
            var matr = mFigureDefinition.Params;
            var row = matr.GetRow(0);
            row.w = value;
            matr.SetRow(0, row);
            mFigureDefinition.Params = matr;
        }
    }

    public Vector3 Params
    {
        get { return mFigureDefinition.Params.GetRow(0).ToVector3(); }
        set
        {
            var matr = mFigureDefinition.Params;
            var row = matr.GetRow(0);
            row.x = value.x;
            row.y = value.y;
            row.z = value.z;
            matr.SetRow(0, row);
            mFigureDefinition.Params = matr;
        }
    }
}
