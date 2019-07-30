using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIFigure : MonoBehaviour, IPointerClickHandler
{

    public void Init(FigureDefinition figureDefinition)
    {
        mFigureDefinition = figureDefinition;
        mFigureDefinition.PropertyChanged += OnValueChanged;
        mFigureDefinition.DoneAction += OnDoneAction;

        mText.text = mFigureDefinition.Type.GetDescription();
        mToggle.isOn = mFigureDefinition.Enabled;
    }

    [SerializeField]
    private Text mText;

    [SerializeField]
    private Toggle mToggle;

    private FigureDefinition mFigureDefinition;

    private void OnValueChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(mFigureDefinition.Type):
                mText.text = mFigureDefinition.Type.ToString();
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

    public bool Enabled
    {
        set { mFigureDefinition.Enabled = value; }
    }

    public void Delete()
    {
        MonoSingleton<FigureManager>.Instance.DeleteFigure(mFigureDefinition);
    }

    public void MoveUp()
    {
        MonoSingleton<FigureManager>.Instance.MoveFigureUp(mFigureDefinition);
    }

    public void MoveDown()
    {
        MonoSingleton<FigureManager>.Instance.MoveFigureDown(mFigureDefinition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MonoSingleton<FigureManager>.Instance.ShowDetails(mFigureDefinition);
    }
}
