﻿namespace Core.Objects
{
    public class MissileSprite : BaseObjectGame, IGameObjectWithDamage
    {
        private const float StartSpeed = 0.5f;
        private const float Acceleration = 0.15f;

        private float _speed = StartSpeed;

        public int Damage => 25;

        // keep track of scaled down texture size
        private int _missileHeight;
        private int _missileWidth;

        // missiles are attached to their own particle emitter
        private ExhaustEmitter _exhaustEmitter;

        public override Vector2 Position
        {
            set
            {
                var emitterOffsetX = 18;
                var emitterOffsetY = -10;

                var emitterPosX = _position.X + emitterOffsetX;
                var emitterPosY = _position.Y + _missileHeight + emitterOffsetY;
                _exhaustEmitter.Position = new Vector2(emitterPosX, emitterPosY);
                base.Position = value;
            }
        }

        public MissileSprite(Texture2D missileTexture, Texture2D exhaustTexture)
        {
            _texture = missileTexture;
            _exhaustEmitter = new ExhaustEmitter(exhaustTexture, _position);

            var ratio = (float)_texture.Height / (float)_texture.Width;
            _missileWidth = 50;
            _missileHeight = (int)(_missileWidth * ratio);

            // note that the missile is scaled down! so it's bounding box must be scaled down as well
            var bbRatio = (float)_missileWidth / _texture.Width;

            var bbOriginalPositionX = 352;
            var bbOriginalPositionY = 7;
            var bbOriginalWidth = 150;
            var bbOriginalHeight = 500;

            var bbPositionX = bbOriginalPositionX * bbRatio;
            var bbPositionY = bbOriginalPositionY * bbRatio;
            var bbWidth = bbOriginalWidth * bbRatio;
            var bbHeight = bbOriginalHeight * bbRatio;

            AddBoundingBox(new Engine.ObjectGame.BoundingBox(new Vector2(bbPositionX, bbPositionY), bbWidth, bbHeight));
        }

        public void Update(GameTime gameTime)
        {
            _exhaustEmitter.Update(gameTime);

            Position = new Vector2(Position.X, Position.Y - _speed);
            _speed = _speed + Acceleration;
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            // need to scale down the sprite. The original texture is very big
            var destRectangle = new Rectangle((int)Position.X, (int)Position.Y, _missileWidth, _missileHeight);
            spriteBatch.Draw(_texture, destRectangle, Color.White);

            _exhaustEmitter.Render(spriteBatch);
        }
    }
}
