using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Article{
    [Key]
    public int Id { get; set; }
    [StringLength(255)]
    [Required]
    [DisplayName("Tiêu đề")]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Required]
    [DisplayName("Ngày tạo")]
    public DateTime Created { get; set; }

    [Column(TypeName = "ntext")]
    [DisplayName("Nội dung bài viết")]
    public string? Content { get; set; }
}