using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaOnline.Models
{
    public class ElevModel
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public Dictionary<DateTime,int> ZilePlatite { get;set; }
    }
}
