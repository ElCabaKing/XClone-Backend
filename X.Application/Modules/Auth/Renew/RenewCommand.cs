using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace X.Application.Modules.Auth.Renew;

	public class RenewAuthCommand
	{
		[Required]
		[Description("Token que se usa para renovar la sesión. Este se consigue, al iniciar sesión en el aplicativo.")]
		public string RefreshToken { get; set; } = null!;
	}