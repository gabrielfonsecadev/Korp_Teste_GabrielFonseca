using System.Net.Http.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace BillingService.Services;

public class StockServiceClient : IStockServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StockServiceClient> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;

    public StockServiceClient(
        HttpClient httpClient, 
        ILogger<StockServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configure resilience pipeline with retry and circuit breaker
        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        "Tentativa {AttemptNumber} de chamar Stock Service falhou. Tentando novamente em {Delay}ms",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(30),
                OnOpened = args =>
                {
                    _logger.LogError("Circuit breaker aberto! Stock Service está indisponível.");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    _logger.LogInformation("Circuit breaker fechado. Stock Service voltou ao normal.");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                _logger.LogInformation("Buscando produto {ProductId} no Stock Service", productId);
                var response = await _httpClient.GetAsync($"api/products/{productId}", cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Produto {ProductId} não encontrado no Stock Service", productId);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
            }, CancellationToken.None);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError("Circuit breaker está aberto. Stock Service indisponível.");
            throw new InvalidOperationException("O serviço de estoque está temporariamente indisponível. Por favor, tente novamente mais tarde.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto {ProductId} no Stock Service", productId);
            throw new InvalidOperationException("Erro ao comunicar com o serviço de estoque", ex);
        }
    }

    public async Task<bool> UpdateStockAsync(int productId, decimal quantity)
    {
        try
        {
            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                _logger.LogInformation(
                    "Atualizando estoque do produto {ProductId} em {Quantity} no Stock Service", 
                    productId, 
                    quantity);

                var request = new UpdateStockRequest { Quantity = quantity };
                var response = await _httpClient.PatchAsJsonAsync(
                    $"api/products/{productId}/stock", 
                    request, 
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Falha ao atualizar estoque: {Error}", error);
                    throw new InvalidOperationException($"Falha ao atualizar estoque: {error}");
                }

                return true;
            }, CancellationToken.None);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError("Circuit breaker está aberto. Stock Service indisponível.");
            throw new InvalidOperationException("O serviço de estoque está temporariamente indisponível. Por favor, tente novamente mais tarde.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estoque do produto {ProductId}", productId);
            throw new InvalidOperationException("Erro ao comunicar com o serviço de estoque", ex);
        }
    }
}
