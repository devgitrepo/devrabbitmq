using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQConfig
{
    public class Publisher: IPublisher
    {
        private readonly IModel _model;
        private readonly string _exchange;
        private bool _disposed;

        public Publisher(IConnectionProvider provider, string exchange, string type, bool durable=false, bool autoDelete=false,  int timetolive=30000)
        {
            _exchange = exchange;
            _model = provider.Connection().CreateModel();
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", timetolive }
            };
            _model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments:ttl);
        }

        public void Publish(string message, string routingKey, IDictionary<string, object> headers, string timetolive ="30000")
        {
            var properties = _model.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;
            properties.Expiration = timetolive;

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _model.BasicPublish(_exchange, routingKey, properties, body);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _model?.Close();

            _disposed = true;
        }
    }
}
