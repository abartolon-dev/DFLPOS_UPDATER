# Compatibilidad: ejecuta el script ubicado en la raiz de la solucion.
Push-Location ..
try {
    .\ScriptsDatabase\RegenerarEntidadesDFLSAI.ps1
}
finally {
    Pop-Location
}
