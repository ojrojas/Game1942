using Microsoft.Xna.Framework.Input.Touch;

namespace Engine.Mapper.Base
{
    public interface IBaseInputMapper
    {
        IEnumerable<IBaseInputCommand> GetGamepadState(GamePadState gamePadState);
        IEnumerable<IBaseInputCommand> GetKeyboardState(KeyboardState keyboardState);
        IEnumerable<IBaseInputCommand> GetMouseState(MouseState mouseState);
        IEnumerable<IBaseInputCommand> GetTouchState(TouchCollection touchState);
    }
}