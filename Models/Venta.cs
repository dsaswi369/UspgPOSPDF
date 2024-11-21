using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Venta
    {
        [Column("id")]
        public long? Id { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("cliente_id")]
        public long ClienteId { get; set; }

        [Column("sucursal_id")]
        public long SucursalId { get; set; }

        public Cliente? Cliente { get; set; } // Relacion de uno a uno

        public Sucursal? Sucursal { get; set; } // Relacion de uno a uno
		public ICollection<DetalleVenta> DetallesVenta { get; set; }

	}
}
