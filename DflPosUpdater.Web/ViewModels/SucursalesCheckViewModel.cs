using DflPosUpdater.App.Entities.DFLSAI;
using DflPosUpdater.App.Entities.Models.DFLSAI;

namespace DflPosUpdater.Web.ViewModels
{
    public class SucursalesCheckViewModel
    {       
        public ListSitePosition ListPosition { get; set; }
        public List<SiteModel> Sites { get; set; }
        public List<string> Districts { get; set; }
        public SucursalesCheckViewModel()
        {              
            ListPosition = new ListSitePosition();
            Sites = new List<SiteModel>();
            Districts = new List<string>();
        }
    }
    public class ListSitePosition
    {
        public int first_column_for_column_2 { get; set; }
        public int first_column_for_column_3 { get; set; }
        public int second_column_forcolumn_3 { get; set; }
        public int first_column_for_column_4 { get; set; }
        public int second_column_for_column_4 { get; set; }
        public int third_column_for_column_4 { get; set; }
        public int first_column_for_column_2_with_inputs { get; set; }
        public int all_sites_actives { get; set; }
        public int all_sites { get; set; }
    }
}
