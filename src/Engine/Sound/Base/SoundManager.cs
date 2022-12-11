namespace Engine.Sound.Base
{
    public class SoundManager
    {
        private int _soundTrackIndex = -1;
        private IList<SoundEffectInstance> _soundTracks = new List<SoundEffectInstance>();
        private Dictionary<Type, SoundBankItem> _soundBank = new Dictionary<Type, SoundBankItem>();

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
                sound.Sound.Play(sound.Attributes.Volume, sound.Attributes.Pitch, sound.Attributes.Pan);
            }
        }

        public void RegisterSound(IBaseGameStateEvent gameEvent, SoundEffect sound)
        {
            RegisterSound(gameEvent, sound, 1.0f, .0f, .0f);
        }

        public void RegisterSound(IBaseGameStateEvent gameEvent, SoundEffect sound, float volume, float pitch, float pan) 
        {
            _soundBank.Add(gameEvent.GetType(), new SoundBankItem(sound, new SoundAttributes(volume, pitch, pan)));
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
