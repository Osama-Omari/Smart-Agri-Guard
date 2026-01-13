using ApplicationLayer.DTOs;
using DataAccessLayer.Models;
using InfrastructureLayer.AI.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InfrastructureLayer.AI.Services
{
    public class TomatoHealthModel : IPlantHealthModel
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _onnxModel;
        private readonly PredictionEngine<TomatoHealthInput, TomatoHealthOutput> _predictionEngine;

        public TomatoHealthModel(IWebHostEnvironment env)
        {
            var modelPath = Path.Combine(env.ContentRootPath, "AIModels", "tomato_model.onnx");
            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"ONNX model not found: {modelPath}");

            _mlContext = new MLContext();

            // Define ONNX pipeline
            var pipeline = _mlContext.Transforms.ApplyOnnxModel(
                modelFile: modelPath,
                inputColumnNames: new[] { "float_input" },
                outputColumnNames: new[] { "output_label", "output_probability" }
            );

            // Fit pipeline on empty data
            var emptyData = _mlContext.Data.LoadFromEnumerable(new List<TomatoHealthInput>());
            _onnxModel = pipeline.Fit(emptyData);

            // Create PredictionEngine for predicted label only
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TomatoHealthInput, TomatoHealthOutput>(_onnxModel);
        }

        /// <summary>
        /// Predict only the label (Healthy / Non-Healthy)
        /// </summary>
        public async Task<PlantHealthStatus> PredictAsync(TomatoHealthInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var result = _predictionEngine.Predict(input);
            int predictedLabel = (int)result.output_label[0];
            return await Task.FromResult((PlantHealthStatus)predictedLabel);
        }

        /// <summary>
        /// Predict label and get probability/confidence
        /// </summary>
        public async Task<(PlantHealthStatus Status, float Confidence)> PredictWithConfidenceAsync(TomatoHealthInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Load single sample into IDataView
            var inputDv = _mlContext.Data.LoadFromEnumerable(new List<TomatoHealthInput> { input });

            // Transform through ONNX model
            var transformed = _onnxModel.Transform(inputDv);

            // Predicted label
            var predictedLabels = transformed.GetColumn<long>("output_label").ToArray();
            int predictedLabel = (int)predictedLabels[0];

            // Probabilities are sequence of dictionaries
            var probabilitiesSeq = transformed.GetColumn<Dictionary<long, float>>("output_probability").FirstOrDefault();
            float confidence = 0f;

            if (probabilitiesSeq != null && probabilitiesSeq.TryGetValue(predictedLabel, out float prob))
            {
                confidence = prob * 100f;
            }

            return await Task.FromResult(((PlantHealthStatus)predictedLabel, confidence));
        }
    }
}
