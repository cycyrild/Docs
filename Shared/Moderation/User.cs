using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Moderation
{
	public class User
	{
		public uint Id { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastLogin { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool FullNamePrivacy { get; set; }
	}
}
/*select id, email, username, createdDate, lastLogin, firstName, lastName, fullNamePrivacy
from login*/