using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Sucursal
    {

        [Column("id")]
        public long Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("area")]
        public string Area { get; set; }

        [Column("ciudad")]
        public string Ciudad { get; set; }
    }
}
