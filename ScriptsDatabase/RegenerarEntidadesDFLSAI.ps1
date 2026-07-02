$connection = "Server=ABARTOLON;Database=DFLSAI;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Application Name=EntityFramework"

# Ejecutar desde la raiz de la solucion DflPosUpdaterCapas.
# Genera el DbContext en App.DAL y las entidades SITES/MA_CODE en App.Entities.

dotnet ef dbcontext scaffold $connection Microsoft.EntityFrameworkCore.SqlServer `
    --project DflPosUpdater.App.DAL/DflPosUpdater.App.DAL.csproj `
    --startup-project DflPosUpdater.Web/DflPosUpdater.Web.csproj `
    --context DflSaiDbContext `
    --context-dir Data/DFLSAI `
    --output-dir ../DflPosUpdater.App.Entities/Models/DFLSAI `
    --namespace DflPosUpdater.App.Entities.DFLSAI `
    --context-namespace DflPosUpdater.App.DAL.DFLSAI `
    --table SITES `
    --table MA_CODE `
    --force `
    --no-onconfiguring `
    --data-annotations
