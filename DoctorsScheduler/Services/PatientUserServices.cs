using DoctorsAppointmentScheduler.DTO;
using DoctorsScheduler.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static DoctorsScheduler.Services.CommonServices;

namespace DoctorsScheduler.Services
{
    public class PatientUserServices
    {
        private readonly HttpClient _httpClient;
        public ILogger<PatientUserServices> _logger { get; }
        public PatientUserServices(ILogger<PatientUserServices> logger, IOptions<ApiOption> patientApiOptions)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"{patientApiOptions.Value.BaseUrl}/Patient/") };
            _logger = logger;
        }
        public async Task<ApiResponse<int>> LoginAsync(AuthenticationDTO loginModel)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("Login", loginModel))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        var responseData = await JsonSerializer.DeserializeAsync<int>(responseStream, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return new ApiResponse<int>
                        {
                            Success = true,
                            StatusCode = (int)response.StatusCode,
                            Data = responseData
                        };
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            _logger.LogError(errorMessage);
                        }
                        return new ApiResponse<int>
                        {
                            Success = false,
                            StatusCode = (int)response.StatusCode,
                            Message = errorMessage
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
        public async Task<ApiResponse<bool>> RegisterAsync(PatientDTO patient)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("RegisterPatient", patient))
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
        public async Task<IEnumerable<DoctorSpecialityDTO>> GetAllDoctorsAsync(int specialtyId)
        {
            try
            {
                var Endpoint = $"GetAllDoctors/{specialtyId}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<DoctorSpecialityDTO>>(Endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int id)
        {
            try
            {
                var Endpoint = $"GetDateAvailability/{id}";
                using (var response = await _httpClient.GetAsync(Endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<DateTime>>();
                        return content;
                    }
                    _logger.LogError("Failed to retrieve available dates from the API");
                    return Enumerable.Empty<DateTime>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<IEnumerable<DateTime>> GetAvailableTimeSlotAsync(DateTime selectedDate, int selectedDoctorId)
        {
            try
            {
                var timeSlot = GenerateTimeSlotsForDate(selectedDate);
                var timeeslotDTO = new TimeSlotRequestDTO
                {
                    DoctorId = selectedDoctorId,
                    Date = selectedDate,
                    TimeSlots = timeSlot
                };
                using (var response = await _httpClient.PostAsJsonAsync("GetTimeSlotAvailability", timeeslotDTO))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<DateTime>>();
                        return content;
                    }
                    _logger.LogError("Failed to retrieve available doctors from the API");
                    return Enumerable.Empty<DateTime>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<ApiResponse<bool>> BookAppointmentAsync(AppointmentDTO appointment)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync("BookAppointment", appointment))
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
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetAppointmentHistoryAsync(int id)
        {
            try
            {
                var Endpoint = $"GetAppointmentHistory/{id}";
                using (var response = await _httpClient.GetAsync(Endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>();
                        return content;
                    }
                    _logger.LogError("Failed to retrieve doctors from the API");
                    return Enumerable.Empty<AppointmentHistoryDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }

        }
        public async Task<IEnumerable<AppointmentHistoryDTO>> GetBookedHistoryAsync(int id)
        {
            try
            {
                var Endpoint = $"GetBookedHistory/{id}";
                using (var response = await _httpClient.GetAsync(Endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentHistoryDTO>>();
                        return content;
                    }
                    _logger.LogError("Failed to retrieve doctors from the API");
                    return Enumerable.Empty<AppointmentHistoryDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public static List<DateTime> GenerateTimeSlotsForDate(DateTime selectedDate)
        {
            DateTime startTime = selectedDate.Date.AddHours(9); // Set the start time to 9:00 AM of the selected date
            DateTime endTime = selectedDate.Date.AddHours(14); // Set the end time to 2:00 PM of the selected date

            List<DateTime> timeSlots = new List<DateTime>();

            while (startTime < endTime)
            {
                timeSlots.Add(startTime);
                startTime = startTime.AddMinutes(15);
            }

            return timeSlots;
        }
        public async Task<EditUserDTO> GetPatientAsync(int id)
        {
            try
            {
                var Endpoint = $"GetPatient/{id}";
                return await _httpClient.GetFromJsonAsync<EditUserDTO>(Endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw new Exception("Error occurred while processing the request");
            }
        }
        public async Task<ApiResponse<bool>> EditPatientAsync(int id, EditUserDTO usersDTO)
        {
            try
            {
                var Endpoint = $"EditPatient/{id}"; // Corrected Endpoint format
                using (var response = await _httpClient.PostAsJsonAsync(Endpoint, usersDTO))
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

    }

}
