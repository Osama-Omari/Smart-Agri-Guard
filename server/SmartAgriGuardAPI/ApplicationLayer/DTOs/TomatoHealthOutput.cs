using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class TomatoHealthOutput
    {
        [VectorType(1)]
        public long[] output_label { get; set; }
    }

}
