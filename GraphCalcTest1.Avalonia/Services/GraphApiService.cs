using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphCalcTest1.Avalonia.Services
{
    public class GraphApiDto
    {
        public Guid Id { get; set; }
        public string Expression { get; set; }
        public string IndependentVariable { get; set; }
        public List<MathPointDto> Points { get; set; } = new();
        public NumericRangeDto Range { get; set; }
    }

    public class MathPointDto
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class NumericRangeDto
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
    }

    public class GraphCalculationRequestDto
    {
        public string Expression { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double XStep { get; set; }
        public bool AutoYRange { get; set; } = false;
    }

    public interface IGraphApiService
    {
        Task<GraphApiDto> CalculateGraphAsync(string expression, double xMin, double xMax, double xStep, bool autoYRange = false);
    }

    public class GraphApiService : IGraphApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public GraphApiService(string baseUrl = "http://localhost:5000")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        public async Task<GraphApiDto> CalculateGraphAsync(string expression, double xMin, double xMax, double xStep, bool autoYRange = false)
        {
            try
            {
                var request = new GraphCalculationRequestDto
                {
                    Expression = expression,
                    XMin = xMin,
                    XMax = xMax,
                    XStep = xStep,
                    AutoYRange = autoYRange
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/graphcalculation/calculate",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<GraphApiDto>(json, options);

                return result ?? throw new Exception("Failed to deserialize graph data");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating graph: {ex.Message}", ex);
            }
        }
    }
}
