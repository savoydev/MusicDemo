using System;
using System.Collections.Generic;
namespace BeatStreamr.Models
{
    public class Album
    {
        public int ID
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public string AlbumArtFilePath
        {
            get;
            set;
        }

        public int ArtisID
        {
            get;
            set;
        }
    }
}
