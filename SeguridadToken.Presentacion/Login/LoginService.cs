﻿using Newtonsoft.Json;
using SeguridadToken.Dto;
using System.Net.Http.Headers;
using System.Text;

namespace SeguridadToken.Presentacion.Login;

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LoginService> _logger;

    public LoginService(HttpClient httpClient, ILogger<LoginService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<UserDto> ActionLogin(LoginDto loginDto)
    {
        string JsonData = JsonConvert.SerializeObject(loginDto);
        StringContent content = new StringContent(JsonData, Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.PostAsJsonAsync("Login/Login", content);
        response.EnsureSuccessStatusCode();

        var user = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

        if (user == null)
        {
            _logger.LogError("Error" + "Usuario null");
        }

        return user;
    }
}
