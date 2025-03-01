



using System.Text;
using System.Text.Json;

namespace MauiTrain
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly HttpClient _client;
        private readonly AppState _appState;
        private string jwt;
        public MainPage(HttpClient client, AppState appState)
        {
            _client = client;
            _appState = appState;
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = NameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Email and password are required.", "OK");
                return;
            }

            bool isValid = Validator.Validator.ValidateEmail(email);
            // Simple email validation
            if (isValid)
            {
                // TODO: Handle login logic (e.g., authentication API call)
                bool success = await SendLoginRequest(email, password);
                if (success)
                {

                    await DisplayAlert("Success", $"Logged in as {email}", "OK");
                    return;
                }

            }

            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;


        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            string email = NameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Email and password are required.", "OK");
                return;
            }

            bool success = await SendSignupRequest(email, password);
            if (success)
            {
                await DisplayAlert("Success", $"Logged in as {email}", "OK");
                return;
            }
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }

        private async Task<bool> SendLoginRequest(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            string json = JsonSerializer.Serialize(loginData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                string urlHttp = "http://localhost:5080/";
                string urlAsp = "http://localhost:5106/api/auth/login";
                HttpResponseMessage response = await _client.PostAsync(urlHttp + "login", content);
                // Read response content as a string
                string responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseJson);
                // Deserialize response JSON
                 using var doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))

                    _appState.StoreJwt(responseJson); // Extract token
                Console.WriteLine(jwt);
                if (string.IsNullOrEmpty(_appState.Jwt))
                {

                    await DisplayAlert("Error", "Token not found in response.", "OK");
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Request failed: {ex.Message}", "OK");
                return false;
            }
        }

        private async Task<bool> SendSignupRequest(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            string json = JsonSerializer.Serialize(loginData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                string urlHttp = "http://localhost:5080/";
                string urlAsp = "http://localhost:5106/api/auth/login";
                HttpResponseMessage response = await _client.PostAsync(urlHttp + "signup", content);

                // Read response content as a string
                string responseJson = await response.Content.ReadAsStringAsync();

                // Deserialize response JSON
                using var doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
                {
                    jwt = tokenElement.GetString(); // Extract token
                    return false;
                }


                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Request failed: {ex.Message}", "OK");
                return false;
            }
        }
    }

}
