namespace Engine.State.Base;

/// <summary>
/// Base game state
/// </summary>
public abstract class BaseGameState
{
    private readonly IList<BaseObjectGame> _gameObjects = new List<BaseObjectGame>();
    private ContentManager _contentManager;
    private const string Empty = "Empty";
    protected int _viewPortHeight;
    protected int _viewPortWidth;
    protected SoundManager _soundManager = new SoundManager();
    protected BaseInputManager BaseInputManager { get; set; }

    public event EventHandler<BaseGameState> OnEventStateSwitched;
    public event EventHandler<IBaseGameStateEvent> OnEventNotified;
    
    public abstract void LoadContent();
    public void UnLoadContent() 
    {
        _contentManager.Unload();
    }

    public abstract void HandleInput(GameTime gameTime);
    public abstract void UpdateGameState(GameTime gameTime);
    protected abstract void SetInputManager();


    public void Initialize(ContentManager contentManager, int viewPortWidth, int viewPortHeight)
    {
        _contentManager = contentManager;
        _viewPortHeight = viewPortHeight;
        _viewPortWidth = viewPortWidth;
        SetInputManager();
    }


    protected void StateSwitched(BaseGameState baseGameState)
    {
        OnEventStateSwitched?.Invoke(this, baseGameState);
    }

    protected void OnNotified(IBaseGameStateEvent e)
    {
        OnEventNotified?.Invoke(this, e);
        foreach (var gameObject in _gameObjects)
            gameObject.OnNotify(e);
        _soundManager.OnNotify(e);
    }

    protected void AddObject(BaseObjectGame gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    protected Texture2D LoadTexture(string textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName);
        return texture ?? _contentManager.Load<Texture2D>(Empty);
    }

    protected SoundEffect LoadSoundEffect(string soundName) 
    { 
        var sound = _contentManager.Load<SoundEffect>(soundName);
        return sound ?? _contentManager.Load<SoundEffect>(Empty);
    }

    public void Render(SpriteBatch spriteBatch)
    {
        foreach(var gameObject in _gameObjects.OrderBy(o => o.ZIndex))
            gameObject.Render(spriteBatch);
    }

    public void Update(GameTime gameTime) 
    {
        UpdateGameState(gameTime);
        _soundManager.PlaySoundTrack();
    }
    protected void RemoveGameObject(BaseObjectGame gameObject) 
    {
        _gameObjects.Remove(gameObject);
    }
}
