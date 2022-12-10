namespace Engine.Mapper.Base
{
    public interface IBaseInputMapper
    {
        IEnumerable<IBaseInputCommand> GetGamepadState(GamePadState gamePadState);
        IEnumerable<IBaseInputCommand> GetKeyboardState(KeyboardState keyboardState);
        IEnumerable<IBaseInputCommand> GetMouseState(MouseState mouseState);
    }
}