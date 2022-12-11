using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Core.State.GamePlay
{
    public partial class GamePlayState : BaseGameState
    {
        private const string BackgroundTexture = @"images\stage\barren";
        private const string PlayerFighter = @"images\player\fighter";
        private const string BulletTexture = @"images\ammo\bullet";

        private const string MissileTexture = @"images\ammo\missile";
        private const string ExhaustTexture = @"images\particles\cloud";

        private Texture2D _exhaustTexture;

        private Texture2D _missileTexture;
        private bool _isShootingMissile;
        private TimeSpan _lastMissileShotAt;
        private IList<MissileSprite> _missileList;


        private Texture2D _bulletTexture;
        private IList<BulletSprite> _bulletList;
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
            if (!_isShooting && typeShoot is TypeShootEnum.Bullets)
            {
                CreateBullets();
                _isShooting = true;
                _lastShotAt = gameTime.TotalGameTime;

                OnNotified(new GamePlayStateEvents.PlayerShoots());

            }
            if (!_isShootingMissile && typeShoot is TypeShootEnum.Missile)
            {
                CreateMissile();
                _isShootingMissile = true;
                _lastMissileShotAt = gameTime.TotalGameTime;

                OnNotified(new GamePlayStateEvents.PlayerShootsMissiles());
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

        private void CreateMissile()
        {
            var missileSprite = new MissileSprite(_missileTexture, _exhaustTexture);
            missileSprite.Position = new Vector2(_playerSprite.Position.X + 33, _playerSprite.Position.Y - 25);

            _missileList.Add(missileSprite);
            AddObject(missileSprite);
        }

        public override void LoadContent()
        {
            _playerSprite = new PlayerSprite(LoadTexture(PlayerFighter));
            _bulletTexture = LoadTexture(BulletTexture);
            _bulletList = new List<BulletSprite>();
            var background = new TerrainBackground(LoadTexture(BackgroundTexture), SCROLLING_SPEED);
            background.ZIndex = -100;
            AddObject(background);

            AddObject(_playerSprite);

            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);


            var track1 = LoadSoundEffect(@"audio\stage\batenkaitos").CreateInstance();
            var track2 = LoadSoundEffect(@"audio\stage\housetwo").CreateInstance();
            var track3 = LoadSoundEffect(@"audio\stage\onabreezyday").CreateInstance();

            _soundManager.SetSoundTrack(new List<SoundEffectInstance>() { track1, track2, track3 });

            _missileTexture = LoadTexture(MissileTexture);
            _exhaustTexture = LoadTexture(ExhaustTexture);
            _missileList = new List<MissileSprite>();

            var missileSound = LoadSoundEffect(@"audio\ammo\player\missile");
            var bulletSound = LoadSoundEffect(@"audio\ammo\player\shot");
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShoots(), bulletSound);
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShootsMissiles(), missileSound, .4f, -.2f, .0f);
        }

        private IList<T> CleanObjects<T>(IList<T> objectList) where T : BaseObjectGame
        {
            List<T> listOfItemsToKeep = new List<T>();
            foreach (T item in objectList)
            {
                var stillOnScreen = item.Position.Y > -50;
                if (stillOnScreen) listOfItemsToKeep.Add(item);
                else RemoveGameObject(item);
            }

            return listOfItemsToKeep;
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
                bullet.MoveUp();

            foreach (var missile in _missileList)
                missile.Update(gameTime);

            RegulateShootingRate(gameTime);


            _bulletList = CleanObjects(_bulletList);
            _missileList = CleanObjects(_missileList);
        }

        private void RegulateShootingRate(GameTime gameTime)
        {
            if (_lastShotAt != null && gameTime.TotalGameTime - _lastShotAt > TimeSpan.FromSeconds(.2))
                _isShooting = false;

            if (_lastMissileShotAt != null && gameTime.TotalGameTime - _lastMissileShotAt > TimeSpan.FromSeconds(1))
                _isShootingMissile = false;
        }
    }
}
