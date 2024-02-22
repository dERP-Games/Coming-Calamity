using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a UI element that populated the action buttons.
/// </summary>
public class ActionBar : MonoBehaviour
{
    [SerializeField]
    private GameObject _actionButton;
    [SerializeField]
    private float _actionButtonSpacing = 10f;
    private ActionManagerBehaviour _actionManagerBehviour;
    private ActionManager _actionManager;

    // Start is called before the first frame update
    void Start()
    {
        _actionManagerBehviour = ServiceLocator.Instance.GetService<ActionManagerBehaviour>();
        _actionManager = _actionManagerBehviour.ActionManager;

        CreateActionBar();
    }

    /// <summary>
    /// Populates action buttons based on available actions.
    /// </summary>
    private void CreateActionBar()
    {
        List<ActionData> actions = _actionManagerBehviour.playerActions;

        for (int i = 0; i < actions.Count; i++)
        {
            // Setting position
            GameObject actionButtonInstance = Instantiate(_actionButton, transform);
            RectTransform rectTransform = actionButtonInstance.GetComponent<RectTransform>();
            if(rectTransform != null )
            {
                rectTransform.transform.localPosition +=  new Vector3(i * (rectTransform.sizeDelta.x + _actionButtonSpacing), 0, 0);
            }

            // Hooking up button functionality and sprite
            Button button = actionButtonInstance.GetComponent<Button>();
            if(button != null )
            {
                button.image.sprite = actions[i].uiSprite;
                //PlayerAction playerAction = ActionFactory.CreatePlayerAction(actions[i].actionType);
                ActionManager.EPlayerAction actionType = actions[i].actionType;
                button.onClick.AddListener(() => OnActionButtonClicked(actionType));
            }
        }
    }

    /// <summary>
    /// This function is called when an action button is clicked.
    /// Creates new player action object.
    /// </summary>
    /// <param name="actionType">Which action button was clicked</param>
    private void OnActionButtonClicked(ActionManager.EPlayerAction actionType)
    {
        PlayerAction playerAction = ActionFactory.CreatePlayerAction(actionType);
        playerAction.OnActionSelect();
    }
}
