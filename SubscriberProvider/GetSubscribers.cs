using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SubscriberProvider
{
    public class GetSubscribers(ILogger<GetSubscribers> logger, DataContext context)
    {
        private readonly ILogger<GetSubscribers> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetSubscribers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {

            var subscribers = await _context.Subscribers.ToListAsync();
            return new OkObjectResult(subscribers);
        }
    }
}
