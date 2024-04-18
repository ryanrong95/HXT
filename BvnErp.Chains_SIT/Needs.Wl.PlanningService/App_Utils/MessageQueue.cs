using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    /// <summary>
    /// RabbitMQ连接工厂
    /// </summary>
    public class RabbitMQConnFactory
    {
        static object locker = new object();
        static ConnectionFactory current;

        public static ConnectionFactory Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ConnectionFactory()
                            {
                                UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"],
                                Password = System.Configuration.ConfigurationManager.AppSettings["Password"],
                                HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"],
                                Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]),
                                VirtualHost = System.Configuration.ConfigurationManager.AppSettings["VirtualHost"],
                                Protocol = Protocols.DefaultProtocol
                            };
                        }
                    }
                }              
                return current;
            }
        }
    }

    /// <summary>
    /// RabbitMQ是实现了高级消息队列协议（AMQP）的开源消息代理软件
    /// </summary>
    public class RabbitMQ
    {
        /// <summary>
        /// 消息队列名称
        /// </summary>
        private string Queue;

        /// <summary>
        /// 路由键
        /// </summary>
        private string RoutingKey;

        public RabbitMQ(string queue, string routingKey = "")
        {
            this.Queue = queue;
            this.RoutingKey = routingKey;
        }

        /// <summary>
        /// 生产消息
        /// </summary>
        /// <param name="message">入列消息，取决于具体业务场景，需要从外部传入</param>
        /// <returns></returns>
        public ResponseCode Publish(string message)
        {
            try
            {
                //1. 实例化连接工厂
                var factory = RabbitMQConnFactory.Current;

                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        //4. 声明队列
                        channel.QueueDeclare(queue: Queue,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        //设置IbasicProperties.SetPersistent属性值为true来标记消息持久化
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //5. 构建字节数据包
                        var body = Encoding.UTF8.GetBytes(message);

                        //6. 发送数据包
                        channel.BasicPublish(exchange: "",
                                             routingKey: RoutingKey,
                                             basicProperties: properties,
                                             body: body);
                    }
                }

                return ResponseCode.Success;
            }
            catch (Exception ex)
            {
                return ResponseCode.Fail;
            }
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="Action">消息出列后需要执行的业务逻辑处理，根据不同业务场景进行回调</param>
        /// <returns></returns>
        public ResponseCode Consume(Action<string> Action)
        {
            try
            {
                //1. 实例化连接工厂
                var factory = RabbitMQConnFactory.Current;

                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {                  
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        //4. 声明队列
                        channel.QueueDeclare(queue: Queue,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        //设置prefetchCount为1来告知RabbitMQ在未收到消费端的消息确认时，不再分发消息
                        channel.BasicQos(prefetchSize: 0,
                                         prefetchCount: 1,
                                         global: false);

                        //5. 构造消费者实例
                        var consumer = new EventingBasicConsumer(channel);

                        //6. 绑定消息接收后的事件委托
                        consumer.Received += (model, ea) =>
                        {
                            var message = Encoding.UTF8.GetString(ea.Body);
                            //处理具体的业务逻辑
                            Action(message);
                            // 发送信息确认信号（手动信息确认）
                            //channel.BasicAck(ea.DeliveryTag, false);
                        };

                        //7. 启动消费者
                        channel.BasicConsume(queue: Queue,
                                             autoAck: true,
                                             consumer: consumer);
                        //TODO:如何让程序在此处暂停，等待consumer.Received全部执行完毕，再释放channel?
                        //Console.ReadKey();
                        Thread.Sleep(3*60*1000);
                    }
                }

                return ResponseCode.Success;
            }
            catch (Exception ex)
            {
                return ResponseCode.Fail;
            }
        }
    }
}
