namespace Core.State.GamePlay
{
    public partial class GamePlayState : BaseGameState
    {
        private const string BackgroundTexture = @"images\stage\barren";
        private const string PlayerFighter = @"images\player\fighter";
        private const string BulletTexture = @"images\ammo\bullet";

        private const string MissileTexture = @"images\ammo\missile";
        private const string ExhaustTexture = @"images\particles\cloud";

        private const string ExplosionTexture = @"images\ammo\explosion";
        private const string ChopperTexture = @"images\enemyships\chopper";

        private const int MaxExplosionAge = 600; // 10 seconds at
                                                 // 60 frames per
                                                 // second = 600
                                                 // Emit particles for 1.2 seconds and let them fade out for
                                                 // 10 seconds
        private const int ExplosionActiveLength = 75;
        private const float SCROLLING_SPEED = 1.0f;


        private Texture2D _chopperTexture;
        private Texture2D _explosionTexture;
        private Texture2D _missileTexture;
        private Texture2D _exhaustTexture;
        private Texture2D _bulletTexture;


        private bool _isShootingMissile;
        private bool _isShooting;
        private bool _playerDead;
        
        private TimeSpan _lastMissileShotAt;
        private TimeSpan _lastShotAt;
        
        
        private IList<MissileSprite> _missileList;
        private IList<ExplosionEmitter> _explosionList = new List<ExplosionEmitter>();
        private IList<ChopperSprite> _enemyList = new List<ChopperSprite>();
        private IList<BulletSprite> _bulletList;


        private PlayerSprite _playerSprite;

        private ChopperGenerator _chopperGenerator;

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
            _missileTexture = LoadTexture(MissileTexture);
            _exhaustTexture = LoadTexture(ExhaustTexture);
            _bulletTexture = LoadTexture(BulletTexture);
            _chopperTexture = LoadTexture(ChopperTexture);
            _explosionTexture = LoadTexture(ExplosionTexture);


            _playerSprite = new PlayerSprite(LoadTexture(PlayerFighter));
            
            _bulletList = new List<BulletSprite>();
            AddObject(new TerrainBackground(LoadTexture(BackgroundTexture), SCROLLING_SPEED));

            AddObject(_playerSprite);

            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);


            var track1 = LoadSoundEffect(@"audio\stage\batenkaitos").CreateInstance();
            var track2 = LoadSoundEffect(@"audio\stage\housetwo").CreateInstance();
            var track3 = LoadSoundEffect(@"audio\stage\onabreezyday").CreateInstance();

            _soundManager.SetSoundTrack(new List<SoundEffectInstance>() { track1, track2, track3 });

            _missileList = new List<MissileSprite>();

