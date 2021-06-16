using System;
using System.Collections.Generic;

namespace Hishop.Weixin.MP.Domain
{
    public interface IMedia
    {
        int MediaId { get; set; }
    }

    public interface IThumbMedia
    { 
        int ThumbMediaId { get; set; }
    }


    public class Image : IMedia
    {
        public int MediaId { get; set; }
    }

    public class Voice : IMedia 
    {
        public int MediaId { get; set; }
    }

    public class Video : IMedia, IThumbMedia
    {
        public int MediaId { get; set; }

        public int ThumbMediaId { get; set; }        
    }

    public class Music : IThumbMedia
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string MusicUrl { get; set; }

        public string HQMusicUrl { get; set; }

        public int ThumbMediaId { get; set; }
    }
}
