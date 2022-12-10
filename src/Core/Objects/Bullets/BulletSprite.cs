namespace Core.Objects
{
    public class BulletSprite: BaseObjectGame
    {
        private float SPEED = 10.0f;

        public BulletSprite(Texture2D texture) 
        {
            _texture= texture;
        }

        public void MoveUp()
        {
            Position = new Vector2(Position.X, Position.Y - SPEED);
        }
    }
}
