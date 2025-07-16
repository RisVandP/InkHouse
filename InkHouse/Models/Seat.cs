using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InkHouse.Models
{
    // ��λʵ���࣬��Ӧ���ݿ��е� Seats ��
    [Table("Seats")]
    public class Seat
    {
        // ����������ID
        [Key]
        public int Id { get; set; }
        // ��λ��ţ������󳤶�10
        [Required]
        [MaxLength(10)]
        public required string SeatNumber { get; set; }
        // ��λ״̬�����С���ԤԼ��ʹ����
        [Required]
        [MaxLength(10)]
        public required string Status { get; set; } // Free/Reserved/Occupied
        // ��ǰԤԼ�û�ID����Ϊ��
        public int? CurrentUserId { get; set; }
        // �������ԣ���ǰԤԼ�û�
        [ForeignKey("CurrentUserId")]
        public User? CurrentUser { get; set; }
    }
} 