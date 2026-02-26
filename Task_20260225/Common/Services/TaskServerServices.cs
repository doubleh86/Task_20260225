namespace Task_20260225.Common.Services;

public class TaskServerServices
{
    private readonly LoggerService _loggerService = new();
    private IConfiguration _configuration;
    
    public LoggerService LoggerService => _loggerService;
    public void Initialize(List<string> configurationFiles)
    {
        _InitializeConfiguration(configurationFiles);
        _loggerService.CreateLogger(_configuration);
    }

    private void _InitializeConfiguration(List<string> configurationFiles)
    {
        var configurationBuilder = new ConfigurationBuilder();
        foreach (var configurationFile in configurationFiles)
        {
            if (File.Exists(configurationFile) == false)
            {
                continue;
            }
            
            configurationBuilder.AddJsonFile(configurationFile);
        }
        
        _configuration = configurationBuilder.Build();
    }
    
    public T GetValue<T>(string key, T defaultValue)
    {
        return _configuration.GetValue<T>(key) ?? defaultValue;
    }

    public T GetSection<T>(string key) where T: class, new()
    {
        var options = new T();
        _configuration.GetSection(key).Bind(options);

        return options;
    }
    
}