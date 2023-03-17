using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConfig
{
    public class Subscriber: ISubscriber
    {
        private readonly IModel _model;
        private readonly string _exchange;
        private readonly string _queue;
        private bool _disposed;

        public Subscriber(IConnectionProvider provider, string exchange, string queue, string routingKey, string exchangeType, int timetolive=30000, ushort prefetchSize=10)
        {
            _exchange = exchange;
            _queue = queue;

            _model = provider.Connection().CreateModel();


            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", timetolive }
            };
            _model.ExchangeDeclare(_exchange, exchangeType, arguments:ttl);

            _model.QueueDeclare(_queue, durable:false, exclusive:false, autoDelete:false);
            _model.QueueBind(_queue, exchange, routingKey);
            _model.BasicQos(0, prefetchSize, false);
        }

        public Subscriber(IConnectionProvider provider, string exchange, string queue, string routingKey,  string type, bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            _exchange = exchange;
            _queue = queue;

            _model = provider.Connection().CreateModel();

            _model.ExchangeDeclare(_exchange, type);
            _model.QueueDeclare(_queue, durable, exclusive, autoDelete, arguments: null);
            _model.QueueBind(_queue, _exchange, routingKey);
            _model.BasicQos(0, 10, false);
        }

        public void Subscribe(Func<string, IDictionary<string, object>, bool> callback)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = callback.Invoke(message, e.BasicProperties.Headers);
                if (success)
                {
                    _model.BasicAck(e.DeliveryTag, true);
                }

            };

            _model.BasicConsume(_queue, false, consumer);
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