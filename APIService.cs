using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

namespace EternalReturnOverlay
{
    public class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = APIKey.GetApiKey();
        
        public APIService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey); // 헤더에 API 키 추가
        }

        public async Task<int> GetSeasonIDAsync()
        {
            string url = "https://open-api.bser.io/v2/data/Season";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            int seasonId = 0;

            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = doc.RootElement;
                JsonElement dataArray = root.GetProperty("data");

                foreach (JsonElement season in dataArray.EnumerateArray())
                {
                    if (season.GetProperty("isCurrent").GetInt32() == 1)
                    {
                        seasonId = season.GetProperty("seasonID").GetInt32();
                        break;
                    }
                }
            }

            return seasonId;
        }

        public async Task<int> GetUserNumAsync(string nickname)
        {
            try
            {
                string url = $"https://open-api.bser.io/v1/user/nickname?query={nickname}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("user", out JsonElement user))
                    {
                        int userNum = user.GetProperty("userNum").GetInt32();
                        return userNum;
                    }
                    else
                    {
                        throw new UserNotFoundException("존재하지 않는 닉네임입니다.");
                    }
                }
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                throw new UserNotFoundException("존재하지 않는 닉네임입니다.");
            }
        }

        public async Task GetUserStatsAsync(int userNum, int seasonId)
        {
            string url = $"https://open-api.bser.io/v1/user/stats/{userNum}/{seasonId}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = doc.RootElement;
                JsonElement userStatsArray = root.GetProperty("userStats");

                if (userStatsArray.ValueKind == JsonValueKind.Array && userStatsArray.GetArrayLength() > 0)
                {
                    JsonElement userStats = userStatsArray[0];

                    // UserDataManager를 통해 UserData 업데이트
                    UserDataManager.Instance.UserData.RP = GetPropertyIfExists(userStats, "mmr", 0);
                    UserDataManager.Instance.UserData.Ranking = GetPropertyIfExists(userStats, "rank", 0);
                    UserDataManager.Instance.UserData.TotalGames = GetPropertyIfExists(userStats, "totalGames", 0);
                    UserDataManager.Instance.UserData.RankSize = GetPropertyIfExists(userStats, "rankSize", 0);
                    UserDataManager.Instance.UserData.Top1 = GetPropertyIfExists(userStats, "top1", 0.0);
                    UserDataManager.Instance.UserData.Top2 = GetPropertyIfExists(userStats, "top2", 0.0);
                    UserDataManager.Instance.UserData.Top3 = GetPropertyIfExists(userStats, "top3", 0.0);
                    UserDataManager.Instance.UserData.AverageRank = GetPropertyIfExists(userStats, "averageRank", 0.0);
                    UserDataManager.Instance.UserData.RankPercent = (double)UserDataManager.Instance.UserData.Ranking / UserDataManager.Instance.UserData.RankSize * 100;
                }

                else
                {
                    // 유저 데이터가 없을 경우 초기화
                    UserDataManager.Instance.UserData.RP = 0;
                    UserDataManager.Instance.UserData.Ranking = 0;
                    UserDataManager.Instance.UserData.TotalGames = 0;
                    UserDataManager.Instance.UserData.RankPercent = 0.0;
                    UserDataManager.Instance.UserData.Top1 = 0.0;
                    UserDataManager.Instance.UserData.Top2 = 0.0;
                    UserDataManager.Instance.UserData.Top3 = 0.0;
                    UserDataManager.Instance.UserData.AverageRank = 0.0;
                }
            }
        }

        private int GetPropertyIfExists(JsonElement element, string propertyName, int defaultValue)
        {
            if (element.TryGetProperty(propertyName, out JsonElement property))
            {
                return property.GetInt32();
            }
            return defaultValue;
        }

        private double GetPropertyIfExists(JsonElement element, string propertyName, double defaultValue)
        {
            if (element.TryGetProperty(propertyName, out JsonElement property))
            {
                return property.GetDouble();
            }
            return defaultValue;
        }
    }
}