using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VETLY_BE.Entities
{
    public partial class DocumentoProcesado
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Cuerpo { get; set; }
        public DateTime? Fecha_Impresion { get; set; }
    }
}
