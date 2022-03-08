namespace Elsa.Jobs.Contracts;

public interface IJobSerializer
{
    string Serialize(IJob job);
    T Deserialize<T>(string json) where T : IJob;
}