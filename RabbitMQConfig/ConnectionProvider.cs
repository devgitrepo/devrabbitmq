﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConfig
{
    public class ConnectionProvider: IConnectionProvider
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private bool _disposed;

        public ConnectionProvider(string uriValue)
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uriValue)
            };
            _connection = _factory.CreateConnection();
        }

        public IConnection Connection ()
        {
            return _connection;
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
                _connection?.Close();

            _disposed = true;
        }
    }
}
