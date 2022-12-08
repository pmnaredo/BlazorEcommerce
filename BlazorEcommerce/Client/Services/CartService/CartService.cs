using BlazorEcommerce.Shared;
using BlazorEcommerce.Shared.DTOs;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private ILocalStorageService _localStorage;
        private HttpClient _http;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            List<CartItem>? cart = await GetOrCreateCart();

            var sameItem = cart.Find(item => item.ProductId == cartItem.ProductId
                && item.ProductTypeId == cartItem.ProductTypeId);
            if (sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        private async Task<List<CartItem>> GetOrCreateCart()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await GetOrCreateCart();

            return cart;
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts =
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
            return cartProducts.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(item => item.ProductId == productId
                && item.ProductTypeId == productTypeId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(item => item.ProductId == product.ProductId
                && item.ProductTypeId == product.ProductTypeId);
            if (cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorage.SetItemAsync("cart", cart);
                //OnChange.Invoke();
            }
        }
    }
}
