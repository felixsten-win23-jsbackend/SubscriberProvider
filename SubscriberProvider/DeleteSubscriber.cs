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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "subscribers")] HttpRequest req)
        {
            string email = req.Query["email"];

            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult("Email is required.");
            }

            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
                if (subscriber == null)
                {
                    return new NotFoundResult();
                }

                _context.Subscribers.Remove(subscriber);
                await _context.SaveChangesAsync();

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