namespace SampleDotNetOTEL.BusinessService.Controllers;

public class ErrorResponsePolicy
{
    private readonly double _rate;

    public ErrorResponsePolicy(double rate)
    {
        _rate = rate;
    }

    public bool IsProduceError()
    {
        return Random.Shared.NextDouble() > _rate;
    }
}