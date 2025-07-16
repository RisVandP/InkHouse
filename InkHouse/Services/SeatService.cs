using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InkHouse.Models;

namespace InkHouse.Services
{
    /// <summary>
    /// ��λ�����࣬������λԤԼ���ҵ���߼�
    /// </summary>
    public class SeatService
    {
        private readonly DbContextFactory _dbContextFactory;

        public SeatService(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// ��ȡ������λ
        /// </summary>
        public async Task<List<Seat>> GetAllSeatsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Seats.AsNoTracking().OrderBy(s => s.SeatNumber).ToListAsync();
        }

        /// <summary>
        /// ��ȡ��λԤԼͳ����Ϣ
        /// </summary>
        public async Task<(int totalSeats, int freeSeats, int reservedSeats, int occupiedSeats)> GetSeatStatisticsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var totalSeats = await context.Seats.CountAsync();
            var freeSeats = await context.Seats.CountAsync(s => s.Status == "Free");
            var reservedSeats = await context.Seats.CountAsync(s => s.Status == "Reserved");
            var occupiedSeats = await context.Seats.CountAsync(s => s.Status == "Occupied");
            return (totalSeats, freeSeats, reservedSeats, occupiedSeats);
        }

        /// <summary>
        /// �û�ԤԼ��λ
        /// </summary>
        public async Task<SeatReservation> ReserveSeatAsync(int seatId, int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var seat = await context.Seats.FindAsync(seatId);
            if (seat == null)
                throw new InvalidOperationException("��λ������");
            if (seat.Status != "Free")
                throw new InvalidOperationException("����λ����ԤԼ");
            // ����û��Ƿ�����δ��ɵ�ԤԼ
            bool hasActive = await context.SeatReservations.AnyAsync(r => r.UserId == userId && (r.Status == "��ԤԼ" || r.Status == "ʹ����"));
            if (hasActive)
                throw new InvalidOperationException("������δ��ɵ���λԤԼ");
            // ����ԤԼ��¼
            var reservation = new SeatReservation
            {
                UserId = userId,
                SeatId = seatId,
                ReserveTime = DateTime.Now,
                Status = "��ԤԼ"
            };
            context.SeatReservations.Add(reservation);
            // ������λ״̬
            seat.Status = "Reserved";
            seat.CurrentUserId = userId;
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// �û����ݣ���λ��Ϊʹ����
        /// </summary>
        public async Task<SeatReservation> CheckInAsync(int reservationId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var reservation = await context.SeatReservations.Include(r => r.Seat).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                throw new InvalidOperationException("ԤԼ��¼������");
            if (reservation.Status != "��ԤԼ")
                throw new InvalidOperationException("��ǰԤԼ���ɵ���");
            reservation.Status = "ʹ����";
            reservation.CheckInTime = DateTime.Now;
            // ������λ״̬
            if (reservation.Seat != null)
            {
                reservation.Seat.Status = "Occupied";
            }
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// �û���ݣ���λ��Ϊ����
        /// </summary>
        public async Task<SeatReservation> CheckOutAsync(int reservationId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var reservation = await context.SeatReservations.Include(r => r.Seat).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                throw new InvalidOperationException("ԤԼ��¼������");
            if (reservation.Status != "ʹ����")
                throw new InvalidOperationException("��ǰԤԼ�������");
            reservation.Status = "�����";
            reservation.CheckOutTime = DateTime.Now;
            // ������λ״̬
            if (reservation.Seat != null)
            {
                reservation.Seat.Status = "Free";
                reservation.Seat.CurrentUserId = null;
            }
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// ��ȡ�û���ǰ��ЧԤԼ����ԤԼ/ʹ���У�
        /// </summary>
        public async Task<SeatReservation?> GetUserActiveReservationAsync(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.SeatReservations
                .Include(r => r.Seat)
                .Where(r => r.UserId == userId && (r.Status == "��ԤԼ" || r.Status == "ʹ����"))
                .OrderByDescending(r => r.ReserveTime)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// ��ȡ������λԤԼ���
        /// </summary>
        public async Task<List<SeatReservation>> GetAllActiveReservationsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.SeatReservations
                .Include(r => r.Seat)
                .Include(r => r.User)
                .Where(r => r.Status == "��ԤԼ" || r.Status == "ʹ����")
                .ToListAsync();
        }

        /// <summary>
        /// ��ȡ�û�������λԤԼ��¼
        /// </summary>
        public async Task<List<SeatReservation>> GetUserReservationsAsync(int userId)
        {
            try
            {
                Console.WriteLine($"��ʼ��ѯ�û�ID {userId} ����λԤԼ��¼...");
                using var context = _dbContextFactory.CreateDbContext();
                
                // �ȼ�����ݿ����Ƿ�������
                var totalCount = await context.SeatReservations.CountAsync();
                Console.WriteLine($"���ݿ����ܹ��� {totalCount} ����λԤԼ��¼");
                
                var userCount = await context.SeatReservations.CountAsync(sr => sr.UserId == userId);
                Console.WriteLine($"�û� {userId} �����ݿ����� {userCount} ����λԤԼ��¼");
                
                var records = await context.SeatReservations
                    .Include(r => r.Seat)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.ReserveTime)
                    .ToListAsync();
                
                Console.WriteLine($"��ѯ�� {records.Count} ����λԤԼ��¼");
                foreach (var record in records)
                {
                    Console.WriteLine($"��λԤԼ��¼: ID={record.Id}, �û�ID={record.UserId}, ��λID={record.SeatId}, ״̬={record.Status}");
                }
                
                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"��ѯ�û���λԤԼ��¼ʱ�����쳣: {ex.Message}");
                Console.WriteLine($"�쳣��ջ: {ex.StackTrace}");
                return new List<SeatReservation>();
            }
        }

        /// <summary>
        /// �����λ
        /// </summary>
        public async Task<Seat> AddSeatAsync(string seatNumber)
        {
            using var context = _dbContextFactory.CreateDbContext();
            if (await context.Seats.AnyAsync(s => s.SeatNumber == seatNumber))
                throw new InvalidOperationException("��λ����Ѵ���");
            var seat = new Seat { SeatNumber = seatNumber, Status = "Free" };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            return seat;
        }

        /// <summary>
        /// ɾ����λ
        /// </summary>
        public async Task<bool> DeleteSeatAsync(int seatId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var seat = await context.Seats.FindAsync(seatId);
            if (seat == null) return false;
            context.Seats.Remove(seat);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// �޸���λ״̬
        /// </summary>
        public async Task<Seat> UpdateSeatStatusAsync(int seatId, string newStatus)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var seat = await context.Seats.FindAsync(seatId);
            if (seat == null) throw new InvalidOperationException("��λ������");
            seat.Status = newStatus;
            if (newStatus == "Free") seat.CurrentUserId = null;
            await context.SaveChangesAsync();
            return seat;
        }
    }
} 