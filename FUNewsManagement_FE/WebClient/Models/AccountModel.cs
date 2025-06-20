namespace WebClient.Models
{
    public class AccountModel
    {
        public short AccountId { get; set; }
        public string AccountEmail { get; set; } = string.Empty;
        public string AccountPassword { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public short AccountRole { get; set; } // 1 = Staff, 2 = Lecturer
    }

    public class CategoryModel
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }  // ✅ Bổ sung dòng này
        public bool? IsActive { get; set; }
    }

}
