using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InkHouse.Models
{
    // ��λԤԼ��¼ʵ���࣬��Ӧ���ݿ��е� SeatReservations ��
    [Table("SeatReservations")]
    public class SeatReservation
    {
        // ����������ID
        [Key]
        public int Id { get; set; }
        // �û�ID�����������
        [Required]
        public int UserId { get; set; }
        // ��λID�����������
        [Required]
        public int SeatId { get; set; }
        // ԤԼʱ�䣬����
        [Required]
        public DateTime ReserveTime { get; set; }
        // ����ʱ�䣬��Ϊ��
        public DateTime? CheckInTime { get; set; }
        // ���ʱ�䣬��Ϊ��
        public DateTime? CheckOutTime { get; set; }
        // ״̬�������󳤶�20���硰��ԤԼ������ʹ���С���������ݡ���
        [Required]
        [MaxLength(20)]
        public required string Status { get; set; }
        // �������ԣ���λ
        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }
        // �������ԣ��û�
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
} 