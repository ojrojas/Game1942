namespace Engine.Inputs.Base;

public class BaseInputManager
{
    private readonly IBaseInputMapper _baseInputMapper;

    public BaseInputManager(IBaseInputMapper baseInputMapper)
    {
        _baseInputMapper = baseInputMapper;
    }

    public void GetCommand(Action<IBaseInputCommand> actionOnCommands)
    {
        var keyboardState = Keyboard.GetState();
        foreach (var state in _baseInputMapper.GetKeyboardState(keyboardState))
            actionOnCommands(state);

        var mouseState = Mouse.GetState();
        foreach (var state in _baseInputMapper.GetMouseState(mouseState))
            actionOnCommands(state);

        var gamePadState = GamePad.GetState(PlayerIndex.One | PlayerIndex.Two);
        foreach (var state in _baseInputMapper.GetGamepadState(gamePadState))
            actionOnCommands(state);
    }
}

