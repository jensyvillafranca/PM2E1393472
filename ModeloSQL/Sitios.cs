using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PM2E1393472.ModeloSQL
{

    [Table("sitios")]
    public class Sitios
    {
        [PrimaryKey, AutoIncrement] 
        public int id { get; set; }

        public string Imagen { get; set; }

        public string latitud { get; set; }

        public string longitud { get; set; }

        [MaxLength(100)]
        public string descripcion { get; set; }

        //public string NombreCompleto
        //{
            //get { return $"{nombres} {apellidos}"; }
        //}


    }
}
