using System;
using System.ComponentModel.DataAnnotations;

namespace BeatStreamr.Models
{
    public class BeatArtistViewModel
    {
        [Display(Name = "Artist")]
        public string Name
        {
            get;
            set;
        }

        public string ProfileImagePath
        {
            get;
            set;
        }

        public string HeaderImagePath
        {
            get;
            set;
        }

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

        public decimal BPM
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public int ArtistID
        {
            get;
            set;
        }

        [DataType(DataType.Currency)]
        public decimal Price
        {
            get;
            set;
        }

        public int? AlbumID
        {
            get;
            set;
        }

        public string BeatFilePath
        {
            get;
            set;
        }

        public string ArtFilePath
        {
            get;
            set;
        }
    }
}
