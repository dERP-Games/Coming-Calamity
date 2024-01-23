using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Timeline : MonoBehaviour
{
    public Canvas canvas;
    public Sprite horLeftEdge;
    public Sprite horStraight;
    public Sprite horRightEdge;
    public Sprite verBigNotch;
    public Sprite verSmallNotch;
    public Sprite arrow;

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

    Dictionary<int, List<HazardCommand>> _hazardsToExecute;
    LinkedList<GameObject> _notches = new LinkedList<GameObject>();
    List<GameObject> _lineImgs = new List<GameObject>();
    private TimeManager _timeManager;
    private int _currentHazard = 0;
    

    private void Start()
    {
        _hazardsToExecute = ServiceLocator.Instance.GetService<HazardManagerBehaviour>().HazardManager.hazardsToExectute;
        _timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
        _timeManager.D_tick += OnTick;
        ConstructTimeline();
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
            if(j%2 == 0)
            {
                _notches.AddLast(CreateAndAddImage(verBigNotch, new Vector2(j * _distanceBetweenSteps, _timelineYOffset), "notch" + (j + _timelineExtent)));
            }
            else
            {
                _notches.AddLast(CreateAndAddImage(verSmallNotch, new Vector2(j * _distanceBetweenSteps, _timelineYOffset), "notch" + (j + _timelineExtent)));
            }
        }
    }

    private GameObject CreateAndAddImage(Sprite sprite, Vector2 position, string name = "default")
    {
        GameObject imgObject = new GameObject(name);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(this.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = position; // setting position, will be on center
        trans.sizeDelta = new Vector2(_distanceBetweenSteps, 200); // custom size

        Image image = imgObject.AddComponent<Image>();
        image.sprite = sprite;
        imgObject.transform.SetParent(this.transform);

        return imgObject;
    }

    private void OnTick()
    {
        foreach(GameObject _notch in _notches)
        {
            RectTransform rectTransform = _notch.GetComponent<RectTransform>();
            float currentX = rectTransform.anchoredPosition.x;
            rectTransform.DOLocalMoveX(currentX - _distanceBetweenSteps, 1f);
        }

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
}
