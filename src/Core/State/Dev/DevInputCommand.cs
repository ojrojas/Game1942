namespace Core.State.Dev
{
    public class DevInputCommand : IBaseInputCommand
    {
        // Out of Game Commands
        public class DevQuit : DevInputCommand { }
        public class DevShoot : DevInputCommand { }
    }
}
