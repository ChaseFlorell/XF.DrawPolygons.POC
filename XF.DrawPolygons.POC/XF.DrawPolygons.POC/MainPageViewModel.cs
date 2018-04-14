using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using Xamarin.Forms;

namespace XF.DrawPolygons.POC
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IList<Position> _tappedLocations = new List<Position>();
        private bool _allowedToAddPoint = true;
        private ICommand _mapClickedCommand;
        private ICommand _pinClickedCommand;

        public ICommand MapClickedCommand => _mapClickedCommand ?? (_mapClickedCommand = new Command<Position>(OnMapClicked));
        public ICommand PinClickedCommand => _pinClickedCommand ?? (_pinClickedCommand = new Command<TKCustomMapPin>(OnPinClicked));

        public IList<TKCustomMapPin> Pins => _tappedLocations.Select(p => new TKCustomMapPin
        {
            Position = p, 
            DefaultPinColor = Color.White
        }).ToList();

        public IList<TKPolyline> Polylines => new List<TKPolyline>
        {
            new TKPolyline {LineCoordinates = _tappedLocations.ToList(), Color = Color.White, LineWidth = 2}
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddTappedLocationAndRaise(Position position)
        {
            if (!_allowedToAddPoint) return;

            _tappedLocations.Add(position);
            RaisePropertyChanged(nameof(Pins));
            RaisePropertyChanged(nameof(Polylines));
        }

        private void OnMapClicked(Position position)
        {
            AddTappedLocationAndRaise(position);
        }

        private void OnPinClicked(TKCustomMapPin pin)
        {
            var firstPosition = _tappedLocations.First();
            var position = pin.Position;
            AddTappedLocationAndRaise(position);

            // if the user tapped the first marker, we'll close the gap.
            // and prevent them from adding more points.
             _allowedToAddPoint = !(Math.Abs(position.Latitude - firstPosition.Latitude) <= 0 && Math.Abs(position.Longitude - firstPosition.Longitude) <= 0);
        }
    }
}