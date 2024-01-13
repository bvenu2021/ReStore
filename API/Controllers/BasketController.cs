using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
    private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            Basket basket = await RetrieveBasket();
            if (basket == null) basket = CreateBasket();

            if (basket == null) return NotFound();
            
            return MapToBasketDto(basket);
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToBasket(int productId, int quantity)
        {
            // Create or get Basket
            Basket basket = await RetrieveBasket();
            if(basket == null) basket = CreateBasket();

            // get Product
            var product = await _context.Products.FindAsync(productId);
            if(product == null) return NotFound();

            // Add Item
            basket.AddItem(product, quantity);

            // Save changes
            var result = _context.SaveChanges() > 0;

            if(result) return CreatedAtRoute("GetBasket", MapToBasketDto(basket));

            return BadRequest(new ProblemDetails{Title = "Failed to save into Database!"});
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // Get Basket
            var basket = await RetrieveBasket();

            if(basket == null) return NotFound();

            // remove item or reduce quantity
            basket.RemoveItem(productId, quantity);
            
            // Save changes
            var result = await _context.SaveChangesAsync() > 0;
            if(result) return Ok();

            return BadRequest(new ProblemDetails{Title = "Failed to remove item from the basket"});
        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions{IsEssential = true, Expires = DateTime.Now.AddDays(30)};

            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            
            var basket = new Basket{BuyerId = buyerId};
            _context.Baskets.Add(basket);
            return basket;
        }

        private async Task<Basket> RetrieveBasket()
        {
            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private BasketDto MapToBasketDto(Basket basket)
        {
            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.Product.Id,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Brand = item.Product.Brand,
                    Type = item.Product.Type,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}