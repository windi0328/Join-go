using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.ViewModels
{
	public class LoginVM
	{
		public string Msg { get; set; }
		public bool Result { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string VCode { get; set; }
	}
}