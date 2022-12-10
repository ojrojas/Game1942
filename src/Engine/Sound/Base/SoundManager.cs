using Engine.State;

namespace Engine.Sound.Base
{
    public class SoundManager
    {
        private int _soundTrackIndex = -1;
        private IList<SoundEffectInstance> _soundTracks = new List<SoundEffectInstance>();
        private Dictionary<Type, SoundEffect> _soundBank = new Dictionary<Type, SoundEffect>();

        public void SetSoundTrack(List<SoundEffectInstance> tracks)
        {
            _soundTracks = tracks;
            _soundTrackIndex = _soundTracks.Count -1;
        }

        public void OnNotify(IBaseGameStateEvent gameEvent)
        {
            if(_soundBank.ContainsKey(gameEvent.GetType()))
            {
                var sound = _soundBank[gameEvent.GetType()];
                sound.Play();
            }
        }

        public void RegisterSound(BaseGameStateEvent gameEvent, SoundEffect sound)
        {
            _soundBank.Add(gameEvent.GetType(), sound);
        }

        public void PlaySoundTrack()
        {
            var nbTracks = _soundTracks.Count;
            if(nbTracks <=  0 ) 
            {
                return;
            }

            var currentTrack = _soundTracks[_soundTrackIndex];
            var nextTrack = _soundTracks[(nbTracks + 1) % nbTracks];

            if (currentTrack.State == SoundState.Stopped)
            {
                nextTrack.Play();
                _soundTrackIndex ++;
                if(_soundTrackIndex >= _soundTracks.Count)
                {
                    _soundTrackIndex = 0;
                }
            }

        }
    }
}
