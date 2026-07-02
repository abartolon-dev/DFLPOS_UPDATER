namespace DflPosUpdater.App.Entities;

public enum VersionEstado
{
    Borrador = 0,
    Publicada = 1,
    Cancelada = 2
}

public enum DespliegueEstado
{
    Pendiente = 0,
    EnProceso = 1,
    Exitoso = 2,
    Fallido = 3,
    Cancelado = 4,
    Reintentando = 5,
    Omitido = 6
}

public enum LogTipo
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Success = 3
}
