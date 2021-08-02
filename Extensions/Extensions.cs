using BastetAPI.DTOs;
using BastetAPI.Entities;
using BastetFTMAPI.Authentication;

namespace BastetAPI
{
    public static class Extensions
    {
        public static ClientDto ClientAsDto(this Client client) => new
        (
                client.Id,
                client.CreatedDate,
                client.UpdatedDate,
                client.Firstname,
                client.Lastname,
                client.Suffix,
                client.Addresses.ConvertAll(a => new AddressInfoDto(a.Id, a.Address, a.City, a.State, a.Zip)),
                client.PhoneNumbers.ConvertAll(p => new PhoneInfoDto(p.Id, p.Phone)),
                client.Emails.ConvertAll(e => new EmailInfoDto(e.Id, e.Email)),
                client.Tickets.ConvertAll(t => new TicketInfoDto(t.Id,
                                                                t.Destination,
                                                                t.IssuenceDate,
                                                                t.TicketNumber,
                                                                t.PNR,
                                                                t.TicketTotal,
                                                                t.Airline,
                                                                t.TicketSoldFor,
                                                                t.TicketFare,
                                                                t.ToBePaid,
                                                                t.ToBePaidFor,
                                                                t.Comission,
                                                                t.Notes.ConvertAll(n => new NoteInfoDto(n.Id, n.Note)))),
                client.Notes.ConvertAll(n => new NoteInfoDto(n.Id, n.Note))
         );

        public static ScuffedClientDto ScuffedClientAsDto(this ScuffedClient client) => new
        (
               client.Id,
               client.Firstname,
               client.Lastname,
               client.Suffix
        );
        public static TicketInfoDto TicketsAsDto(this TicketInfo Tickets) => new
        (
           Tickets.Id,
           Tickets.Destination,
           Tickets.IssuenceDate,
           Tickets.TicketNumber,
           Tickets.PNR,
           Tickets.TicketTotal,
           Tickets.Airline,
           Tickets.TicketSoldFor,
           Tickets.TicketFare,
           Tickets.ToBePaid,
           Tickets.ToBePaidFor,
           Tickets.Comission,
           Tickets.Notes.ConvertAll(n => new NoteInfoDto(n.Id, n.Note))
        );
        public static UserDto UsersAsDto(this User user) => new
        (
           user.Id,
           user.Username,
           user.DisplayName,
           user.UserNameHash,
           user.Email,
           user.Password,
           user.Roles.ConvertAll(n => new Roles()),
           user.CreationDate,
           user.LastLoginDate,
           user.FailedAttempts,
           user.IsLocked
        );
    }
}
