using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConfig
{
    public interface IConnectionProvider
    {
        IConnection Connection();
    }
}
