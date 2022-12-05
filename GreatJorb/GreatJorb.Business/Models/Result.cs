namespace GreatJorb.Business.Models;

public record Result<T>(bool Success, T Data);

