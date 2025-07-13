using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManualMovementsManager.Infrastructure.Serialization
{
    public static class JsonSerializationConfig
    {
        public static JsonSerializerOptions CreateDefaultOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        public static JsonSerializerOptions CreateApiOptions()
        {
            var options = CreateDefaultOptions();
            
            // Configurações específicas para API
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = false; // Para produção
            
            return options;
        }
    }
} 