using System;

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