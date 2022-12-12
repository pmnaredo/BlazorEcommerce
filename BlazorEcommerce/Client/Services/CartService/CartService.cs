using BlazorEcommerce.Shared;
using BlazorEcommerce.Shared.DTOs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private ILocalStorageService _localStorage;
        private HttpClient _http;
        private AuthenticationStateProvider _authStateProvider;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, HttpClient http,
            AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _http = http;
            _authStateProvider = authStateProvider;
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await GetOrCreateCart();

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
            }
            
            await GetCartItemsCount();
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

        //public async Task<List<CartItem>> GetCartItems()
        //{
        //    await GetCartItemsCount();
        //    var cart = await GetOrCreateCart();

        //    return cart;
        //}

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if (await IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cartItems == null)
                    return new List<CartProductResponse>();

                var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
                return cartProducts.Data;
            }
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if (await IsUserAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
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
                }
            }
            
            //await GetCartItemsCount();
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if (await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _http.PutAsJsonAsync("api/cart/update-quantity", request);

            }
            else
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

        public  async Task StoreCartItems(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (localCart == null)
                return;

            await _http.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task GetCartItemsCount()
        {
            if (await IsUserAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                var count = cart != null ? cart.Count : 0;
                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }

            OnChange.Invoke();
        }
    }
}
