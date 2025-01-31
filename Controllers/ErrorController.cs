using erpv0._1.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace erpv0._1.Controllers
{
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [Route("{statusCode:int}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode,
                ErrorTitle = GetStatusCodeTitle(statusCode),
                ErrorMessage = GetStatusCodeMessage(statusCode)
            };

            _logger.LogError("HTTP {StatusCode} error occurred. Path: {Path}", statusCode, Request.Path);
            return View("Error", errorViewModel);
        }

        [Route("")]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorTitle = "Application Error",
                ErrorMessage = _environment.IsDevelopment()
                    ? exception?.Message ?? "An error occurred."
                    : "An error occurred while processing your request.",
                StackTrace = _environment.IsDevelopment() ? exception?.StackTrace : null,
                InnerException = _environment.IsDevelopment() ? exception?.InnerException?.Message : null
            };

            if (exception != null)
            {
                _logger.LogError(exception, "An unhandled exception occurred at {Time}. Path: {Path}",
                    DateTime.UtcNow, Request.Path);
            }

            return View("Error", errorViewModel);
        }

        private string GetStatusCodeTitle(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Page Not Found",
                500 => "Server Error",
                503 => "Service Unavailable",
                _ => "Error"
            };
        }

        private string GetStatusCodeMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "The request could not be understood by the server.",
                401 => "Authentication is required and has failed or has not been provided.",
                403 => "You do not have permission to access this resource.",
                404 => "The requested page could not be found.",
                500 => "An internal server error occurred.",
                503 => "The service is temporarily unavailable.",
                _ => "An error occurred while processing your request."
            };
        }
    }
}