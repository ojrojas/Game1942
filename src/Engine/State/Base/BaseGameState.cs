namespace Engine.State.Base;

/// <summary>
/// Base game state
/// </summary>
public abstract class BaseGameState
{
    protected bool _debug = false;
    /// <summary>
    /// List game objects stages
    /// </summary>
    private readonly IList<BaseObjectGame> _gameObjects = new List<BaseObjectGame>();
    /// <summary>
    /// Instance content Manager
    /// </summary>
    private ContentManager _contentManager;
    /// <summary>
    /// Size view height from MainGame
    /// </summary>
    protected int _viewPortHeight;
    /// <summary>
    /// Size view width from MainGame
    /// </summary>
    protected int _viewPortWidth;

    /// <summary>
    /// Base input manager from 
    /// </summary>
    protected BaseInputManager BaseInputManager { get; set; }
    /// <summary>
    /// SoundManager instance from engine
    /// </summary>
    protected SoundManager _soundManager = new SoundManager();

    /// <summary>
    /// Event state switch between stages or phases
    /// </summary>
    public event EventHandler<BaseGameState> OnEventStateSwitched;
    /// <summary>
    /// Notify events wich sound or actions into the game
    /// </summary>
    public event EventHandler<IBaseGameStateEvent> OnEventNotified;
    
    /// <summary>
    /// Empty for load content do not crash, when load assets
    /// </summary>
    private const string Empty = "Empty";

    /// <summary>
    /// LoadContent implementation load assets, images, sprites, etc
    /// </summary>
    public abstract void LoadContent();

    /// <summary>
    /// Confirm disposed this instance BaseGameState
    /// </summary>
    protected bool _disposed;

    /// <summary>
    /// UnLoadContent implementation unload assets etc
    /// </summary>
    public void UnLoadContent() 
    {
        _contentManager.Unload();
    }

    /// <summary>
    /// Handle inputs of games invoke inputmanager and commands
    /// </summary>
    /// <param name="gameTime">Current game time</param>
    public abstract void HandleInput(GameTime gameTime);

    /// <summary>
    /// Update all scenarios of stages, sounds, sprites, positions, etc
    /// </summary>
    /// <param name="gameTime"></param>
    public abstract void UpdateGameState(GameTime gameTime);

    /// <summary>
    /// Setinput manager invoke instance BaseInputManager
    /// </summary>
    protected abstract void SetInputManager();

    /// <summary>
    /// Initialize state switched into game
    /// </summary>
    /// <param name="contentManager">ContentManager -> MainGame</param>
    /// <param name="viewPortWidth">Viewport -> MainGame width</param>
    /// <param name="viewPortHeight">Viewport -> MainGame height</param>
    public void Initialize(ContentManager contentManager, int viewPortWidth, int viewPortHeight)
    {
        _contentManager = contentManager;
        _viewPortHeight = viewPortHeight;
        _viewPortWidth = viewPortWidth;
        SetInputManager();
    }

    /// <summary>
    /// Publish event change or switched state machine
    /// </summary>
    /// <param name="baseGameState">State machine to switched</param>
    protected void StateSwitched(BaseGameState baseGameState)
    {
        OnEventStateSwitched?.Invoke(this, baseGameState);
    }

    /// <summary>
    /// Publish event state currently for actions invoke
    /// </summary>
    /// <param name="e">Event publish</param>
    protected void OnNotified(IBaseGameStateEvent e)
    {
        OnEventNotified?.Invoke(this, e);
        foreach (var gameObject in _gameObjects)
            gameObject.OnNotify(e);
        _soundManager.OnNotify(e);
    }

    /// <summary>
    /// Add object into state machine
    /// </summary>
    /// <param name="gameObject">Object game</param>
    protected void AddObject(BaseObjectGame gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    /// <summary>
    /// Load textures images or textures into state
    /// </summary>
    /// <param name="textureName">Texture name from content manager</param>
    /// <returns>Texture2D instance</returns>
    protected Texture2D LoadTexture(string textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName);
        return texture ?? _contentManager.Load<Texture2D>(Empty);
    }

    /// <summary>
    /// Load sound into game from content manager 
    /// </summary>
    /// <param name="soundName">Sound name</param>
    /// <returns>SoundEffect instance</returns>
    protected SoundEffect LoadSoundEffect(string soundName) 
    { 
        var sound = _contentManager.Load<SoundEffect>(soundName);
        return sound ?? _contentManager.Load<SoundEffect>(Empty);
    }

    /// <summary>
    /// Render the state currently in action
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch from MainGame</param>
    public void Render(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in _gameObjects.OrderBy(a => a.ZIndex))
        {
            if (_debug)
            {
                gameObject.RenderBoundingBoxes(spriteBatch);
            }

            gameObject.Render(spriteBatch);
        }
    }

    /// <summary>
    /// Update all, invoke UpdateGameState
    /// </summary>
    /// <param name="gameTime">Game time</param>
    public void Update(GameTime gameTime) 
    {
        UpdateGameState(gameTime);
        _soundManager.PlaySoundTrack();
    }

    /// <summary>
    /// Remove game object of the state machine
    /// </summary>
    /// <param name="gameObject"></param>
    protected void RemoveGameObject(BaseObjectGame gameObject) 
    {
        _gameObjects.Remove(gameObject);
    }
}
