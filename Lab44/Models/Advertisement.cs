using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvertisementServiceMVC2.Models
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal Price { get; set; }

        // Внешние ключи
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public int RegionId { get; set; }
        public virtual Region Region { get; set; } = null!;

        // Связь с пользователем Identity (String ID)
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Status { get; set; } = "Active";
        public enum AdvertisementStatus
        {
            Active,
            Sold,
            Archived
        }
        public virtual ICollection<AdvertisementPhoto> Photos { get; set; } = new List<AdvertisementPhoto>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}