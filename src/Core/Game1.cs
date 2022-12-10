namespace Core;

public class Game1942 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;
    private Rectangle _renderScaleRectangle;
    private int _designedResolutionWidth = 480;
    private int _designedResolutionHight = 640;
    private float _designedResolutionAspectRatio;

    private BaseGameState _currentGameState;
    private BaseGameState _firstBaseGame;

    public Game1942(int width, int height, BaseGameState firstBaseGame)
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        _firstBaseGame = firstBaseGame;
        _designedResolutionWidth = width;
        _designedResolutionHight = height;
        _designedResolutionAspectRatio = width / (float)height;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _designedResolutionWidth;
        _graphics.PreferredBackBufferHeight = _designedResolutionHight;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.ApplyChanges();

        _renderTarget = new RenderTarget2D(
            _graphics.GraphicsDevice,
            _designedResolutionWidth,
            _designedResolutionHight,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.DiscardContents);

        _renderScaleRectangle = GetScaleRectangle();
        base.Initialize();
    }

    private Rectangle GetScaleRectangle()
    {
        var variance = 0.5;
        var actualAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;

        Rectangle scaleRectangle;

        if (actualAspectRatio <= _designedResolutionAspectRatio)
        {
            var presentHeight = (int)(Window.ClientBounds.Width / _designedResolutionAspectRatio + variance);
            var barHeight = (Window.ClientBounds.Height - presentHeight) / 2;

            scaleRectangle = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
        }
        else
        {
            var presentWidth = (int)(Window.ClientBounds.Height * _designedResolutionAspectRatio + variance);
            var barWidth = (Window.ClientBounds.Width - presentWidth) / 2;

            scaleRectangle = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
        }

        return scaleRectangle;
    }

    private void SwitchGameState(BaseGameState gameState)
    {
        if (_currentGameState is not null)
        {
            _currentGameState.OnEventStateSwitched -= CurrentGameState_OnStateSwitched;
            _currentGameState.OnEventNotified -= CurrentGameState_OnEventNotification;
            _currentGameState.UnLoadContent();
        }

        _currentGameState = gameState;
        _currentGameState.Initialize(Content, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);
        _currentGameState.LoadContent();

        _currentGameState.OnEventStateSwitched += CurrentGameState_OnStateSwitched;
        _currentGameState.OnEventNotified += CurrentGameState_OnEventNotification;
    }

    private void CurrentGameState_OnEventNotification(object sender, IBaseGameStateEvent e)
    {
        switch (e)
        {
            case BaseGameStateEvent.GameQuit _:
                Exit();
                break;
            case BaseGameStateEvent.Pause _:
                SwitchGameState(new SplashGameState());
                break;
        }
    }

    private void CurrentGameState_OnStateSwitched(object sender, BaseGameState e)
    {
        SwitchGameState(e);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SwitchGameState(_firstBaseGame);
    }

    protected override void UnloadContent()
    {
        _currentGameState?.UnLoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _currentGameState.HandleInput(gameTime);
        _currentGameState.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // first run splash screen -- after currentGameState inject
        _spriteBatch.Begin();
        _currentGameState.Render(_spriteBatch);
        _spriteBatch.End();

        // Now Render the scaled content
        _graphics.GraphicsDevice.SetRenderTarget(null);
        _graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Gray, 1.0f, 0);
        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

        _spriteBatch.Draw(_renderTarget, _renderScaleRectangle, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
