using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using System.ComponentModel.DataAnnotations;
namespace BeatStreamr.Models
{
    public class Beat
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

        public TimeSpan? Duration
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
