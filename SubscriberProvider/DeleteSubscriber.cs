using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SubscriberProvider
{
    public class DeleteSubscriber
    {
        private readonly ILogger<DeleteSubscriber> _logger;
        private readonly DataContext _context;

        public DeleteSubscriber(ILogger<DeleteSubscriber> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteSubscriber")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "subscribers/{email}")] HttpRequest req,
            string email)
        {
            _logger.LogInformation("Deleting subscriber with email {Email}", email);

            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
                if (subscriber == null)
                {
                    _logger.LogWarning("Subscriber with email {Email} not found", email);
                    return new NotFoundResult();
                }

                _context.Subscribers.Remove(subscriber);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Subscriber with email {Email} deleted successfully", email);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscriber with email {Email}", email);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}