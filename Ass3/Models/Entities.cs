namespace Ass3.Models;

public sealed record ServiceItem(int Id, string Name, int AverageMinutes, bool IsActive);
public sealed record DeskItem(int Id, string Name, string Status);
public sealed record TicketItem(long Id, string Number, string Taxpayer, string Service, string? Desk, string Status, DateTime IssuedAt, DateTime? CalledAt);
public sealed record DashboardSummary(int Taxpayers, int TicketsToday, int Waiting, int InService, int Served, int OpenDesks);
