using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLM.DTOs.House
{
    public class CreateOrUpdateHouseInput
    {
        public int Id { get; set; }
       public string Address { get; set; }
       public string Name { get; set; }     
        
    }
}
