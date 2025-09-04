using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class FileDataDTO
    {
        public Stream Content { get; set; }
        public string FileName { get; set; }
    }
}
