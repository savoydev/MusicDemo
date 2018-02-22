using System;
using System.Collections.Generic;
namespace BeatStreamr.Models
{
    public class UserBeatsViewModel
    {
        public List<BeatArtistViewModel> beatsList;
        public User User
        {
            get;
            set;
        }
    }
}
