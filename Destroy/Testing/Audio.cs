namespace Destroy
{
    using System;
    using System.Media;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 只支持Wav文件
    /// </summary>
    public class Audio
    {
        private SoundPlayer player;

        /// <summary>
        /// 同步加载音乐
        /// </summary>
        public Audio(string path)
        {
            player = new SoundPlayer(path);
            player.Load();
        }

        /// <summary>
        /// 开启新线程播放音乐
        /// </summary>
        public void Play() => player.Play();

        /// <summary>
        /// 停止当前播放
        /// </summary>
        public void Stop() => player.Stop();

        /// <summary>
        /// 开启新线程循环播放音乐
        /// </summary>
        public void PlayLooping() => player.PlayLooping();
    }

    [Obsolete("Dont use this")]
    /// <summary>
    /// 支持MP3文件
    /// </summary>
    public class MP3Audio
    {
        private string path;

        public MP3Audio(string path)
        {
            this.path = path;
        }

        /// <summary>   
        /// 播放
        /// </summary>   
        public void Play()
        {
            mciSendString("close all", "", 0, 0);
            mciSendString($"open {path} alias media", "", 0, 0);
            mciSendString("play media", "", 0, 0);
        }

        /// <summary>   
        /// 暂停
        /// </summary>   
        public void Pause()
        {
            mciSendString("pause media", "", 0, 0);
        }

        /// <summary>   
        /// 停止
        /// </summary>   
        public void Stop()
        {
            mciSendString("close media", "", 0, 0);
        }

        /// <summary>   
        /// API函数
        /// </summary>   
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        private static extern int mciSendString(
         string lpstrCommand,
         string lpstrReturnString,
         int uReturnLength,
         int hwndCallback
        );
    }
}