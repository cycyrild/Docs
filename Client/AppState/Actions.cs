namespace DocsWASM.Client.AppState
{
	public class Actions
	{
		public event Action SearchInputNeedToBeClear;
		public void SearchInputClear()
		{
			SearchInputNeedToBeClear.Invoke();
		}

	}
}
