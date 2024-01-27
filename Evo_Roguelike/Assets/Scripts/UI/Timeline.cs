using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Timeline : MonoBehaviour
{
    public RectMask2D rectMask;
    public Sprite horLeftEdge;
    public Sprite horStraight;
    public Sprite horRightEdge;
    public Sprite verBigNotch;
    public Sprite verSmallNotch;
    public Sprite arrow;

    [SerializeField]
    private float _timelineHeight = 200;
    [SerializeField]
    private int _timelineExtent = 5;
    [SerializeField]
    private float _distanceBetweenSteps = 20;
    [SerializeField]
    private int _timelineYOffset = -10;
    [SerializeField]
    private int _arrowYOffset = -70; 
    [SerializeField, Tooltip("Enable to see property changes reflected in realtime (Only for designing)")]
    private bool _bLiveEdit = false;

    LinkedList<GameObject> _notches = new LinkedList<GameObject>();
    List<GameObject> _lineImgs = new List<GameObject>();

    private TimeManager _timeManager;
    private HazardManager _hazardManager;
    private int _currentHazard = 0;
    

    private void Start()
    {

    }

    private void OnEnable()
    {
        _hazardManager = ServiceLocator.Instance.GetService<HazardManagerBehaviour>().HazardManager;
        if(_hazardManager != null )
        {
            _hazardManager.dHazardsGenerated += OnHazardsGenerated;
        }
        _timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
        if(_timeManager != null )
        {
            _timeManager.D_tick += OnTick;  
        }
    }

    private void OnDisable()
    {
        if(_hazardManager != null )
        {
            _hazardManager.dHazardsGenerated -= OnHazardsGenerated;
        }
        if (_timeManager != null)
        {
            _timeManager.D_tick -= OnTick;
        }
    }

    private void ConstructTimeline()
    {
        ClearLine();
        ClearNotches();

        int i = -(_timelineExtent + 1);
        _lineImgs.Add(CreateAndAddImage(horLeftEdge, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "leftEdge"));
        i++;
        while(i <= _timelineExtent)
        {
            _lineImgs.Add(CreateAndAddImage(horStraight, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "middle" + (i + _timelineExtent)));
            i++;
        }
        _lineImgs.Add(CreateAndAddImage(horRightEdge, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "rightEdge"));

        _lineImgs.Add(CreateAndAddImage(arrow, new Vector2(0, _arrowYOffset)));

        for(int j = -_timelineExtent; j <= _timelineExtent + 1; j++)
        {
            int timeStep = _timeManager.CurrentTimeStep + j;
            if (_hazardManager.GetHazardsAtTimeStamp(timeStep) != null)
            {
                _notches.AddLast(CreateAndAddImage(verBigNotch, new Vector2(j * _distanceBetweenSteps, _timelineYOffset), "notch" + (j + _timelineExtent)));
            }
            else
            {
                _notches.AddLast(CreateAndAddImage(verSmallNotch, new Vector2(j * _distanceBetweenSteps, _timelineYOffset), "notch" + (j + _timelineExtent)));
            }
        }

        rectMask.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((2 * (_timelineExtent + 1) * _distanceBetweenSteps) - 10, _timelineHeight);
    }

    private GameObject CreateAndAddImage(Sprite sprite, Vector2 position, string name = "default")
    {
        GameObject imgObject = new GameObject(name);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(rectMask.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = position; // setting position, will be on center
        trans.sizeDelta = new Vector2(_distanceBetweenSteps, _timelineHeight); // custom size

        Image image = imgObject.AddComponent<Image>();
        image.sprite = sprite;
        imgObject.transform.SetParent(rectMask.transform);

        return imgObject;
    }

    private void OnTick()
    {

        StartCoroutine(MoveNotches());
        /*LinkedListNode<GameObject> firstNotch = _notches.First;


        firstNotch.Value.GetComponent<RectTransform>().anchoredPosition = new Vector2(_distanceBetweenSteps * _timelineExtent, _timelineYOffset);*/
    }

    private void Update()
    {
        if(_bLiveEdit)
        {
            ConstructTimeline();
        }
    }

    private void OnHazardsGenerated()
    {
        ConstructTimeline();
    }

    private void ClearNotches()
    {
        foreach(GameObject notch in _notches)
        {
            Destroy(notch);
        }
        _notches.Clear();
    }

    private void ClearLine()
    {
        foreach (GameObject gO in _lineImgs)
        {
            Destroy(gO);
        }
        _lineImgs.Clear();
    }

    IEnumerator MoveNotches()
    {
        _timeManager.bIsTransitioningToNextTimeStep = true;

        Tween notchTween = null;
        foreach (GameObject _notch in _notches)
        {
            RectTransform rectTransform = _notch.GetComponent<RectTransform>();
            float currentX = rectTransform.anchoredPosition.x;
            notchTween = rectTransform.DOLocalMoveX(currentX - _distanceBetweenSteps, 1f);
        }

        if(notchTween != null)
            yield return notchTween.WaitForCompletion();
        else
            yield return null;

        LinkedListNode<GameObject> firstNotch = _notches.First;
        if(firstNotch != null)
        {
            int timeStep = _timeManager.CurrentTimeStep + _timelineExtent + 1;
            if (_hazardManager.GetHazardsAtTimeStamp(timeStep) != null)
            {
                firstNotch.Value.GetComponent<Image>().sprite = verBigNotch;
            }
            else
            {
                firstNotch.Value.GetComponent<Image>().sprite = verSmallNotch;
            }

            firstNotch.Value.GetComponent<RectTransform>().anchoredPosition = new Vector2(_distanceBetweenSteps * (_timelineExtent + 1), _timelineYOffset);
            _notches.RemoveFirst();
            _notches.AddLast(firstNotch);
        }

        _timeManager.bIsTransitioningToNextTimeStep = false;
    }
}
