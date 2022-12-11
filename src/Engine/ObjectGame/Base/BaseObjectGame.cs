namespace Engine.ObjectGame.Base;
/// <summary>
/// Base Object Game
/// </summary>
public class BaseObjectGame : IAggregateRoot
{
    /// <summary>
    /// Texture object (img)
    /// </summary>
    protected Texture2D _texture;

    /// <summary>
    /// Bounding texture for collisions
    /// </summary>
    protected Texture2D _boundingBoxTexture;

    /// <summary>
    /// zIndex position layer
    /// </summary>
    public int ZIndex { get; set; }
    /// <summary>
    /// Position Scene
    /// </summary>
    protected Vector2 _position = Vector2.One;

    /// <summary>
    /// Width object game img
    /// </summary>
    public virtual int Width => _texture.Width;
    /// <summary>
    /// Height object game img
    /// </summary>
    public virtual int Height => _texture.Height;

    public event EventHandler<IBaseGameStateEvent> OnObjectChanged;

    protected float _angle;

    protected Vector2 _direction;

    protected IList<BoundingBox> _boundingBoxes = new List<BoundingBox>();

    public bool Destroyed { get; private set; }

    /// <summary>
    /// Position Scene (methods setter, getter)
    /// </summary>
    public virtual Vector2 Position
    {
        get => _position;
        set
        {
            var deltaX = value.X - _position.X;
            var deltaY = value.Y - _position.Y;
            _position = value;

            foreach (var bb in _boundingBoxes)
            {
                bb.Position = new Vector2(bb.Position.X + deltaX, bb.Position.Y + deltaY);
            }
        }
    }

    public IList<BoundingBox> BoundingBoxes
    {
        get
        {
            return _boundingBoxes;
        }
    }

    /// <summary>
    /// Render object in scene
    /// </summary>
    /// <param name="spriteBatch">Instance spriteBatch</param>
    public virtual void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, Color.White);
    }

    /// <summary>
    /// Notify event of game
    /// </summary>
    /// <param name="eventGame">Enum event game</param>
    public virtual void OnNotify(IBaseGameStateEvent eventGame) { }

    public void SendEvent(IBaseGameStateEvent eventGame)
    {
        OnObjectChanged?.Invoke(this, eventGame);
    }

    public void RenderBoundingBoxes(SpriteBatch spriteBatch)
    {
        if (_boundingBoxTexture == null)
        {
            CreateBoundingBoxTexture(spriteBatch.GraphicsDevice);
        }

        foreach (var bb in _boundingBoxes)
        {
            spriteBatch.Draw(_boundingBoxTexture, bb.Rectangle, new Color(Color.Red, .2f));
        }
    }

    public void Destroy()
    {
        Destroyed = true;
    }

    public void AddBoundingBox(BoundingBox bb)
    {
        _boundingBoxes.Add(bb);
    }

    private void CreateBoundingBoxTexture(GraphicsDevice graphicsDevice)
    {
        _boundingBoxTexture = new Texture2D(graphicsDevice, 1, 1);
        _boundingBoxTexture.SetData(new Color[] { Color.White });
    }

    protected Vector2 CalculateDirection(float angleOffset = 0.0f)
    {
        _direction = new Vector2((float)Math.Cos(_angle - angleOffset), (float)Math.Sin(_angle - angleOffset));
        _direction.Normalize();

        return _direction;
    }
}
