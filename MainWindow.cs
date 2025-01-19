using System;
using System.Threading.Tasks;
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
        private OverlayWindow _overlayWindow;

        public MainWindow()
        {
            InitializeComponent();
            UserDataManager.Instance.SharedTimer.Tick += Timer_Tick;
            this.Closed += MainWindow_Closed;
            _overlayWindow = new OverlayWindow();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimerText.Text = $"Next update in: {UserDataManager.Instance.SharedTimeSpan:mm\\:ss}";
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void ChangeNickNameButton_Click(object sender, RoutedEventArgs e)
        {
            NickNameWindow nickNameWindow = new NickNameWindow();
            if (nickNameWindow.ShowDialog() == true)
            {
                string newNickName = nickNameWindow.NickName;
                try
                {
                    UserDataManager.Instance.UserData.Nickname = newNickName;
                    Nick.Text = newNickName;

                    // FetchUserDataAsync 호출하여 오버레이 업데이트
                    await _overlayWindow.FetchUserDataAsync(newNickName);
                }
                catch (UserNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(_overlayWindow.Visibility == Visibility.Visible)
            {
                _overlayWindow.Hide();
            }
            else
            {
                _overlayWindow.Show();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if(UserDataManager.Instance.UserData == null || string.IsNullOrEmpty(UserDataManager.Instance.UserData.Nickname))
            {
                MessageBox.Show("닉네임을 먼저 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            await _overlayWindow.FetchUserDataAsync(UserDataManager.Instance.UserData.Nickname);
            //자동 갱신 타이머 초기화
            UserDataManager.Instance.SharedTimeSpan = TimeSpan.FromSeconds(30);
            //과도한 요청 방지를 위해 5초간 버튼 비활성화
            RefreshButton.IsEnabled = false;
            RefreshButton.Content = "Refreshing...";
            await Task.Delay(TimeSpan.FromSeconds(5));
            RefreshButton.IsEnabled = true;
            RefreshButton.Content = "Refresh";
        }
    }
}