using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Wl.IcgooMQ
{
    public class Receive
    {
        private string UserName { get; set; }
        private string Password { get; set; }
        private string HostName { get; set; }
        private int Port { get; set; }
        private string VirtualHost { get; set; }

        public Receive(string UserName, string Password, string HostName,int Port,string VirtualHost)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.HostName = HostName;
            this.Port = Port;
            this.VirtualHost = VirtualHost;
        }
        public void IcgooConsume()
        {
            //var factory = new ConnectionFactory() { HostName = HostAddress };
            //配置Factory连接
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

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    IcgooCreateOrderSpeed createOrder = new IcgooCreateOrderSpeed();
                    Console.WriteLine("开始处理订单" + message);
                    createOrder.Create(message);
                    Console.WriteLine("订单处理结束" + message);


                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "ForIcgooIFOrder",
                                     autoAck: false,
                                     consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public void InsideConsume()
        {
            //var factory = new ConnectionFactory() { HostName = HostAddress };
            //配置Factory连接
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

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    InsideCreateOrder createOrder = new InsideCreateOrder();
                    Console.WriteLine("开始处理订单" + message);
                    createOrder.Create(message);
                    Console.WriteLine("订单处理结束" + message);


                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "ForInsideIFOrder",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
