namespace PostOffice.Application.Contracts.PostOffices;

public  record PostOfficeDto(Guid Id, string ZipCode, string Name, string City);
public  record CreatePostOfficeRequest(string ZipCode, string Name, string City);
public  record UpdatePostOfficeRequest(string ZipCode, string Name, string City);
