using DocsWASM.Shared;

namespace DocsWASM.Client.AppState
{
	public class Session
	{
		public uint documentZoomLevel = 100;
		public uint? lastDocumentViewer;
        public AccountModels.User? user;

    }
}
