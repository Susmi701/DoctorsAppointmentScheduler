using DoctorsAppointmentScheduler.DTO;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static DoctorsScheduler.Services.CommonServices;

namespace DoctorsScheduler.Services
{
    public class AdminUserServices
    {
        private readonly HttpClient _httpClient;
        public ILogger<DoctorUseServices> _logger { get; }
        public AdminUserServices(ILogger<DoctorUseServices> logger, IOptions<ApiOption> patientApiOptions)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"{patientApiOptions.Value.BaseUrl}/MedicalStaff/") };
            _logger = logger;
        }

        public async Task<ApiResponse<AuthenticationDTO>> RegisterDoctorAsync(MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("RegisterDoctor", medicalStaffDTO))
                {
                    var message = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using var content = await response.Content.ReadAsStreamAsync();
                        var data = await JsonSerializer.DeserializeAsync<AuthenticationDTO>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new ApiResponse<AuthenticationDTO>
                        {
                            Success = true,
                            Data = data
                        };
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            _logger.LogError(message);
                        }

                        return new ApiResponse<AuthenticationDTO>
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
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllSpecialityAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<DoctorSpecialityDTO>>("GetSpeciality");
                if (response != null)
                {
                    return response;
                }
                else
                {
                     _logger.LogError("Failed to retrieve doctors from the API");
                    return Enumerable.Empty<DoctorSpecialityDTO>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("DeleteDoctor", doctorId))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                        return false;
                }                  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<DoctorSpecialityDTO>>("GetAllDoctors");
                if (response != null)
                {
                    return response;
                }
                else
                {
                     _logger.LogError("Failed to retrieve doctors from the API");
                    return Enumerable.Empty<DoctorSpecialityDTO>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetAllTotalAppointmentsAsync()
        {
            try
            {
                using (var response = await _httpClient.GetAsync("GetAllAppointments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>();
                        return content;
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve doctors from the API");
                        return Enumerable.Empty<AppointmentHistoryDTO>();
                    }
                }                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetTotalAppointmentsOverAPeriodAsync(DateDTO date)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("GetAppointmentsWithStatus", date))
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
        public async Task<IEnumerable<CountDTO>> SpecialityWithLeastPatientAsync()
        {
            try
            {
                using (var response = await _httpClient.GetAsync("GetSpecialitiesWithLeastPatients"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<CountDTO>>();
                        return content;
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve doctors from the API");
                        return Enumerable.Empty<CountDTO>();
                    }
                }
 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<CountDTO>> DoctorsWithLeastPatientAsync()
        {
            try
            {
                using (var response = await _httpClient.GetAsync("GetDoctorsByPatientCount"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<CountDTO>>();
                        return content;
                    }
                    return Enumerable.Empty<CountDTO>();
                }                  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> DoctorTotalAppointmentsAsync(int id)
        {
            try
            {
                var registerEndpoint = $"GetTotalAppointments/{id}";
                using (var response = await _httpClient.GetAsync(registerEndpoint))
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
        public async Task<IEnumerable<AppointmentHistoryDTO>> DoctorTotalAppointmentsOverAPeriodAsync(DateDTO requestDTO,int id)
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
        public async Task<MedicalStaffDTO> GetDoctorsForAdminAsync(int id)
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
        public async Task<ApiResponse<bool>> EditDoctorForAdminAsync(int id, MedicalStaffDTO medicalStaffDTO)
        {
            try
            {
                var registerEndpoint = $"EditDoctorForAdmin/{id}";
                using (var response = await _httpClient.PostAsJsonAsync(registerEndpoint, medicalStaffDTO))
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
        public async Task<IEnumerable<CountDTO>> PatientDetailsAsync()
        {
            try
            {
                using (var response = await _httpClient.GetAsync("GetAllPatient"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return Enumerable.Empty<CountDTO>();
                    }

                    var content = await response.Content.ReadFromJsonAsync<IEnumerable<CountDTO>>();
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
