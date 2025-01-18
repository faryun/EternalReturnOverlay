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
    private static UserDataManager? _instance;
    public UserData UserData { get; private set; }

    private UserDataManager()
    {
        UserData = new UserData();
    }

    public static UserDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UserDataManager();
            }
            return _instance;
        }
    }
}