using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EternalReturnOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly APIService _apiService;
        private readonly DispatcherTimer _timer;
        private TimeSpan _timeSpan;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new APIService();
            _timeSpan = TimeSpan.FromSeconds(30); // 30초 간격
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // 1초마다 틱
            _timer.Tick += Timer_Tick;
            _timer.Start();
            UpdateUI();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (_timeSpan.TotalSeconds > 0)
            {
                _timeSpan = _timeSpan.Add(TimeSpan.FromSeconds(-1));
                TimerText.Text = $"Next update in: {_timeSpan:mm\\:ss}";
            }
            else
            {
                _timeSpan = TimeSpan.FromSeconds(30); // 30초 간격으로 재설정
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ChangeNickNameButton_Click(object sender, RoutedEventArgs e)
        {
            NickNameWindow nickNameWindow = new NickNameWindow();
            if (nickNameWindow.ShowDialog() == true)
            {
                string newNickName = nickNameWindow.NickName;
                UserDataManager.Instance.UserData.Nickname = newNickName;
                Nick.Text = newNickName;
                await FetchUserDataAsync(newNickName);
            }
        }

        private async Task FetchUserDataAsync(string nickname)
        {
            int userNum = await _apiService.GetUserNumAsync(nickname);
            int seasonId = await _apiService.GetSeasonIDAsync();
            await _apiService.GetUserStatsAsync(userNum, seasonId);
            UpdateUI();
            Console.WriteLine("Data Updated");
        }

        private void UpdateUI()
        {
            UpdateRP();
            UpdateTier();
            UpdateRanking();
            UpdateWinRate();
        }

        private void UpdateRP()
        {
            RP.Text = "RP " + UserDataManager.Instance.UserData.RP.ToString();
        }

        private void UpdateTier()
        {
            // 티어에 따라 이미지 & 텍스트 변경(추가 예정)
            if (UserDataManager.Instance.UserData.RP >= 7700 && UserDataManager.Instance.UserData.Ranking <= 300)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                MainBorder.Background = new SolidColorBrush(Colors.IndianRed);
                Tier.Text = "이터니티";
            }
            else if (UserDataManager.Instance.UserData.RP >= 7700 && UserDataManager.Instance.UserData.Ranking <= 1000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.White);
                MainBorder.Background = new SolidColorBrush(Colors.WhiteSmoke);
                Tier.Text = "데미갓";
            }
            else if (UserDataManager.Instance.UserData.RP >= 7000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.LightBlue);
                MainBorder.Background = new SolidColorBrush(Colors.SkyBlue);
                Tier.Text = "미스릴";
            }
            else if (UserDataManager.Instance.UserData.RP >= 6400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Blue);
                MainBorder.Background = new SolidColorBrush(Colors.DarkBlue);
                Tier.Text = "메테오라이트";
            }
            else if (UserDataManager.Instance.UserData.RP >= 5000)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Purple);
                MainBorder.Background = new SolidColorBrush(Colors.MediumPurple);
                Tier.Text = "다이아몬드";
            }
            else if (UserDataManager.Instance.UserData.RP >= 3600)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Lime);
                MainBorder.Background = new SolidColorBrush(Colors.LimeGreen);
                Tier.Text = "플래티넘";
            }
            else if (UserDataManager.Instance.UserData.RP >= 2400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);
                MainBorder.Background = new SolidColorBrush(Colors.LightYellow);
                Tier.Text = "골드";
            }
            else if (UserDataManager.Instance.UserData.RP >= 1400)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Silver);
                MainBorder.Background = new SolidColorBrush(Colors.SlateGray);
                Tier.Text = "실버";
            }
            else if (UserDataManager.Instance.UserData.RP >= 600)
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Brown);
                MainBorder.Background = new SolidColorBrush(Colors.SandyBrown);
                Tier.Text = "브론즈";
            }
            else
            {
                MainBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
                MainBorder.Background = new SolidColorBrush(Colors.Black);
                Tier.Text = "아이언";
            }
        }

        private void UpdateRanking()
        {
            Ranking.Text = "Asia " + UserDataManager.Instance.UserData.Ranking.ToString() + "위" + " / " + UserDataManager.Instance.UserData.RankPercent.ToString("F2") + "%";
        }

        private void UpdateWinRate()
        {
            WinRate.Text = "Win Rate: " + UserDataManager.Instance.UserData.Top1 * 100 + "%";
        }
    }
}