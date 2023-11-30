namespace SampleDotNetOTEL.BusinessService.Controllers;

public class ErrorResponsePolicy(double rate)
{
    public bool IsProduceError()
    {
        return Random.Shared.NextDouble() > rate;
    }
}