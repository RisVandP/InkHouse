using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InkHouse.Models
{
    // 借阅记录实体类，对应数据库中的BorrowRecords表
    [Table("BorrowRecords")]
    public class BorrowRecord
    {
        // 主键，自增ID
        [Key]
        public int Id { get; set; }
        // 用户ID，外键，必填
        [Required]
        public int UserId { get; set; }
        // 图书ID，外键，必填
        [Required]
        public int BookId { get; set; }
        // 借出时间，必填
        [Required]
        public DateTime BorrowDate { get; set; }
        // 归还时间，可以为空
        public DateTime? ReturnDate { get; set; }
        // 状态，必填，最大长度20（如"借出"或"归还"）
        [Required]
        [MaxLength(20)]
        public required string Status { get; set; } // 借出/归还
        // 是否已归还（用于简化逻辑）
        public bool IsReturned { get; set; } = false;

        // 导航属性：图书
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
        // 导航属性：用户
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
} 