using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SLM.Models
{
    public class House
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string LandlordsId { get; set; }
        public string Address { get; set; }

   


    }
}
