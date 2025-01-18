using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EternalReturnOverlay
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly APIService _apiService;

        public MainViewModel()
        {
            _apiService = new APIService();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
