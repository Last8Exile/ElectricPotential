using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class FigureManager : MonoBehaviour {

    void Start()
    {
        mSettings = MonoSingleton<SettingsManager>.Instance.Settings;
        if (mSettings.Geometry_Figures == null)
            mSettings.Geometry_Figures = LoadDefaultFigures();
        LoadFigures(mSettings.Geometry_Figures);

        mSettings.PropertyChanged += OnSettingsChanged;
    }
    private Settings mSettings;

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mSettings.Geometry_Figures):
                LoadFigures(mSettings.Geometry_Figures);
                break;
        }
    }

    [SerializeField]
    private Transform mTree, mUITree, mUIDetails;

    [SerializeField]
    private GameObject mFigurePrefab, mUIFigurePrefab, mUIDetailsPrefab;

    private List<FigureDefinition> LoadDefaultFigures()
    {
        var figures = mTree.GetComponentsInChildren<Figure>();
        return figures.Select(x => x.GetNewFigureDefinition()).ToList();
    }

    private void LoadFigures(List<FigureDefinition> figures)
    {
        if (!Application.isPlaying)
            return;

        mTree.RemoveChilds();
        mUITree.RemoveChilds();
        mUIDetails.RemoveChilds();
        foreach (var figure in figures)
        {
            Instantiate(mFigurePrefab, mTree).GetComponent<Figure>().Init(figure);
            Instantiate(mUIFigurePrefab, mUITree).GetComponent<UIFigure>().Init(figure);
        }
    }

    public void AddFigure()
    {
        var figure = new FigureDefinition();
        mSettings.Geometry_Figures.Add(figure);
        Instantiate(mFigurePrefab, mTree).GetComponent<Figure>().Init(figure);
        Instantiate(mUIFigurePrefab, mUITree).GetComponent<UIFigure>().Init(figure);
    }

    public void ShowDetails(FigureDefinition figureDefinition)
    {
        mUIDetails.RemoveChilds();
        Instantiate(mUIDetailsPrefab, mUIDetails).GetComponent<UIDetails>().Init(figureDefinition);
    }

    public void DeleteFigure(FigureDefinition figureDefinition)
    {
        mUIDetails.RemoveChilds();
        mSettings.Geometry_Figures.Remove(figureDefinition);
        figureDefinition.DoAction(FigureAction.Deleted);
    }

    public void MoveFigureUp(FigureDefinition figureDefinition)
    {
        var pos = mSettings.Geometry_Figures.IndexOf(figureDefinition);
        var action = FigureAction.MovedUP;
        if (pos == 0)
        {
            pos = mSettings.Geometry_Figures.Count;
            action = FigureAction.MoveLast;
        }
        mSettings.Geometry_Figures.Remove(figureDefinition);
        mSettings.Geometry_Figures.Insert(pos - 1, figureDefinition);
        figureDefinition.DoAction(action);
    }

    public void MoveFigureDown(FigureDefinition figureDefinition)
    {
        var pos = mSettings.Geometry_Figures.IndexOf(figureDefinition);
        var action = FigureAction.MovedDown;
        if (pos == mSettings.Geometry_Figures.Count - 1)
        {
            pos = -1;
            action = FigureAction.MoveFirsrt;
        }
        mSettings.Geometry_Figures.Remove(figureDefinition);
        mSettings.Geometry_Figures.Insert(pos + 1, figureDefinition);
        figureDefinition.DoAction(action);
    }
}
