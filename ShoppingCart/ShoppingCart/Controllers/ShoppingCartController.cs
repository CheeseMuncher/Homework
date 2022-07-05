using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : Controller
    {
        private readonly ILogger _logger;
        private readonly IShoppingCartCalculator _shoppingCartCalculator;

        public ShoppingCartController(ILogger logger, IShoppingCartCalculator shoppingCartCalculator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shoppingCartCalculator = shoppingCartCalculator ?? throw new ArgumentNullException(nameof(shoppingCartCalculator));
            _logger.Debug("ShoppingCartController built");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ShoppingCartRequest request)
        {
            return Ok($"{_shoppingCartCalculator.Total(request)}");
        }
    }
}