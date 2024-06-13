using SyncedProperty;
using UnityEditor;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomEditor(typeof(SyncFloat))]
	public class SyncFloatEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
	}
}