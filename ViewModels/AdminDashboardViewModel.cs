namespace MediAI.ViewModels
{
    public class AdminDashboardViewModel
    {
        // الإحصائيات العامة
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalDiagnosiss { get; set; }
        public int PendingDiagnosiss { get; set; }
        public int CompletedDiagnosiss { get; set; }
        public int CriticalDiagnosiss { get; set; }
        public int TotalReports { get; set; }
        public int TotalNotifications { get; set; }

        // المستخدمين
        public List<UserDto>? LatestUsers { get; set; }        
        public List<SystemLog>? RecentActivities { get; set; }
    }
}
