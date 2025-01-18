using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EternalReturnOverlay
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private readonly APIService _apiService;
        private bool _isDarkBackground;
        public OverlayWindow()
        {
            InitializeComponent();
            _apiService = new APIService();
            UserDataManager.Instance.SharedTimer.Tick += Timer_Tick;
            UpdateUI();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (UserDataManager.Instance.SharedTimeSpan.TotalSeconds == 0)
            {
                string? nickname = UserDataManager.Instance.UserData.Nickname;
                if (!string.IsNullOrEmpty(nickname))
                {
                    await FetchUserDataAsync(nickname);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public async Task FetchUserDataAsync(string nickname)
        {
            int userNum = await _apiService.GetUserNumAsync(nickname);
            int seasonId = await _apiService.GetSeasonIDAsync();
            await _apiService.GetUserStatsAsync(userNum, seasonId);
            UpdateUI();
        }

        public void UpdateUI()
        {
            UpdateRP();
            UpdateTier();
            UpdateRanking();
            UpdateWinRate();
        }

        private void UpdateRP()
        {
            SetTextProperties(RP, _isDarkBackground);
            RP.Text = "RP " + UserDataManager.Instance.UserData.RP.ToString();
        }

        private void UpdateTier()
        {
            // 티어에 따라 이미지 & 텍스트 변경
            if (UserDataManager.Instance.UserData.RP >= 7700 && UserDataManager.Instance.UserData.Ranking <= 300)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                MainBorder.Background = new SolidColorBrush(Colors.IndianRed);
                Tier.Text = "이터니티";
                _isDarkBackground = false;
            }
            else if (UserDataManager.Instance.UserData.RP >= 7700 && UserDataManager.Instance.UserData.Ranking <= 1000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.White);
                MainBorder.Background = new SolidColorBrush(Colors.WhiteSmoke);
                Tier.Text = "데미갓";
                _isDarkBackground = false;
            }
            else if (UserDataManager.Instance.UserData.RP >= 7000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.LightBlue);
                MainBorder.Background = new SolidColorBrush(Colors.SkyBlue);
                Tier.Text = "미스릴";
                _isDarkBackground = false;
            }
            else if (UserDataManager.Instance.UserData.RP >= 6400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Blue);
                MainBorder.Background = new SolidColorBrush(Colors.DarkBlue);
                Tier.Text = "메테오라이트";
                _isDarkBackground = true;
            }
            else if (UserDataManager.Instance.UserData.RP >= 5000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Purple);
                MainBorder.Background = new SolidColorBrush(Colors.MediumPurple);
                Tier.Text = "다이아몬드";
                _isDarkBackground = true;
            }
            else if (UserDataManager.Instance.UserData.RP >= 3600)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Lime);
                MainBorder.Background = new SolidColorBrush(Colors.LimeGreen);
                Tier.Text = "플래티넘";
                _isDarkBackground = false;
            }
            else if (UserDataManager.Instance.UserData.RP >= 2400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);
                MainBorder.Background = new SolidColorBrush(Colors.LightYellow);
                Tier.Text = "골드";
                _isDarkBackground = false;
            }
            else if (UserDataManager.Instance.UserData.RP >= 1400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Silver);
                MainBorder.Background = new SolidColorBrush(Colors.SlateGray);
                Tier.Text = "실버";
                _isDarkBackground = true;
            }
            else if (UserDataManager.Instance.UserData.RP >= 600)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Brown);
                MainBorder.Background = new SolidColorBrush(Colors.SandyBrown);
                Tier.Text = "브론즈";
                _isDarkBackground = false;
            }
            else
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
                MainBorder.Background = new SolidColorBrush(Colors.Black);
                Tier.Text = "아이언";
                _isDarkBackground = true;
            }

            SetTextProperties(Tier, _isDarkBackground);
        }

        private void SetTextProperties(TextBlock textBlock, bool isDarkBackground)
        {
            if (isDarkBackground)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void UpdateRanking()
        {
            SetTextProperties(Ranking, _isDarkBackground);
            Ranking.Text = "Asia " + UserDataManager.Instance.UserData.Ranking.ToString() + "위" + " / " + UserDataManager.Instance.UserData.RankPercent.ToString("F2") + "%";
        }

        private void UpdateWinRate()
        {
            SetTextProperties(WinRate, _isDarkBackground);
            WinRate.Text = "Win Rate: " + (UserDataManager.Instance.UserData.Top1 * 100).ToString("F2") + "%";
        }
    }
}