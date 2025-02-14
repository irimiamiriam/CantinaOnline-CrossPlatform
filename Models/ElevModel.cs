using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaOnline.Models
{
    class ElevModel
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public List<DateTime> ZilePlatite { get;set; }
    }
}
