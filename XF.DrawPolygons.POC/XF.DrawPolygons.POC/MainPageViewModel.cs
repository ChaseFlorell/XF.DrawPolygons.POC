using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _allowedToAddPoint = true;
        private ICommand _mapClickedCommand;
        private ICommand _pinClickedCommand;
        private ICommand _clearPolygonCommand;

        public ICommand MapClickedCommand => _mapClickedCommand ?? (_mapClickedCommand = new Command<Position>(OnMapClicked));
        public ICommand PinClickedCommand => _pinClickedCommand ?? (_pinClickedCommand = new Command<TKCustomMapPin>(OnPinClicked));
        public ICommand ClearPolygonCommand => _clearPolygonCommand ?? (_clearPolygonCommand = new Command(ClearPolygon));

        private void ClearPolygon(object obj)
        {
            Pins.Clear();
            Polylines.Clear();
            Polygons.Clear();
            _allowedToAddPoint = true;
        }

        public ObservableCollection<TKCustomMapPin> Pins { get; } = new ObservableCollection<TKCustomMapPin>();
        public ObservableCollection<TKPolyline> Polylines { get; } = new ObservableCollection<TKPolyline>();
        public ObservableCollection<TKPolygon> Polygons { get; } = new ObservableCollection<TKPolygon>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AddTappedLocationAndRaise(Position position)
        {
            if (!_allowedToAddPoint) return;

            Pins.Add(new TKCustomMapPin { Position = position, DefaultPinColor = Color.Teal });
            if (Pins.Count <= 1) return; // can't make a line with 1 pin
            
            var previous = Pins.Reverse().Skip(1).First();
            Polylines.Add(new TKPolyline{LineCoordinates = new List<Position>{previous.Position,position},Color = Color.Teal, LineWidth = 2});
        }

        private void OnMapClicked(Position position)
        {
            AddTappedLocationAndRaise(position);
        }

        private void OnPinClicked(TKCustomMapPin pin)
        {
            var firstPosition = Pins.First().Position;
            var position = pin.Position;

            // if the user tapped the first marker, we'll close the gap.
            // and prevent them from adding more points.
             _allowedToAddPoint = _allowedToAddPoint 
                                  && Pins.Count > 1
                                  && !(Math.Abs(position.Latitude - firstPosition.Latitude) <= 0 && Math.Abs(position.Longitude - firstPosition.Longitude) <= 0);

             if (!_allowedToAddPoint)
             {
                 var polygon = new TKPolygon
                 {
                     Color = Color.FromHex("#7f000000"),
                     StrokeColor = Color.Black, 
                     StrokeWidth = 2
                 };
                 foreach (var line in Polylines)
                 {
                     polygon.Coordinates.AddRange(line.LineCoordinates);
                 }
                 Polylines.Clear();
                 Polygons.Clear();
                 Pins.Clear();
                 Polygons.Add(polygon);
             }
        }
    }
}