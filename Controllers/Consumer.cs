using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using problem4.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Consumer : BackgroundService
{
    private readonly ILogger _logger;
    private IConnection _connection;
    private IModel _channel;

    private readonly BtcContext _context;

    public Consumer(BtcContext context)
    {
        _context = context;
    }

    public Consumer(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<Consumer>();
        InitRabbitMQ();
    }

    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory
        {

            HostName = "localhost",
            Port = 31091
            //HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
            //Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))

        };

        // create connection  
        _connection = factory.CreateConnection();

        // create channel  
        _channel = _connection.CreateModel();

        //_channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
        _channel.QueueDeclare("info", false, false, false, null);
        // _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
        // _channel.BasicQos(0, 1, false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            // received message  
            var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

            // handle the received message  
            HandleMessage(content);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume("info", false, consumer);
        return Task.CompletedTask;
    }

    private void HandleMessage(string content)
    {
        // we just print this message and save in db  
        _logger.LogInformation($"consumer received {content}");
       
        //_context.BtcPriceInfo.Add(BtcPriceInfo);
        //_context.SaveChangesAsync();

        //return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
