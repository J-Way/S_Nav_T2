using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Nav.Pages.NavPage.Searches
{

    public class SearchRoomMasterMenuItem
    {
        public SearchRoomMasterMenuItem()
        {
            TargetType = typeof(SearchRoomMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}