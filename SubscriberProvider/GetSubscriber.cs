using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SubscriberProvider
{
    public class GetSubscriber
    {
        private readonly ILogger<GetSubscriber> _logger;
        private readonly DataContext _context;

        public GetSubscriber(ILogger<GetSubscriber> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetSubscriber")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "subscribers/{id:int}")] HttpRequest req,
            int id)
        {
            _logger.LogInformation("Getting subscriber with id {Id}", id);

            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
                if (subscriber == null)
                {
                    _logger.LogWarning("Subscriber with id {Id} not found", id);
                    return new NotFoundResult();
                }

                return new OkObjectResult(subscriber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriber with id {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}