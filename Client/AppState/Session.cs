using DocsWASM.Shared;
using Microsoft.JSInterop;

namespace DocsWASM.Client.AppState
{
	public class Session
	{
		public AccountModels.User? user;
		public uint? lastDocumentViewer;
		public bool IsAllowedViewModerate()
		{
			return user != null && (user.TypeOfUser == 3 || user.TypeOfUser == 1);

		}
	}
}
