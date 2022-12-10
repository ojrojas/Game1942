namespace Core.State.GamePlay
{
    public partial class GamePlayState : BaseGameState
    {
        private const string PlayerFighter = "fighter";
        private const string BulletTexture = "bullet";
        private const string BackgroundTexture = "barren";
        private Texture2D _bulletTexture;
        private List<BulletSprite> _bulletList;
        private PlayerSprite _playerSprite;
        private bool _isShooting;
        private TimeSpan _lastShotAt;
        private const float SCROLLING_SPEED = 1.0f;

        public override void HandleInput(GameTime gameTime)
        {
            this.HandlerInputGamePlay(gameTime);
        }

        private void Shoot(GameTime gameTime, TypeShootEnum typeShoot)
        {
            if (!_isShooting)
            {
                CreateBullets();
                _isShooting = true;
                _lastShotAt = gameTime.TotalGameTime;
            }
        }

        private void CreateBullets()
        {
            var bulletSpriteLeft = new BulletSprite(_bulletTexture);
            var bulletSpriteRight = new BulletSprite(_bulletTexture);
            var bulletY = _playerSprite.Position.Y + 30;
            var bulletLeftX = _playerSprite.Position.X + _playerSprite.Width / 2 - 40;
            var bulletRightX = _playerSprite.Position.X + _playerSprite.Width / 2 + 10;

            bulletSpriteLeft.Position = new Vector2(bulletLeftX, bulletY);
            bulletSpriteRight.Position = new Vector2(bulletRightX, bulletY);

            _bulletList.Add(bulletSpriteLeft);
            _bulletList.Add(bulletSpriteRight);

            AddObject(bulletSpriteLeft);
            AddObject(bulletSpriteRight);

        }

        public override void LoadContent()
        {
            _playerSprite = new PlayerSprite(LoadTexture(@"player\" + PlayerFighter));
            _bulletTexture = LoadTexture(@"ammo\" + BulletTexture);
            _bulletList = new List<BulletSprite>();
            var background = new TerrainBackground(LoadTexture(@"stage\" + BackgroundTexture), SCROLLING_SPEED);
            background.ZIndex = -100;
            AddObject(background);

            AddObject(_playerSprite);

            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);
        }

        protected override void SetInputManager()
        {
            BaseInputManager = new BaseInputManager(new GamePlayInputMapper());
        }

        private void KeepPlayerInBounds()
        {
            if (_playerSprite.Position.X < 0)
            {
                _playerSprite.Position = new Vector2(0, _playerSprite.Position.Y);
            }
            if (_playerSprite.Position.X > _viewPortWidth - _playerSprite.Width)
            {
                _playerSprite.Position = new Vector2(_viewPortWidth - _playerSprite.Width, _playerSprite.Position.Y);
            }
            if (_playerSprite.Position.Y < 0)
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, 0);
            }
            if (_playerSprite.Position.Y > _viewPortHeight - _playerSprite.Height)
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, _viewPortHeight - _playerSprite.Height);
            }
        }

        public override void UpdateGameState(GameTime gameTime)
        {
            foreach (var bullet in _bulletList)
            {
                bullet.MoveUp();
            }

            if (_lastShotAt != null && gameTime.TotalGameTime - _lastShotAt > TimeSpan.FromSeconds(.2))
            {
                _isShooting = false;
            }

            var newBulletList = new List<BulletSprite>();
            foreach (var bullet in _bulletList)
            {
                var bulletStillOnScreen = bullet.Position.Y > -30;

                if (bulletStillOnScreen)
                {
                    newBulletList.Add(bullet);
                }
                else
                {
                    RemoveGameObject(bullet);
                }
            }
            _bulletList = newBulletList;
        }
    }
}
