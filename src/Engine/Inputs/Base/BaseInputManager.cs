using Microsoft.Xna.Framework.Input.Touch;

namespace Engine.Inputs.Base;

/// <summary>
/// BaseInputManager
/// </summary>
public class BaseInputManager
{
    /// <summary>
    /// BaseInputMapper instance
    /// </summary>
    private readonly IBaseInputMapper _baseInputMapper;

    /// <summary>
    /// Constructor create new instance base input manager
    /// </summary>
    /// <param name="baseInputMapper">BaseInputMapper instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BaseInputManager(IBaseInputMapper baseInputMapper)
    {
        _baseInputMapper = baseInputMapper ?? throw new ArgumentNullException(nameof(baseInputMapper));
    }

    /// <summary>
    /// GetCommand actions of game keyboards, gamepad, mouse
    /// </summary>
    /// <param name="actionOnCommands">Actions commands inputs controllers, keyboards, mouse, touch</param>
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

        var touchState = TouchPanel.GetState();
        foreach (var state in _baseInputMapper.GetTouchState(touchState));
    }
}

