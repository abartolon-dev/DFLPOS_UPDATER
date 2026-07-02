using DflPosUpdater.App.DAL.DFLSAI;
using DflPosUpdater.App.Entities.Models.DFLSAI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflPosUpdater.App.DAL.Data.DFLSAI
{   
    public class SiteRepository
    {
        private DflSaiDbContext _dflsaiDbContext;
        public SiteRepository(DflSaiDbContext db)
        {
            _dflsaiDbContext = db;
        }        
        public List<SiteModel> GetAllSites()
        {
            var sites = _dflsaiDbContext.Sites.Where(x => x.SiteType == "TIENDA" && x.SiteName != "PROCESADORA").Select(s => new SiteModel { SiteCode = s.SiteCode, SiteName = s.SiteName, NewErp = s.NewErp, City = s.City ?? "Todos", District = s.DistrictCode ?? "D999" }).ToList();
            return sites;
        }
    }
}
