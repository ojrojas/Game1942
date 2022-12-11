namespace Engine.ObjectGame.Base
{
    public class BaseTextObject : BaseObjectGame
    {
        protected SpriteFont _font;

        public string Text { get; set; }

        public override void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, Text, _position, Color.White);
        }
    }
}
