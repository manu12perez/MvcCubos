using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCubos.Models
{
    [Table("COMPRA")]
    public class Compra
    {
        [Key]
        [Column("ID_COMPRA")]
        public int IdCompra { get; set; }

        [Column("ID_CUBO")]
        public int IdCubo { get; set; }

        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Column("PRECIO")]
        public int Precio { get; set; }

        [Column("FECHAPEDIDO")]
        public DateTime FechaPedido { get; set; }
    }
}