            var missileSound = LoadSoundEffect(@"audio\ammo\player\missile");
            var bulletSound = LoadSoundEffect(@"audio\ammo\player\shot");
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShoots(), bulletSound);
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShootsMissiles(), missileSound, .4f, -.2f, .0f);

            _chopperGenerator = new ChopperGenerator(_chopperTexture, 4, AddChopper);
            _chopperGenerator.GenerateChoppers();
        }

        private IList<T> CleanObjects<T>(IList<T> objectList) where T : BaseObjectGame
        {
            IList<T> listOfItemsToKeep = new List<T>();
            foreach (T item in objectList)
            {
                var offScreen = item.Position.Y < -50;

                if (offScreen || item.Destroyed)
                {
                    RemoveGameObject(item);
                }
                else
                {
                    listOfItemsToKeep.Add(item);
                }
            }

            return listOfItemsToKeep;
        }

        private void AddChopper(ChopperSprite chopper)
        {
            chopper.OnObjectChanged += ChopperSprite_OnObjectChanged;
            _enemyList.Add(chopper);
            AddObject(chopper);
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

            foreach (var chopper in _enemyList)
                chopper.Update();

            
            UpdateExplosions(gameTime);
            RegulateShootingRate(gameTime);
            DetectCollections();


            _bulletList = CleanObjects(_bulletList);
            _missileList = CleanObjects(_missileList);
            _enemyList = CleanObjects(_enemyList);
        }

        private void DetectCollections()
        {
            var bulletCollisionDetector = new AABBCollisionDetector<BulletSprite, ChopperSprite>(_bulletList);
            var missileCollisionDetector = new AABBCollisionDetector<MissileSprite, ChopperSprite>(_missileList);
            var playerCollisionDetector = new AABBCollisionDetector<ChopperSprite, PlayerSprite>(_enemyList);

            bulletCollisionDetector.DetectCollisions(_enemyList, (bullet, chopper) =>
            {
                var hitEvent = new GamePlayStateEvents.ChopperHitBy(bullet);
                chopper.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                bullet.Destroy();
            });

            missileCollisionDetector.DetectCollisions(_enemyList, (missile, chopper) =>
            {
                var hitEvent = new GamePlayStateEvents.ChopperHitBy(missile);
                chopper.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                missile.Destroy();
            });

            if (!_playerDead)
            {
                playerCollisionDetector.DetectCollisions(_playerSprite, (chopper, player) =>
                {
                    KillPlayer();
                });
            }
        }

        private async void KillPlayer()
        {
            _playerDead = true;

            AddExplosion(_playerSprite.Position);
            RemoveGameObject(_playerSprite);

            await Task.Delay(TimeSpan.FromSeconds(2));
            ResetGame();
        }

        private void ResetGame()
        {
            if (_chopperGenerator != null)
            {
                _chopperGenerator.StopGenerating();
            }

            foreach (var bullet in _bulletList)
            {
                RemoveGameObject(bullet);
            }

            foreach (var missile in _missileList)
            {
                RemoveGameObject(missile);
            }

            foreach (var chopper in _enemyList)
            {
                RemoveGameObject(chopper);
            }

            foreach (var explosion in _explosionList)
            {
                RemoveGameObject(explosion);
            }

            _bulletList = new List<BulletSprite>();
            _missileList = new List<MissileSprite>();
            _explosionList = new List<ExplosionEmitter>();
            _enemyList = new List<ChopperSprite>();

            _chopperGenerator = new ChopperGenerator(_chopperTexture, 4, AddChopper);
            _chopperGenerator.GenerateChoppers();

            AddObject(_playerSprite);

            // position the player in the middle of the screen, at the bottom, leaving a slight gap at the bottom
            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height - 30;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);

            _playerDead = false;
        }


        private void RegulateShootingRate(GameTime gameTime)
        {
            if (_lastShotAt != null && gameTime.TotalGameTime - _lastShotAt > TimeSpan.FromSeconds(.2))
                _isShooting = false;

            if (_lastMissileShotAt != null && gameTime.TotalGameTime - _lastMissileShotAt > TimeSpan.FromSeconds(1))
                _isShootingMissile = false;
        }

        private void ChopperSprite_OnObjectChanged(object sender, IBaseGameStateEvent e)
        {
            var chopper = (ChopperSprite)sender;
            switch (e)
            {
                case GamePlayStateEvents.EnemyLostLife ge:
                    if (ge.CurrentLife <= 0)
                    {
                        AddExplosion(new Vector2(chopper.Position.X - 40, chopper.Position.Y - 40));
                        chopper.Destroy();
                    }
                    break;
            }
        }

        private void AddExplosion(Vector2 position)
        {
            var explosion = new ExplosionEmitter(_explosionTexture, position);
            AddObject(explosion);
            _explosionList.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            foreach (var explosion in _explosionList)
            {
                explosion.Update(gameTime);

                if (explosion.Age > ExplosionActiveLength)
                {
                    explosion.Deactivate();
                }

                if (explosion.Age > MaxExplosionAge)
                {
                    RemoveGameObject(explosion);
                }
            }
        }
    }
}
