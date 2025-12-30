using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstruplastERP.Core
{
    public class ClienteMaterialFazon
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int MaterialGenericoId { get; set; } // El ID 990, 991, etc.
        public int MaterialRealId { get; set; }     // El material físico del cliente

        // Relaciones
        public Cliente Cliente { get; set; }
        public Producto MaterialGenerico { get; set; }
        public Producto MaterialReal { get; set; }
    }
}
