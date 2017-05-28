using kursach.Core;
using System.ComponentModel;

namespace kursach
{
	public class EditWindowViewModel : INotifyPropertyChanged
	{
		private bool isBusy;
		public event PropertyChangedEventHandler PropertyChanged;

		// Create the OnPropertyChanged method to raise the event
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				// Call OnPropertyChanged whenever the property is updated
				OnPropertyChanged("IsBusy");
			}
		}
	}
}
