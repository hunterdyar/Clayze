namespace Connection.Models
{
	public interface IInkMessageHandler
	{
		public void OnInkStartFromServer(byte[] data);
		public void OnInkEndFromServer(byte[] data);
		public void OnInkAddFromServer(byte[] data);
		public void OnInkNewCanvasFromServer(byte[] data);
	}
}