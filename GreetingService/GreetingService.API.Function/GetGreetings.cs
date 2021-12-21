using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function
{
    public class GetGreetings
    {
        private readonly ILogger<GetGreetings> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;

        public GetGreetings(ILogger<GetGreetings> log, IGreetingRepository greetingRepository, IAuthHandler authHandler)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
        }

        [FunctionName("GetGreetings")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authHandler.IsAuthorized(req))
                return new UnauthorizedResult();

            var greetings = _greetingRepository.Get();
            
            return new OkObjectResult(greetings);
        }
    }
}

