﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes.Internals.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Session04.MassTest.Contracts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Session04.MassTest.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<OrderRegistered> requestClient;

        public OrderController(ILogger<OrderController> logger, IRequestClient<OrderRegistered> requestClient)
        {
            _logger = logger;
            this.requestClient = requestClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id = 2, string customerNumber = "12")
        {

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("rabbitmq://localhost");
            });

            await bus.StartAsync();

            var orderRegistered = new OrderRegistered
            {
                OrderId = id,
                CustomerNumber = customerNumber
            };
            var res = bus.CreateRequestClient<OrderRegistered>();
            var (accepted, rejected) = await requestClient.GetResponse<OrderAccepted, OrderRejected>(orderRegistered);
            
            if (accepted.IsCompleted)
                return Ok(await accepted);
            return Ok(await rejected);
        }
    }
}
