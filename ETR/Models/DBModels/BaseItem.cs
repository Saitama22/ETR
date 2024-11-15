using System.ComponentModel.DataAnnotations;

namespace ETR.Models.DBModels
{
    /// <summary>
    /// Продукт для оценки
    /// </summary>
    public class BaseItem
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
		public string? Creator { get; set; }
		public string? Description { get; set; }
	}
}
