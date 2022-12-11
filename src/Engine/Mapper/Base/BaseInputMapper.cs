using Microsoft.Xna.Framework.Input.Touch;

namespace Engine.Mapper.Base;
public class BaseInputMapper : IBaseInputMapper
{
    public virtual IEnumerable<IBaseInputCommand> GetKeyboardState(KeyboardState keyboardState)
    {
        return new List<IBaseInputCommand>();
    }

    public virtual IEnumerable<IBaseInputCommand> GetGamepadState(GamePadState gamePadState)
    {
        return new List<IBaseInputCommand>();
    }

    public virtual IEnumerable<IBaseInputCommand> GetMouseState(MouseState mouseState)
    {
        return new List<IBaseInputCommand>();
    }

    public IEnumerable<IBaseInputCommand> GetTouchState(TouchCollection touchState)
    {
        return new List<IBaseInputCommand>();
    }
}
