using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConfig
{
    public interface ISubscriber
    {
        void Subscribe(Func<string, IDictionary<string, object>, bool> callback);
    }
}
