namespace X.Shared.Constants
{
	public static class ResponseConstants
	{

		public const string COLLABORATOR_NOT_EXISTS = "El colaborador no existe";

		public const string PROJECT_NOT_EXISTS = "El proyecto no existe";

		public static string ERROR_UNEXPECTED(string traceId)
		{
			return $"Ha ocurrido un error inesperado: Contacto con soporte, mencionando el siguiente código de error: {traceId}";
		}

		public const string LOGIN_ERROR = "Correo o contraseña incorrectos";

		public const string USER_NOT_FOUND = "Usuario no encontrado";
	}
}