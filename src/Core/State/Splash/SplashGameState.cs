namespace Core.State.Splash;
public class SplashGameState : BaseGameState
{
    private const string TEXTURENAME = "menus/splashgame";

    public override void HandleInput(GameTime gameTime)
    {
        var gamePadState = GamePad.GetState(PlayerIndex.One | PlayerIndex.Two);
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Enter) || gamePadState.Buttons.Start == ButtonState.Pressed)
        {
            this.StateSwitched(new GamePlayState());
        }
        if (keyboardState.IsKeyDown(Keys.Escape) || gamePadState.Buttons.Back == ButtonState.Pressed)
        {
            OnNotified(new BaseGameStateEvent.GameQuit());
        }
    }

    protected override void SetInputManager()
    {
        BaseInputManager = new BaseInputManager(new GamePlayInputMapper());
    }

    public override void LoadContent()
    {
        AddObject(new SplashImage(LoadTexture(TEXTURENAME)));
    }

    public override void UpdateGameState(GameTime gameTime)
    {
        
    }
}
