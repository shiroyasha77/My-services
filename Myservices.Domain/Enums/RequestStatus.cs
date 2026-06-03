namespace MyServices.Domain.Enums;

public enum RequestStatus
{
    Pending,           // بانتظار القبول
    WaitingForStart,   // بانتظار البدء
    InProgress,        // جاري التنفيذ
    Completed,         // مكتمل
    Rejected,          // مرفوض
    Cancelled
}