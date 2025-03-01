using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiakaBotBackEnd
{
    internal class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int QuantityProduct { get; set; }
        public string WarrantyProduct { get; set; }
    }
}
