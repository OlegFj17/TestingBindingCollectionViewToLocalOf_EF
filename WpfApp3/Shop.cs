using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    /// <summary>
    /// тестовая сущность для БД
    /// </summary>
    public class Shop
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
