using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BastetAPI.DTOs
{
    #region ClientDto

    public record ScuffedClientDto(
    Guid Id,
    string Firstname,
    string Lastname,
    string Suffix
);
    public record ClientDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        DateTimeOffset UpdatedDate,
        string Firstname,
        string Lastname,
        string Suffix,
        List<AddressInfoDto> Addresses,
        List<PhoneInfoDto> PhoneNumbers,
        List<EmailInfoDto> Emails,
        List<TicketInfoDto> Tickets,
        List<NoteInfoDto> Notes
    );
    public record AddressInfoDto(
        Guid Id,
        string Address,
        string City,
        string State,
        string Zip);
    public record PhoneInfoDto(Guid Id, string Phone);
    public record EmailInfoDto(Guid Id, string Email);
    public record TicketInfoDto(
        Guid Id,
        string Destination,
        DateTimeOffset IssuenceDate,
        string TicketNumber,
        string PNR,
        string TicketTotal,
        string Airline,
        string TicketSoldFor,
        string TicketFare,
        string ToBePaid,
        string ToBePaidFor,
        string Comission,
        List<NoteInfoDto> Notes
        );
    public record NoteInfoDto(Guid Id, string Note);

    #endregion ClientDto

    #region CreateClientDto

    public record CreateClientDto(
        [Required] string Firstname,
        [Required] string Lastname,
        [Required] string Suffix,
        List<CreateAddressInfoDto> Addresses,
        List<CreatePhoneInfoDto> PhoneNumbers,
        List<CreateEmailInfoDto> Emails,
        List<CreateTicketInfoDto> Tickets,
        List<CreateNoteInfoDto> Notes
        );
    public record CreateAddressInfoDto(
    string Address,
    string City,
    string State,
    string Zip);
    public record CreatePhoneInfoDto([Required] string Phone);
    public record CreateEmailInfoDto([Required] string Email);
    public record CreateTicketInfoDto(
       string Destination,
       DateTimeOffset IssuenceDate,
       string TicketNumber,
       string PNR,
       string TicketTotal,
       string Airline,
       string TicketSoldFor,
       string TicketFare,
       string ToBePaid,
       string ToBePaidFor,
       string Comission,
       List<CreateNoteInfoDto> Notes
       );
    public record CreateNoteInfoDto([Required] string Note);

    #endregion CreateClientDto

    #region UpdateClientDto

    public record UpdateClientDto(
        [Required] string Firstname,
        [Required] string Lastname,
        [Required] string Suffix
        );
    public record UpdateAddressInfoDto(
    string Address,
    string City,
    string State,
    string Zip);
    public record UpdatePhoneInfoDto(string Phone);
    public record UpdateEmailInfoDto(string Email);
    public record UpdateTicketInfoDto(
    string Destination,
    DateTimeOffset IssuenceDate,
    string TicketNumber,
    string PNR,
    string TicketTotal,
    string Airline,
    string TicketSoldFor,
    string TicketFare,
    string ToBePaid,
    string ToBePaidFor,
    string Comission,
    List<CreateNoteInfoDto> Notes
    );
    public record UpdateNoteInfoDto(string Note);

    #endregion UpdateClientDto
}
