using Core.Objects;

namespace Core.State.Dev
{
    /// <summary>
    /// Used to test out new things, like particle engines and shooting missiles
    /// </summary>
    public class DevState : BaseGameState
    {
        private const string ExhaustTexture = @"images\particles\cloud";
        private const string MissileTexture = @"images\ammo\missile";
        private const string PlayerFighter = @"images\player\fighter";

        private ExhaustEmitter _exhaustEmitter;
        private MissileSprite _missile;
        private PlayerSprite _player;

        public override void LoadContent()
        {
            var exhaustPosition = new Vector2(_viewPortWidth / 2, _viewPortHeight / 2);
            _exhaustEmitter = new ExhaustEmitter(LoadTexture(ExhaustTexture), exhaustPosition);
            AddObject(_exhaustEmitter);

            _player = new PlayerSprite(LoadTexture(PlayerFighter));
            _player.Position = new Vector2(500, 500);
            AddObject(_player);
        }

        public override void HandleInput(GameTime gameTime)
        {
            BaseInputManager.GetCommand(cmd =>
            {
                if (cmd is DevInputCommand.DevQuit)
                {
                    OnNotified(new BaseGameStateEvent.GameQuit());
                }

                if (cmd is DevInputCommand.DevShoot)
                {
                    _missile = new MissileSprite(LoadTexture(MissileTexture), LoadTexture(ExhaustTexture));
                    _missile.Position = new Vector2(_player.Position.X, _player.Position.Y - 25);
                    AddObject(_missile);
                }
            });
        }

        public override void UpdateGameState(GameTime gameTime)
        {
            _exhaustEmitter.Position = new Vector2(_exhaustEmitter.Position.X, _exhaustEmitter.Position.Y - 3f);
            _exhaustEmitter.Update(gameTime);

            if (_missile != null)
            {
                _missile.Update(gameTime);

                if (_missile.Position.Y < -100)
                {
                    RemoveGameObject(_missile);
                }
            }

            if (_exhaustEmitter.Position.Y < -200)
            {
                RemoveGameObject(_exhaustEmitter);
            }
        }

        protected override void SetInputManager()
        {
            BaseInputManager = new BaseInputManager(new DevInputMapper());
        }
    }
}
