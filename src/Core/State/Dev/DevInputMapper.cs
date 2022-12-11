namespace Core.State.Dev
{
    public class DevInputMapper : BaseInputMapper
    {
        public override IEnumerable<IBaseInputCommand> GetKeyboardState(KeyboardState state)
        {
            var commands = new List<DevInputCommand>();

            if (state.IsKeyDown(Keys.Escape))
            {
                commands.Add(new DevInputCommand.DevQuit());
            }

            if (state.IsKeyDown(Keys.Space))
            {
                commands.Add(new DevInputCommand.DevShoot());
            }

            return commands;
        }
    }
}
