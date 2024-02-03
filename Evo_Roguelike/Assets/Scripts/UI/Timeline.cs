using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for visualing the timeline of the game through the use of UI.
/// </summary>
public class Timeline : MonoBehaviour
{
    public RectMask2D rectMask;

    // Sprites for building timeline
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

    /// <summary>
    /// Builds the timeline ui widget
    /// </summary>
    private void ConstructTimeline()
    {
        // Clears line and notch sprites if any, usually for live-edit mode.
        ClearLine();
        ClearNotches();

        // Constructing left edge
        int i = -(_timelineExtent + 1);
        _lineImgs.Add(CreateAndAddImage(horLeftEdge, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "leftEdge"));
        i++;

        // Constructing middle parts based on length
        while(i <= _timelineExtent)
        {
            _lineImgs.Add(CreateAndAddImage(horStraight, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "middle" + (i + _timelineExtent)));
            i++;
        }

        // Constructing right edge
        _lineImgs.Add(CreateAndAddImage(horRightEdge, new Vector2(i * _distanceBetweenSteps, _timelineYOffset), "rightEdge"));
        // Constructing arrow
        _lineImgs.Add(CreateAndAddImage(arrow, new Vector2(0, _arrowYOffset)));

        // Constructing notches. If hazard at timestep use big notch, else small
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

        // Creating the sprite mask around the widget to hide transitions
        rectMask.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((2 * (_timelineExtent + 1) * _distanceBetweenSteps) - 10, _timelineHeight);
    }


    /// <summary>
    /// Creates and adds image object to the canvas
    /// </summary>
    /// <param name="sprite"> Image sprite </param>
    /// <param name="position"> Position to render image </param>
    /// <param name="name"> Gameobject name in hierarchy </param>
    /// <returns> The Image GameObject </returns>
    private GameObject CreateAndAddImage(Sprite sprite, Vector2 position, string name = "default")
    {
        GameObject imgObject = new GameObject(name);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(rectMask.transform);
        trans.localScale = Vector3.one;
        trans.anchoredPosition = position; 
        trans.sizeDelta = new Vector2(_distanceBetweenSteps, _timelineHeight);

        Image image = imgObject.AddComponent<Image>();
        image.sprite = sprite;
        imgObject.transform.SetParent(rectMask.transform);

        return imgObject;
    }

    /// <summary>
    /// On new timestep, plays transition animation
    /// </summary>
    private void OnTick()
    {
        MoveNotches();
    }

    private void Update()
    {
        // Reconstructs timeline every frame for visualization purposes. Should be false in actual game
        if(_bLiveEdit)
        {
            ConstructTimeline();
        }
    }

    /// <summary>
    /// Constructs timeline when hazards get generated
    /// </summary>
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

    /// <summary>
    /// Moves notches back one time step and moves first notch to last position to recycle the same gameObject.
    /// </summary>
    /// <returns></returns>
    private void MoveNotches()
    {
        _timeManager.bIsTransitioningToNextTimeStep = true;
        LTDescr lt = null;
        foreach (GameObject _notch in _notches)
        {
            RectTransform rectTransform = _notch.GetComponent<RectTransform>();
            float currentX = rectTransform.anchoredPosition.x;
            lt = LeanTween.moveLocalX(rectTransform.gameObject, currentX - _distanceBetweenSteps, 1f);
        }

        if(lt != null )
            lt.setOnComplete(OnNotchTweenComplete);
        else
            _timeManager.bIsTransitioningToNextTimeStep = false;

    }

    private void OnNotchTweenComplete()
    {
        LinkedListNode<GameObject> firstNotch = _notches.First;
        if (firstNotch != null)
        {
            // Changes sprite based on if hazard is present at timestep
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
