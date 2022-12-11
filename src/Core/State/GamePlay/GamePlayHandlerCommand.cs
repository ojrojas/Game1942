namespace Core.State.GamePlay
{
    public partial class GamePlayState
    {
        public void HandlerInputGamePlay(GameTime gameTime)
        {
            BaseInputManager.GetCommand(command =>
            {
                if (command is GamePlayInputCommand.GameMenu)
                {
                    OnNotified(new BaseGameStateEvent.GameQuit());
                }
                if (command is GamePlayInputCommand.GamePause)
                {
                    OnNotified(new BaseGameStateEvent.Pause());
                }
                if (command is GamePlayInputCommand.PlayerMoveLeft)
                {
                    _playerSprite.MoveLeft();
                    KeepPlayerInBounds();
                }
                if (command is GamePlayInputCommand.PlayerMoveRight)
                {
                    _playerSprite.MoveRight();
                    KeepPlayerInBounds();
                }
                if (command is GamePlayInputCommand.PlayerMoveUp)
                {
                    _playerSprite.MoveUp();
                    KeepPlayerInBounds();
                }
                if (command is GamePlayInputCommand.PlayerMoveDown)
                {
                    _playerSprite.MoveDown();
                    KeepPlayerInBounds();
                }
                if (command is GamePlayInputCommand.PlayerActionY)
                {
                    Shoot(gameTime, TypeShootEnum.Bullets);
                }
                if (command is GamePlayInputCommand.PlayerActionX)
                {
                    Shoot(gameTime, TypeShootEnum.None);
                }
                if (command is GamePlayInputCommand.PlayerActionA)
                {
                    Shoot(gameTime, TypeShootEnum.Special);
                }
                if (command is GamePlayInputCommand.PlayerActionB)
                {
                    Shoot(gameTime, TypeShootEnum.Missile);
                }
            });
        }
    }
}
