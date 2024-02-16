using SQLite;

namespace PM2E1393472.Controles
{
    public class SitiosControl
    {
        private SQLiteAsyncConnection conec;
    
    public SitiosControl()
    {

    }

    public async Task Init() //Creando tarea
    {
        if (conec is not null) //si la conexión actualmente es nula vamos a crear la conexión como tal
        {
            return;
        }
        SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite |
                                             SQLite.SQLiteOpenFlags.Create |
                                             SQLite.SQLiteOpenFlags.SharedCache; //se usan para inicializar la conexión a la base de datos
                                                                                 //string rutaBaseDatos = string.Empty;
                                                                                 //rutaBaseDatos = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);


        conec = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBSitios.db3"), extensiones); //la base de datos db Sitios va estar ubicada en el directorio de la bd
                                                                                                                         //.db3 es la extensión de sql lite
        var creacion = await conec.CreateTableAsync<ModeloSQL.Sitios>();
    }

    //CREAR
    public async Task<int> StoreSitio(ModeloSQL.Sitios Sitios)
    {
        await Init();
        if (Sitios.id == 0)
        {
            return await conec.InsertAsync(Sitios);
        }
        else
        {
            return await conec.UpdateAsync(Sitios);
        }
    }

    public async Task<List<ModeloSQL.Sitios>> GetListSitios()
    {
        await Init();
        return await conec.Table<ModeloSQL.Sitios>().ToListAsync();
    }

    public async Task<ModeloSQL.Sitios> GetSitio(int pid)
    {
        await Init();
        return await conec.Table<ModeloSQL.Sitios>().Where(i => i.id == pid).FirstOrDefaultAsync();
    }

    public async Task<int> DeleteSitio(ModeloSQL.Sitios Sitios)
    {
        await Init();
        return await conec.DeleteAsync(Sitios);
    }
    }
}
