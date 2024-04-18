using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MQMethod
    {
        private string UserName { get; set; }
        private string Password { get; set; }
        private string HostName { get; set; }
        private int Port { get; set; }
        private string VirtualHost { get; set; }

        public MQMethod(string UserName, string Password, string HostName, int Port, string VirtualHost)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.HostName = HostName;
            this.Port = Port;
            this.VirtualHost = VirtualHost;
        }
        public bool ProduceInside(string PostID, ref string msg)
        {
            try
            {
                //var factory = new ConnectionFactory() { HostName = HostAddress };
                var factory = new ConnectionFactory();
                factory.UserName = this.UserName;
                factory.Password = this.Password;
                factory.HostName = this.HostName;
                factory.Port = this.Port;
                factory.VirtualHost = this.VirtualHost;
                factory.Protocol = Protocols.DefaultProtocol;

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ForInsideIFOrder",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = PostID;
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "ForInsideIFOrder",
                                         basicProperties: properties,
                                         body: body);
                }
                msg = "下单成功";
                return true;
            }
            catch (Exception ex)
            {
                msg = "下单失败";
                return false;
            }
        }

        public bool ProduceIcgoo(string PostID,ref string msg)
        {
            try
            {
                //var factory = new ConnectionFactory() { HostName = "localhost" };
                var factory = new ConnectionFactory();
                factory.UserName = this.UserName;
                factory.Password = this.Password;
                factory.HostName = this.HostName;
                factory.Port = this.Port;
                factory.VirtualHost = this.VirtualHost;
                factory.Protocol = Protocols.DefaultProtocol;

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ForIcgooIFOrder",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = PostID;
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "ForIcgooIFOrder",
                                         basicProperties: properties,
                                         body: body);
                }
                msg = "下单成功";
                return true;
            }
            catch (Exception ex)
            {
                msg = "下单失败";
                return false;
            }
        }

        public bool ProduceIcgooInXDT(string PostID, ref string msg)
        {
            try
            {
                //var factory = new ConnectionFactory() { HostName = "localhost" };
                var factory = new ConnectionFactory();
                factory.UserName = this.UserName;
                factory.Password = this.Password;
                factory.HostName = this.HostName;
                factory.Port = this.Port;
                factory.VirtualHost = this.VirtualHost;
                factory.Protocol = Protocols.DefaultProtocol;

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ForIcgooInXDTIFOrder",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = PostID;
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "ForIcgooInXDTIFOrder",
                                         basicProperties: properties,
                                         body: body);
                }
                msg = "下单成功";
                return true;
            }
            catch (Exception ex)
            {
                msg = "下单失败";
                return false;
            }
        }

    }
}
