using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using static DoctorsScheduler.Services.CommonServices;

namespace DoctorsScheduler.Services
{
    public class MedicalStaffServices
    {
        private readonly HttpClient _httpClient;
        public ILogger<MedicalStaffServices> _logger { get; }
        public MedicalStaffServices(ILogger<MedicalStaffServices> logger, IOptions<ApiOption> medicalstaffApiOptions)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"{medicalstaffApiOptions.Value.BaseUrl}/MedicalStaff/") };
            _logger = logger;
        }
        public async Task<MedicalStaffLoginDTO?> LoginAsync(AuthenticationDTO loginModel)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("Login", loginModel))
                {
                    var test = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        return await JsonSerializer.DeserializeAsync<MedicalStaffLoginDTO>(responseStream, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    return null;
                }
                    

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ResetPasswordDTO model)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("ForgotPassword", model))
                {
                    var message = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = true,
                            StatusCode = (int)response.StatusCode,
                            Message = message
                        };
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            _logger.LogError(message);
                        }
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            StatusCode = (int)response.StatusCode,
                            Message = message
                        };
                    }
                }
                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO loginModel)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("ResetPassword", loginModel))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                    return true;
                }
                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
    }
 
}
