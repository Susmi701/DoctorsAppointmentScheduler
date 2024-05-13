using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using static DoctorsScheduler.Services.CommonServices;

namespace DoctorsScheduler.Services
{
    public class DoctorUseServices
    {
        private readonly HttpClient _httpClient;
        public ILogger<DoctorUseServices> _logger { get; }
        public DoctorUseServices(ILogger<DoctorUseServices> logger, IOptions<ApiOption> medicalstaffApiOptions)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"{medicalstaffApiOptions.Value.BaseUrl}/MedicalStaff/") };
            _logger = logger;
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>?> GetRequestFromPatient(int id)
        {
            try
            {
                var Endpoint = $"GetRequestFromPatient/{id}";
                using (var response = await _httpClient.GetAsync(Endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>();
                        return content;
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
        public async Task<bool> UpdateBookingStatus(int appointmentId)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("UpdateStatus", appointmentId))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<bool> MarkLeave(MarkLeaveDTO markLeave)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("Leave", markLeave))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
                                  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>?> GetTotalAppointmentsAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>($"GetTotalAppointments/{id}");               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>?> GetTotalAppointmentsOverAPeriodAsync(DateDTO requestDTO,int id) 
        {
            try
            {
                var registerEndpoint = $"GetTotalAppointmentsForDoctorOverAPeriod/{id}";
                using (var response = await _httpClient.PostAsJsonAsync(registerEndpoint, requestDTO))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return Enumerable.Empty<AppointmentHistoryDTO>();
                    }
                    var content = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>();
                    return content;
                }
                   
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<MedicalStaffDTO?> GetDoctorsAsync(int id)
        {
            try
            {
                var registerEndpoint = $"GetDoctor/{id}";
                return await _httpClient.GetFromJsonAsync<MedicalStaffDTO>(registerEndpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<ApiResponse<bool>> EditDoctorAsync(int id,MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                var Endpoint = $"EditDoctor{id}";
                using (var response = await _httpClient.PostAsJsonAsync(Endpoint, medicalStaffDTO))
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
        public async Task<IEnumerable<CountDTO>?> PatientDetailsAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CountDTO>>($"GetPatientDetails/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<DateTime>?> GetMarkedLeaveAsync(int id)
        {
            try
            {
                var Endpoint = $"GetMarkedLeave/{id}";
                using (var response = await _httpClient.GetAsync(Endpoint))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return Enumerable.Empty<DateTime>();
                    }
                    var content = await response.Content.ReadFromJsonAsync<IEnumerable<DateTime>>();
                    return content;
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
