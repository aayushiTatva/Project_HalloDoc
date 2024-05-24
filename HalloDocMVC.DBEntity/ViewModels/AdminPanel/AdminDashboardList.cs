namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class AdminDashboardList
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? RequestorPhoneNumber { get; set; }
        public string? Notes { get; set; }
        public int? RequestId { get; set; }
        public int? RequestTypeId { get; set; }
        public string? Address { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? Region { get; set; }
        public int RegionId { get; set; }
        public string Status { get; set; }
        public int ProviderEncounterStatus { get; set; }
        public bool IsFinalize { get; set; }
        public string RequestorAspId { get; set; }
        public string? PhysicianAspId { get; set; }
        public string RequestAspId { get; set; }
    }

    public class PaginationModel
    {
        public List<AdminDashboardList>? list { get; set; }
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public string? SearchInput { get; set; }
        public int? RegionId { get; set; }
        public int? RequestType { get; set; }
        public int TotalItemCount { get; set; } = 1;
    }
}
