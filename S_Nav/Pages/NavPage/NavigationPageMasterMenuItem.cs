using System;

namespace S_Nav
{

    public class NavigationPageMasterMenuItem
    {
        public NavigationPageMasterMenuItem()
        {
            TargetType = typeof(NavigationPageMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}