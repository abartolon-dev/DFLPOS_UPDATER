using DflPosUpdater.App.Entities;
using DflPosUpdater.App.Entities.Models.DFLSAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflPosUpdater.App.BLL.Services.Sucursales.DFLSAI
{
    public interface ISiteService
    {
        Task<List<SiteModel>> GetAllSitesAsync(CancellationToken cancellationToken = default);        
    }
}
