using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is UI element that shows the currently queued actions.
/// </summary>
public class QueuedActions : MonoBehaviour
{
    [SerializeField]
    private float _actionImgSpacing = 5f;
    [SerializeField]
    private float _actionImgHeight = 50f;
    [SerializeField]
    private float _actionImgWidth = 50f;

    private ActionManagerBehaviour _actionManagerBehaviour;
    private ActionManager _actionManager;

    private List<GameObject> _queuedImgs = new List<GameObject>();
    private Vector2 _size;

    void Start()
    {
        _actionManagerBehaviour = ServiceLocator.Instance.GetService<ActionManagerBehaviour>();
        _actionManager = _actionManagerBehaviour.ActionManager;
        _actionManager.dQueuedActionsUpdated += OnActionQueued;
        UpdateUI();

        _size = GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// This function is called when the queuedActions gets updated
    /// </summary>
    private void OnActionQueued()
    {
        UpdateUI();
    }

    /// <summary>
    /// Refreshes UI
    /// </summary>
    private void UpdateUI()
    {
        ClearActions();
        List<PlayerAction> playerActions = _actionManager.queuedActions;


        for (int i = 0; i < playerActions.Count; i++)
        {
           ActionData actionData = _actionManager.GetActionDataFromPlayerAction(playerActions[i]);
           Vector2 pos = new Vector2(i * (_actionImgSpacing + _actionImgWidth) - _size.x/2 + _actionImgWidth/2, 0);
           _queuedImgs.Add(CreateAndAddImage(actionData.uiSprite, pos));
        }
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
        trans.transform.SetParent(this.transform);
        trans.localScale = Vector3.one;
        trans.anchoredPosition = position;
        trans.sizeDelta = new Vector2(_actionImgWidth, _actionImgHeight);

        Image image = imgObject.AddComponent<Image>();
        image.sprite = sprite;
        imgObject.transform.SetParent(this.transform);

        return imgObject;
    }

    /// <summary>
    /// Clears UI actions queue.
    /// </summary>
    private void ClearActions()
    {
        foreach (GameObject gO in _queuedImgs)
        {
            Destroy(gO);
        }
        _queuedImgs.Clear();
    }
}
