namespace Core.Objects;
public  class PlayerSprite : BaseObjectGame
{
    private float SPEED = 3.0f;

    public PlayerSprite(Texture2D texture)
    {
        _texture= texture;
    }

    public void MoveLeft()
    {
        Position = new Vector2(Position.X - SPEED, Position.Y);
    }

    public void MoveRight()
    {
        Position = new Vector2(Position.X + SPEED, Position.Y);
    }

    public void MoveUp()
    {
        Position = new Vector2(Position.X , Position.Y - SPEED);
    }

    public void MoveDown()
    {
        Position = new Vector2(Position.X, Position.Y + SPEED);
    }
}
