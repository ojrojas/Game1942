using Microsoft.Xna.Framework.Input.Touch;

namespace Core.State.GamePlay;

public class GamePlayInputMapper : IBaseInputMapper
{
    public IEnumerable<IBaseInputCommand> GetGamepadState(GamePadState gamePadState)
    {
        var commands = new List<IBaseInputCommand>();

        //Command game 
        if (gamePadState.Buttons.Back == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.GameMenu());
        if (gamePadState.Buttons.Start == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.GamePause());

        // command movemment
        if (gamePadState.DPad.Up == ButtonState.Pressed || gamePadState.ThumbSticks.Left.Y > 0)
            commands.Add(new GamePlayInputCommand.PlayerMoveUp());
        if (gamePadState.DPad.Down == ButtonState.Pressed || gamePadState.ThumbSticks.Left.Y < 0)
            commands.Add(new GamePlayInputCommand.PlayerMoveDown());
        if (gamePadState.DPad.Right == ButtonState.Pressed || gamePadState.ThumbSticks.Left.X > 0)
            commands.Add(new GamePlayInputCommand.PlayerMoveRight());
        if (gamePadState.DPad.Left == ButtonState.Pressed || gamePadState.ThumbSticks.Left.X < 0)
            commands.Add(new GamePlayInputCommand.PlayerMoveLeft());

        // command action
        if (gamePadState.Buttons.Y == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionY());
        if (gamePadState.Buttons.B == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionB());
        if (gamePadState.Buttons.A == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionA());
        if (gamePadState.Buttons.X == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionX());

        if (gamePadState.Buttons.LeftStick == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionL1());
        if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionL2());

        if (gamePadState.Buttons.RightStick == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionR1());
        if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            commands.Add(new GamePlayInputCommand.PlayerActionR2());

        return commands;
    }

    public IEnumerable<IBaseInputCommand> GetKeyboardState(KeyboardState keyboardState)
    {
        var commands = new List<IBaseInputCommand>();

        //Commnand game
        if (keyboardState.IsKeyDown(Keys.Escape))
            commands.Add(new GamePlayInputCommand.GameMenu());
        if (keyboardState.IsKeyDown(Keys.Enter))
            commands.Add(new GamePlayInputCommand.GamePause());

        //command movemment
        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            commands.Add(new GamePlayInputCommand.PlayerMoveUp());
        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            commands.Add(new GamePlayInputCommand.PlayerMoveDown());
        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            commands.Add(new GamePlayInputCommand.PlayerMoveRight());
        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            commands.Add(new GamePlayInputCommand.PlayerMoveLeft());

        // command action
        if (keyboardState.IsKeyDown(Keys.J) || keyboardState.IsKeyDown(Keys.NumPad4))
            commands.Add(new GamePlayInputCommand.PlayerActionY());
        if (keyboardState.IsKeyDown(Keys.K) || keyboardState.IsKeyDown(Keys.NumPad5))
            commands.Add(new GamePlayInputCommand.PlayerActionB());
        if (keyboardState.IsKeyDown(Keys.L) || keyboardState.IsKeyDown(Keys.NumPad6))
            commands.Add(new GamePlayInputCommand.PlayerActionA());
        if (keyboardState.IsKeyDown(Keys.O) || keyboardState.IsKeyDown(Keys.NumPad9))
            commands.Add(new GamePlayInputCommand.PlayerActionA());

        return commands;
    }

    public IEnumerable<IBaseInputCommand> GetMouseState(MouseState mouseState)
    {
        var commands = new List<IBaseInputCommand>();
        return commands;
    }

    public IEnumerable<IBaseInputCommand> GetTouchState(TouchCollection touchState)
    {
        var commands = new List<IBaseInputCommand>();
        return commands;
    }
}
