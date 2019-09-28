using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SLM.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public int HouseId { get; set; }
        public string TenantId { get; set; }
        public string LandlordId { get; set; }
        public string Status { get; set; } 
    }
}
