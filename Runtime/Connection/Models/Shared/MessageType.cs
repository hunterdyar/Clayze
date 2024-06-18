namespace Clayze
{
	public enum MessageType : byte
	{
		Echo = 0,
		Add = 1,
		Remove = 2,
		GetAll = 3,
		IDReply = 4,
		Clear = 5,
		Changed = 6,
		ChangeConfirm = 7,
	}
}