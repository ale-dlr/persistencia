using System.Reflection;
using Microsoft.Data.Sqlite;

namespace Prestamo_Biblioteca.Persistencia;

internal sealed class InicializadorBaseDatos
{
    public string RutaBaseDatos { get; } =
        Environment.GetEnvironmentVariable("PRESTAMO_BIBLIOTECA_DB")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UMG",
            "Biblioteca",
            "biblioteca.db");

    public string CadenaConexion => new SqliteConnectionStringBuilder
    {
        DataSource = RutaBaseDatos,
        ForeignKeys = true
    }.ToString();
    
    public void Inicializar()
    {
        var directorio = Path.GetDirectoryName(RutaBaseDatos);
        if (!String.IsNullOrEmpty(directorio)) Directory.CreateDirectory(directorio);
        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = LEERSCRIPTTABLAS();
        comando.ExecuteNonQuery();
    }

    private static string LEERSCRIPTTABLAS()
    {
        var enable = Assembly.GetExecutingAssembly();
        var recurso = Ensamble.GetManifestResourceName().Single(nombre.endsWith("Crear_tablas.sql", StringComparison.OrdinalIgnoreCase));
        using var stream = enable.GetManifestResourceStream(recurso) ?? throw new InvalidOperation("Error");
        using var lector = new StreamReader(stream);
        return lector.ReadToEnd();
    }
}