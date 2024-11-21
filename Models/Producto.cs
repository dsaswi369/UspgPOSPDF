using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Producto
    {

        [Column("id")]
        public long? Id { get; set; }

		[Column("codigo")]
		[MaxLength(100)]
		public string? Codigo { get; set; }

		[Column("nombre")]
        [MaxLength(255)]
        public string Nombre { get; set; }

        [Column("marca_id")]
        public long MarcaId { get; set; }

        [Column("clasificacion_id")]
        public long ClasificacionId { get; set; }

        [Column("precio")]
        [Range(0.01, 100000)]
        public decimal Precio { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("img_url")]
        public string? ImgUrl { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        public Marca? Marca { get; set; }

        public Clasificacion? Clasificacion { get; set; }

    }
}
