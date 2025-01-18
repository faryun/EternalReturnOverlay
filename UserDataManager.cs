using System.Windows.Threading;

public class UserData
{
    public string? Nickname { get; set; }
    public int RP { get; set; }
    public int Ranking { get; set; }
    public int TotalGames { get; internal set; }
    public double RankPercent { get; internal set; }
    public double Top1 { get; internal set; }
    public double Top2 { get; internal set; }
    public double Top3 { get; internal set; }
    public double AverageRank { get; internal set; }
    public int RankSize{ get; internal set; }
}

public class UserDataManager
{
    private static UserDataManager _instance;
    public static UserDataManager Instance => _instance ??= new UserDataManager();

    public UserData UserData { get; set; }
    public TimeSpan SharedTimeSpan { get; set; } = TimeSpan.FromSeconds(30); // 공유된 TimeSpan 객체
    public DispatcherTimer SharedTimer { get; private set; }

    private UserDataManager()
    {
        UserData = new UserData();
        SharedTimer = new DispatcherTimer();
        SharedTimer.Interval = TimeSpan.FromSeconds(1);
        SharedTimer.Tick += SharedTimer_Tick;
        SharedTimer.Start();
    }

    private void SharedTimer_Tick(object? sender, EventArgs e)
    {
        if (SharedTimeSpan.TotalSeconds > 0)
        {
            SharedTimeSpan = SharedTimeSpan.Add(TimeSpan.FromSeconds(-1));
        }
        else
        {
            SharedTimeSpan = TimeSpan.FromSeconds(30); // 30초 간격으로 재설정
        }
    }
}