using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InkHouse.Models;
using InkHouse.Services;
using System.Linq;

namespace InkHouse.ViewModels
{
    public partial class SeatReservationViewModel : ViewModelBase
    {
        private readonly SeatService _seatService;
        private readonly User _currentUser;

        [ObservableProperty]
        private ObservableCollection<Seat> _seats = new();

        [ObservableProperty]
        private ObservableCollection<SeatReservation> _activeReservations = new();

        [ObservableProperty]
        private SeatReservation? _myReservation;

        [ObservableProperty]
        private int _totalSeats;
        [ObservableProperty]
        private int _freeSeats;
        [ObservableProperty]
        private int _reservedSeats;
        [ObservableProperty]
        private int _occupiedSeats;

        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand<Seat> ReserveCommand { get; }
        public IAsyncRelayCommand CheckInCommand { get; }
        public IAsyncRelayCommand CheckOutCommand { get; }

        public SeatReservationViewModel(SeatService seatService, User currentUser)
        {
            _seatService = seatService;
            _currentUser = currentUser;
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            ReserveCommand = new AsyncRelayCommand<Seat>(ReserveAsync);
            CheckInCommand = new AsyncRelayCommand(CheckInAsync);
            CheckOutCommand = new AsyncRelayCommand(CheckOutAsync);
            // _ = RefreshAsync(); // �Ƴ��Զ�ˢ�£���Ϊ��ͼ����ʱ����
        }

        private async Task RefreshAsync()
        {
            try
            {
                var seats = await _seatService.GetAllSeatsAsync();
                Seats = new ObservableCollection<Seat>(seats);
                var stats = await _seatService.GetSeatStatisticsAsync();
                TotalSeats = stats.totalSeats;
                FreeSeats = stats.freeSeats;
                ReservedSeats = stats.reservedSeats;
                OccupiedSeats = stats.occupiedSeats;
                MyReservation = await _seatService.GetUserActiveReservationAsync(_currentUser.Id);
                var allActive = await _seatService.GetAllActiveReservationsAsync();
                ActiveReservations = new ObservableCollection<SeatReservation>(allActive);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SeatReservationViewModel.RefreshAsync �쳣: " + ex);
                ShowError("��λ���ݼ���ʧ��: " + ex.Message);
            }
        }

        private async Task ReserveAsync(Seat? seat)
        {
            if (seat == null) return;
            try
            {
                await _seatService.ReserveSeatAsync(seat.Id, _currentUser.Id);
                await RefreshAsync();
                ShowSuccess("ԤԼ�ɹ���");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SeatReservationViewModel.ReserveAsync �쳣: " + ex);
                ShowError($"ԤԼʧ��: {ex.Message}");
            }
        }

        private async Task CheckInAsync()
        {
            if (MyReservation == null) return;
            try
            {
                await _seatService.CheckInAsync(MyReservation.Id);
                await RefreshAsync();
                ShowSuccess("�ѵ��ݣ���λʹ���У�");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SeatReservationViewModel.CheckInAsync �쳣: " + ex);
                ShowError($"����ʧ��: {ex.Message}");
            }
        }

        private async Task CheckOutAsync()
        {
            if (MyReservation == null) return;
            try
            {
                await _seatService.CheckOutAsync(MyReservation.Id);
                await RefreshAsync();
                ShowSuccess("����ݣ���λ���ͷţ�");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SeatReservationViewModel.CheckOutAsync �쳣: " + ex);
                ShowError($"���ʧ��: {ex.Message}");
            }
        }
    }
} 