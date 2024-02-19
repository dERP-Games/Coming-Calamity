public abstract class CursorState
{
    public enum StateTypes
    {
        Select,
        Target
    }

    protected abstract void HandleClick();
    protected abstract void HandleHover();

    public void OnClick()
    {
        HandleClick();
    }

    public void OnHover()
    {
        HandleHover();
    }
}

public class SelectCursor : CursorState
{
    protected override void HandleClick()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleHover()
    {
        throw new System.NotImplementedException();
    }
}

public class TargetCursor : CursorState
{
    protected override void HandleClick()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleHover()
    {
        throw new System.NotImplementedException();
    }
}

public static class CursorStateFactory
{
    public static CursorState MakeState(CursorState.StateTypes type)
    {
        switch (type)
        {
            case CursorState.StateTypes.Target:
                return new TargetCursor();
            default:
                return new SelectCursor();
        }
    }
}

public class Cursor
{
    CursorState currentState;

    public void Click()
    {
        currentState.OnClick();
    }

    public void Hover()
    {
        currentState.OnHover();
    }

    public void SetState(CursorState.StateTypes type)
    {
        currentState = CursorStateFactory.MakeState(type);
    }
}