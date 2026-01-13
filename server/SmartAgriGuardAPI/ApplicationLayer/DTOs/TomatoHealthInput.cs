using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class TomatoHealthInput
    {
        [ColumnName("float_input")]
        [VectorType(7)]
        public float[] Features => new float[]
        {
            Temperature, Humidity, SoilMoisture,
            Nitrogen, Phosphorus, Potassium, Ph
        };

        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float SoilMoisture { get; set; }
        public float Nitrogen { get; set; }
        public float Phosphorus { get; set; }
        public float Potassium { get; set; }
        public float Ph { get; set; }
    }
}
