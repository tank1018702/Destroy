namespace Destroy
{
    using System.Media;

    public class Audio
    {
        private SoundPlayer player;

        public Audio(string path)
        {
            player = new SoundPlayer(path);
            player.Load();
        }

        public void Play() => player.Play();

        public void PlayLooping() => player.PlayLooping();
    }
}