namespace PostOffice.Application.Contracts.PostOffices;

public sealed record PostOfficeDto(Guid Id, string ZipCode, string Name, string City);
public sealed record CreatePostOfficeRequest(string ZipCode, string Name, string City);
public sealed record UpdatePostOfficeRequest(string ZipCode, string Name, string City);
