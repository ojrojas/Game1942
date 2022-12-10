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
    public int Width => _texture.Width;
    /// <summary>
    /// Height object game img
    /// </summary>
    public int Height => _texture.Height;

    /// <summary>
    /// Position Scene (methods setter, getter)
    /// </summary>
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
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
}
