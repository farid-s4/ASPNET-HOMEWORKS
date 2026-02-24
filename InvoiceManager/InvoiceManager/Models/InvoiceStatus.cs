namespace InvoiceManager.Models;

public enum InvoiceStatus
{
    Draft,
    Created,
    Sent,
    Received,
    Paid,
    Cancelled,
    Rejected
}