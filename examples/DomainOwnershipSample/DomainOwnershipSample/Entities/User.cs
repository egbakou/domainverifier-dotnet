using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DomainOwnershipSample.Entities;

public class User : IdentityUser
{
    public ICollection<Domain> Domains { get; set; }
}