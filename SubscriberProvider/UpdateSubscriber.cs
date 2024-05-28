using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SubscriberProvider
{
    public class UpdateSubscriber(ILogger<UpdateSubscriber> logger, DataContext context)
    {
        private readonly ILogger<UpdateSubscriber> _logger = logger;
        private readonly DataContext _context = context;


        [Function("UpdateSubscriber")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "subscribers/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Received request to update subscriber with id: {Id}", id);
            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
                if (subscriber == null)
                {
                    _logger.LogWarning("Subscriber with id {Id} not found", id);
                    return new NotFoundResult();
                }

                var updatedSubscriber = await JsonSerializer.DeserializeAsync<SubscriberEntity>(req.Body);
                if (updatedSubscriber == null)
                {
                    _logger.LogWarning("Invalid subscriber data provided in request body");
                    return new BadRequestResult();
                }


                subscriber.Email = updatedSubscriber.Email;

                _context.Subscribers.Update(subscriber);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Subscriber with id {Id} updated successfully", id);
                return new OkObjectResult(subscriber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscriber with id {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
