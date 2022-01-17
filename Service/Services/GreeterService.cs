using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
namespace CasCap.Services;

public class GreeterService : Greeter.GreeterBase
{
    readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger) => _logger = logger;

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        var ctx = context.GetHttpContext();

        _logger.LogInformation(nameof(SayHello));
        return Task.FromResult(new HelloResponse
        {
            Message = "Hello " + request.Name
        });
    }

    public override Task<FullResponse> GetFullResponse(Empty _, ServerCallContext context)
    {
        var c1 = context;
        var ctx = context.GetHttpContext();

        _logger.LogInformation(nameof(SayHello));
        return Task.FromResult(new FullResponse
        {
            //Start = new Google.Protobuf.WellKnownTypes.Timestamp() { },
            //strz = "wibble",
        });
    }

    //public override async Task LotsOfReplies(HelloRequest request, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
    //{
    //    await Task.Delay(0);
    //    _logger.LogInformation(nameof(LotsOfReplies));
    //    return Task.FromResult(new HelloResponse
    //    {
    //        Message = "ok"
    //    });
    //}

    //public override async Task GetWeatherStream(Empty _, IServerStreamWriter<WeatherData> responseStream, ServerCallContext context)
    //{
    //    // TODO - Implementation!
    //}

    //todo: add remaining methods here
}