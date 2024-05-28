using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SubscriberProvider
{
    public class DeleteSubscriber(ILogger<DeleteSubscriber> logger, DataContext context)
    {
        private readonly ILogger<DeleteSubscriber> _logger = logger;
        private readonly DataContext _context = context;


        [Function("DeleteSubscriber")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "subscribers/{id:int}")] HttpRequest req, int id)
        {
            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
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
                _logger.LogError(ex, "Error deleting subscriber with id {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
