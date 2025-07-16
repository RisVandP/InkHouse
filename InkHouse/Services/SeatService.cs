using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InkHouse.Models;

namespace InkHouse.Services
{
    /// <summary>
    /// 座位服务类，负责座位预约相关业务逻辑
    /// </summary>
    public class SeatService
    {
        private readonly DbContextFactory _dbContextFactory;

        public SeatService(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// 获取所有座位
        /// </summary>
        public async Task<List<Seat>> GetAllSeatsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Seats.AsNoTracking().OrderBy(s => s.SeatNumber).ToListAsync();
        }

        /// <summary>
        /// 获取座位预约统计信息
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
        /// 用户预约座位
        /// </summary>
        public async Task<SeatReservation> ReserveSeatAsync(int seatId, int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var seat = await context.Seats.FindAsync(seatId);
            if (seat == null)
                throw new InvalidOperationException("座位不存在");
            if (seat.Status != "Free")
                throw new InvalidOperationException("该座位不可预约");
            // 检查用户是否已有未完成的预约
            bool hasActive = await context.SeatReservations.AnyAsync(r => r.UserId == userId && (r.Status == "已预约" || r.Status == "使用中"));
            if (hasActive)
                throw new InvalidOperationException("您已有未完成的座位预约");
            // 创建预约记录
            var reservation = new SeatReservation
            {
                UserId = userId,
                SeatId = seatId,
                ReserveTime = DateTime.Now,
                Status = "已预约"
            };
            context.SeatReservations.Add(reservation);
            // 更新座位状态
            seat.Status = "Reserved";
            seat.CurrentUserId = userId;
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// 用户到馆，座位变为使用中
        /// </summary>
        public async Task<SeatReservation> CheckInAsync(int reservationId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var reservation = await context.SeatReservations.Include(r => r.Seat).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                throw new InvalidOperationException("预约记录不存在");
            if (reservation.Status != "已预约")
                throw new InvalidOperationException("当前预约不可到馆");
            reservation.Status = "使用中";
            reservation.CheckInTime = DateTime.Now;
            // 更新座位状态
            if (reservation.Seat != null)
            {
                reservation.Seat.Status = "Occupied";
            }
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// 用户离馆，座位变为空闲
        /// </summary>
        public async Task<SeatReservation> CheckOutAsync(int reservationId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var reservation = await context.SeatReservations.Include(r => r.Seat).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                throw new InvalidOperationException("预约记录不存在");
            if (reservation.Status != "使用中")
                throw new InvalidOperationException("当前预约不可离馆");
            reservation.Status = "已离馆";
            reservation.CheckOutTime = DateTime.Now;
            // 更新座位状态
            if (reservation.Seat != null)
            {
                reservation.Seat.Status = "Free";
                reservation.Seat.CurrentUserId = null;
            }
            await context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// 获取用户当前有效预约（已预约/使用中）
        /// </summary>
        public async Task<SeatReservation?> GetUserActiveReservationAsync(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.SeatReservations
                .Include(r => r.Seat)
                .Where(r => r.UserId == userId && (r.Status == "已预约" || r.Status == "使用中"))
                .OrderByDescending(r => r.ReserveTime)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取所有座位预约情况
        /// </summary>
        public async Task<List<SeatReservation>> GetAllActiveReservationsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.SeatReservations
                .Include(r => r.Seat)
                .Include(r => r.User)
                .Where(r => r.Status == "已预约" || r.Status == "使用中")
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