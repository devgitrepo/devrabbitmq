
namespace RabbitMQConfig
{
    public interface IPublisher
    {
        void Publish(string message, string routingKey, IDictionary<string, object> headers, string timetolive = "30000");
    }
}
