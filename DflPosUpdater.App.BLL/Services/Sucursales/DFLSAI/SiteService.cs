using DflPosUpdater.App.DAL.Data.DFLSAI;
using DflPosUpdater.App.DAL.DFLSAI;
using DflPosUpdater.App.Entities.Models.DFLSAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DflPosUpdater.App.BLL.Services.Sucursales.DFLSAI
{
    public class SiteService : ISiteService
    {
        private SiteRepository _siteRepository;        

        Task<List<SiteModel>> ISiteService.GetAllSitesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_siteRepository.GetAllSites().Where(w => w.NewErp == true).ToList());
        }
    }
}
