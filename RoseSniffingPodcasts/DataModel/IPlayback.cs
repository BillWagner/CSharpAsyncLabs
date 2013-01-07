namespace RoseSniffingPodcasts.Data
{
    using global::System;

    public interface IPlayback
    {
        void setSource(Uri source);
        void PlayPause();
    }
}
