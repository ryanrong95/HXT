using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class DecApplyQueue
    {
        public DecApplyQueue()
        {
            this.threadStart();
        }


        private void threadStart()
        {
            ThreadStart threadStart = new ThreadStart(ReadQueue);
            Thread thread = new Thread(threadStart);
            thread.Start();

            //Thread thread = new Thread(threadStart);
            //thread.IsBackground = true;
            //thread.Start();
            //while (true)
            //{
                
            //}
        }

        private void ReadQueue()
        {
            while (true)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    factory.AutomaticRecoveryEnabled = true;
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "DecHead",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                        };
                        channel.BasicConsume(queue: "DecHead",
                                             autoAck: true,
                                             consumer: consumer);

                    }
                    //var factory = new ConnectionFactory() { HostNames = "localhost" };

                    //using (var connection = factory.CreateConnection())
                    //using (var channel = connection.CreateModel())
                    //{
                    //    //factory.AutomaticRecoveryEnabled = true;
                    //    channel.QueueDeclare(queue: "DecHead",
                    //                         durable: true,
                    //                         exclusive: false,
                    //                         autoDelete: false,
                    //                         arguments: null);

                    //    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


                    //    var consumer = new EventingBasicConsumer(channel);
                    //    consumer.Received += (model, ea) =>
                    //    {
                    //        var body = ea.Body;
                    //        var message = Encoding.UTF8.GetString(body);
                    //       // var dec = Tool.Current.Customs.DecHeads.Where(item => item.ID == message).FirstOrDefault();
                    //    //if (dec != null)
                    //    //{
                    //    //    dec.Declare(); //申报
                    //    //}
                    //    };
                    //    channel.BasicConsume(queue: "hello",
                    //                autoAck: true,
                    //                consumer: consumer);
                    //}
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void addRabbitMQ(string ID)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "DecHead",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    string message = ID;
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "DecHead",
                                         basicProperties: properties,
                                         body: body);
                }
            }
        }
    }
}
