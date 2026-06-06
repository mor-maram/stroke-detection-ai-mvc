using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp.PixelFormats;
//using System.Drawing;
using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace MediAI.Services
{
    public interface IStrokePredictionService
    {
        Task<PredicateResultViewModel> PredictAsync(string imagePath);
    }

    public class StrokePredictionService : IStrokePredictionService
    {
        private readonly InferenceSession _session;
        private readonly string[] Categories = { "Normal", "Stroke" };

        public StrokePredictionService(InferenceSession session)
        {
            _session = session;
        }

        public async Task<PredicateResultViewModel> PredictAsync(string imagePath)
        {
            // 1. تحميل ومعالجة الصورة
            var inputTensor = await LoadAndPreprocessImageAsync(imagePath);

            // 2. تجهيز الإدخال
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("conv2d_input", inputTensor)
            };

            // 3. تنفيذ التنبؤ
            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();
            var max = output.Max();

            // 4. استنتاج الفئة المتوقعة
            int predictedIndex = Array.IndexOf(output, max);

            PredicateResultViewModel Result = new PredicateResultViewModel
            {
                Condition = predictedIndex >= 0 && predictedIndex < Categories.Length ? Categories[predictedIndex] : "Unknown",
                ResultRate = (decimal)max,
                ResultIndex = predictedIndex >= 0 && predictedIndex < Categories.Length ? predictedIndex : -1,
            };
            // 5. إعادة النتيجة كنص
            return Result;
        }

        private async Task<DenseTensor<float>> LoadAndPreprocessImageAsync(string imagePath)
        {
            using var image = await Image.LoadAsync<Rgb24>(imagePath);

            // تغيير الحجم إلى 224x224
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(224, 224),
                Mode = ResizeMode.Stretch
            }));

            var imageArray = new float[1 * 224 * 224 * 3]; // [Batch, Height, Width, Channels]
            int i = 0;

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = image[x, y];
                    imageArray[i++] = pixel.R / 255.0f;
                    imageArray[i++] = pixel.G / 255.0f;
                    imageArray[i++] = pixel.B / 255.0f;
                }
            }

            return new DenseTensor<float>(imageArray, new[] { 1, 224, 224, 3 });
        }
    }    
}
