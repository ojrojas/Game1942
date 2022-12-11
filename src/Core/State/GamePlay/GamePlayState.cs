namespace Core.State.GamePlay
{
    public partial class GamePlayState : BaseGameState
    {
        private const string BackgroundTexture = @"images\stage\barren";
        private const string PlayerFighter = @"images\player\fighterspritesheet";
        private const string BulletTexture = @"images\ammo\bullet";

        private const string MissileTexture = @"images\ammo\missile";
        private const string ExhaustTexture = @"images\particles\cloud";

        private const string ExplosionTexture = @"images\ammo\explosion";
        private const string ChopperTexture = @"images\enemyships\chopper";

        private const string TurretTexture = @"images\enemyships\tower";
        private const string TurretMG2Texture = @"images\enemyships\mg2";
        private const string TurretBulletTexture = @"images\ammo\bulletMG";

        private const string TextFont = @"fonts\lives";
        private const string GameOverFont= @"fonts\gameover";



        private const int StartingPlayerLives = 3;
        private int _playerLives = StartingPlayerLives;

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
        private Texture2D _screenBoxTexture;


        private bool _isShootingMissile;
        private bool _isShooting;
        private bool _playerDead;

        private bool _gameOver = false;

        private TimeSpan _lastMissileShotAt;
        private TimeSpan _lastShotAt;
        
        
        private IList<MissileSprite> _missileList = new List<MissileSprite>();
        private IList<BulletSprite> _bulletList = new List<BulletSprite>();
        private IList<ExplosionEmitter> _explosionList = new List<ExplosionEmitter>();
        private IList<ChopperSprite> _enemyList = new List<ChopperSprite>();
        private IList<TurretBulletSprite> _turretBulletList = new List<TurretBulletSprite>();
        private IList<TurretSprite> _turretList = new List<TurretSprite>();

        private LivesText _livesText;
        private GameOverText _levelStartEndText;
        private PlayerSprite _playerSprite;

        private ChopperGenerator _chopperGenerator;

        private Level _level;

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

                OnNotified(new GamePlayStateEvents.PlayerShootsBullets());

            }
            if (!_isShootingMissile && typeShoot is TypeShootEnum.Missile)
            {
                CreateMissile();
                _isShootingMissile = true;
                _lastMissileShotAt = gameTime.TotalGameTime;

                OnNotified(new GamePlayStateEvents.PlayerShootsMissile());
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
            _livesText = new LivesText(LoadFont(TextFont));
            _livesText.NbLives = StartingPlayerLives;
            _livesText.Position = new Vector2(10.0f, 690.0f);

            var background = new TerrainBackground(LoadTexture(BackgroundTexture), SCROLLING_SPEED);
            background.ZIndex= -100;
            AddObject(background);

            AddObject(_playerSprite);

            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);


            _levelStartEndText = new GameOverText(LoadFont(GameOverFont));

            var track3 = LoadSoundEffect(@"audio\stage\batenkaitos").CreateInstance();
            var track2 = LoadSoundEffect(@"audio\stage\housetwo").CreateInstance();
            var track1 = LoadSoundEffect(@"audio\stage\onabreezyday").CreateInstance();

            _soundManager.SetSoundTrack(new List<SoundEffectInstance>() { track1, track2, track3 });
            AddObject(_livesText);

            _missileList = new List<MissileSprite>();

            var missileSound = LoadSoundEffect(@"audio\ammo\player\missile");
            var bulletSound = LoadSoundEffect(@"audio\ammo\player\shot");
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShootsBullets(), bulletSound);
            _soundManager.RegisterSound(new GamePlayStateEvents.PlayerShootsMissile(), missileSound, .4f, -.2f, .0f);

            _chopperGenerator = new ChopperGenerator(_chopperTexture, AddChopper);

            var levelReader = new LevelReader(_viewPortWidth);
            _level = new Level(levelReader);

            _level.OnGenerateEnemies += _level_OnGenerateEnemies;
            _level.OnGenerateTurret += _level_OnGenerateTurret;
            _level.OnLevelStart += _level_OnLevelStart;
            _level.OnLevelEnd += _level_OnLevelEnd;
            _level.OnLevelNoRowEvent += _level_OnLevelNoRowEvent;

            ResetGame();
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

        private IList<T> CleanObjects<T>(IList<T> objectList, Func<T, bool> predicate) where T : BaseObjectGame
        {
            List<T> listOfItemsToKeep = new List<T>();
            foreach (T item in objectList)
            {
                var performRemoval = predicate(item);

                if (performRemoval || item.Destroyed)
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
            chopper.OnObjectChanged += _onObjectChanged;
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
            _playerSprite.Update(gameTime);

            _level.GenerateLevelEvents(gameTime);

            foreach (var bullet in _bulletList)
            {
                bullet.MoveUp();
            }

            foreach (var missile in _missileList)
            {
                missile.Update(gameTime);
            }

            foreach (var chopper in _enemyList)
            {
                chopper.Update();
            }

            foreach (var turret in _turretList)
            {
                turret.Update(gameTime, _playerSprite.CenterPosition);
                turret.Active = turret.Position.Y > 0 && turret.Position.Y < _viewPortHeight;
            }

            foreach (var bullet in _turretBulletList)
            {
                bullet.Update();
            }

            UpdateExplosions(gameTime);
            RegulateShootingRate(gameTime);
            DetectCollisions();

            // get rid of bullets and missiles that have gone out of view
            _bulletList = CleanObjects(_bulletList);
            _missileList = CleanObjects(_missileList);
            _enemyList = CleanObjects(_enemyList);
            _turretBulletList = CleanObjects(_turretBulletList);
            _turretList = CleanObjects( _turretList, turret => turret.Position.Y > _viewPortHeight + 200);
        }

        private async void KillPlayer()
        {
            _playerDead = true;
            _playerLives -= 1;
            _livesText.NbLives = _playerLives;

            AddExplosion(_playerSprite.Position);
            RemoveGameObject(_playerSprite);

            await Task.Delay(TimeSpan.FromSeconds(2));

            if (_playerLives > 0)
            {
                ResetGame();
            }
            else
            {
                GameOver();
            }
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

            foreach (var bullet in _turretBulletList)
            {
                RemoveGameObject(bullet);
            }

            foreach (var turret in _turretList)
            {
                RemoveGameObject(turret);
            }

            _bulletList = new List<BulletSprite>();
            _turretBulletList = new List<TurretBulletSprite>();
            _turretList = new List<TurretSprite>();
            _missileList = new List<MissileSprite>();
            _explosionList = new List<ExplosionEmitter>();
            _enemyList = new List<ChopperSprite>();

            AddObject(_playerSprite);

            // position the player in the middle of the screen, at the bottom, leaving a slight gap at the bottom
            var playerXPos = _viewPortWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewPortHeight - _playerSprite.Height - 30;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);

            _playerDead = false;
            _level.Reset();
        }

        private void DetectCollisions()
        {
            var bulletCollisionDetector = new AABBCollisionDetector<BulletSprite, BaseObjectGame>(_bulletList);
            var missileCollisionDetector = new AABBCollisionDetector<MissileSprite, BaseObjectGame>(_missileList);
            var playerCollisionDetector = new AABBCollisionDetector<ChopperSprite, PlayerSprite>(_enemyList);
            var turretBulletCollisionDetector = new SegmentAABBCollisionDetector<PlayerSprite>(_playerSprite);

            bulletCollisionDetector.DetectCollisions(_enemyList, (bullet, chopper) =>
            {
                var hitEvent = new GamePlayStateEvents.ObjectHitBy(bullet);
                chopper.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                bullet.Destroy();
            });

            missileCollisionDetector.DetectCollisions(_enemyList, (missile, chopper) =>
            {
                var hitEvent = new GamePlayStateEvents.ObjectHitBy(missile);
                chopper.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                missile.Destroy();
            });

            bulletCollisionDetector.DetectCollisions(_turretList, (bullet, turret) =>
            {
                var hitEvent = new GamePlayStateEvents.ObjectHitBy(bullet);
                turret.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                bullet.Destroy();
            });

            missileCollisionDetector.DetectCollisions(_turretList, (missile, turret) =>
            {
                var hitEvent = new GamePlayStateEvents.ObjectHitBy(missile);
                turret.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                missile.Destroy();
            });

            if (!_playerDead)
            {
                var segments = new List<Segment>();
                foreach (var bullet in _turretBulletList)
                {
                    segments.Add(bullet.CollisionSegment);
                }

                turretBulletCollisionDetector.DetectCollisions(segments, _ =>
                {
                    KillPlayer();
                });

                playerCollisionDetector.DetectCollisions(_playerSprite, (chopper, player) =>
                {
                    KillPlayer();
                });
            }
        }



        private void RegulateShootingRate(GameTime gameTime)
        {
            // can't shoot bullets more than every 0.2 second
            if (_lastShotAt != null && gameTime.TotalGameTime - _lastShotAt > TimeSpan.FromSeconds(0.2))
            {
                _isShooting = false;
            }

            // can't shoot missiles more than every 1 second
            if (_lastMissileShotAt != null && gameTime.TotalGameTime - _lastMissileShotAt > TimeSpan.FromSeconds(1.0))
            {
                _isShootingMissile = false;
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

        private void GameOver()
        {
            var font = LoadFont(GameOverFont);
            var gameOverText = new GameOverText(font);
            var textPositionOnScreen = new Vector2(460, 300);

            gameOverText.Position = textPositionOnScreen;
            AddObject(gameOverText);
            _gameOver = true;
        }


        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);

            if (_gameOver)
            {
                // draw black rectangle at 30% transparency
                var screenBoxTexture = GetScreenBoxTexture(spriteBatch.GraphicsDevice);
                var viewportRectangle = new Rectangle(0, 0, _viewPortWidth, _viewPortHeight);
                spriteBatch.Draw(screenBoxTexture, viewportRectangle, Color.Black * 0.3f);
            }
        }

        private Texture2D GetScreenBoxTexture(GraphicsDevice graphicsDevice)
        {
            if (_screenBoxTexture == null)
            {
                _screenBoxTexture = new Texture2D(graphicsDevice, 1, 1);
                _screenBoxTexture.SetData<Color>(new Color[] { Color.White });
            }

            return _screenBoxTexture;
        }

        private void _level_OnLevelStart(object sender, LevelEvents.StartLevel e)
        {
            _levelStartEndText.Text = "Good luck, Player 1!";
            _levelStartEndText.Position = new Vector2(350, 300);
            AddObject(_levelStartEndText);
        }

        private void _level_OnLevelEnd(object sender, LevelEvents.EndLevel e)
        {
            _levelStartEndText.Text = "You escaped. Congrats!";
            _levelStartEndText.Position = new Vector2(300, 300);
            AddObject(_levelStartEndText);
        }

        private void _level_OnLevelNoRowEvent(object sender, LevelEvents.NoRowEvent e)
        {
            RemoveGameObject(_levelStartEndText);
        }

        private void _level_OnGenerateTurret(object sender, LevelEvents.GenerateTurret e)
        {
            var turret = new TurretSprite(LoadTexture(TurretTexture), LoadTexture(TurretMG2Texture), SCROLLING_SPEED);

            // position the turret offscreen at the top
            turret.Position = new Vector2(e.XPosition, -100);

            turret.OnTurretShoots += _turret_OnTurretShoots;
            turret.OnObjectChanged += _onObjectChanged;
            AddObject(turret);

            _turretList.Add(turret);
        }

        private void _turret_OnTurretShoots(object sender, GamePlayStateEvents.TurretShoots e)
        {
            var bullet1 = new TurretBulletSprite(LoadTexture(TurretBulletTexture), e.Direction, e.Angle);
            bullet1.Position = e.Bullet1Position;
            bullet1.ZIndex = -10;

            var bullet2 = new TurretBulletSprite(LoadTexture(TurretBulletTexture), e.Direction, e.Angle);
            bullet2.Position = e.Bullet2Position;
            bullet2.ZIndex = -10;

            AddObject(bullet1);
            AddObject(bullet2);

            _turretBulletList.Add(bullet1);
            _turretBulletList.Add(bullet2);
        }

        private void _level_OnGenerateEnemies(object sender, LevelEvents.GenerateEnemies e)
        {
            _chopperGenerator.GenerateChoppers(e.NbEnemies);
        }

        private void _onObjectChanged(object sender, IBaseGameStateEvent e)
        {
            var chopper = (BaseObjectGame)sender;
            switch (e)
            {
                case GamePlayStateEvents.ObjectLostLife ge:
                    if (ge.CurrentLife <= 0)
                    {
                        AddExplosion(new Vector2(chopper.Position.X - 40, chopper.Position.Y - 40));
                        chopper.Destroy();
                    }
                    break;
            }
        }
    }
}
