using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflPosUpdater.App.Entities.Models.DFLSAI
{
    public class SiteModel
    {
        public string SiteCode { get; set; }
        public int Code { get; set; }
        public string SiteName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int Status { get; set; }
        public DateTime NewErpDate { get; set; }
        public bool? NewErp { get; set; }
        public string IpSite { get; set; }
        public string PortSite { get; set; }
        public string UrlSite { get; set; }        
    }
}
