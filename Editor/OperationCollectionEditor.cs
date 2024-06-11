using System.Linq;
using Clayze;
using Clayze.Connection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Packages.Clayze.Editor
{
	[CustomEditor(typeof(OperationCollection))]
	public class OperationCollectionEditor : UnityEditor.Editor
	{
		private OperationCollection opCol;
		public OperationCollectionEditor()
		{
			EditorApplication.playModeStateChanged+= delegate(PlayModeStateChange change)
			{
				if (change == PlayModeStateChange.ExitingPlayMode)
				{
					if (opCol != null)
					{
						opCol.Stop();
						Repaint();
					}
				}
			};
		}
		public override VisualElement CreateInspectorGUI()
		{
			 opCol = (target as OperationCollection);
			 
			 VisualElement root = new VisualElement();

			 var connectionLabel = new Label($"Connection Status: {opCol.ConnectionStatus}");
			 opCol.OnConnectionStatusChanged += status =>
			 {
				 connectionLabel.text = $"Connection Status: {opCol.ConnectionStatus}";
				 Repaint();
			 };
			 root.Add(connectionLabel);
			 var connnStatusProp = serializedObject.FindProperty("ConnectionStatus");
			 // var connStatusElement = new PropertyField(connnStatusProp);
			 // connStatusElement.SetEnabled(false);
			 // root.Add(connStatusElement);

			 var localStatusProp = serializedObject.FindProperty("localStatus");
			 var localStatusElement = new PropertyField(localStatusProp);
			 localStatusElement.SetEnabled(false);
			 root.Add(localStatusElement);

			 var reconDelayProp = serializedObject.FindProperty("reconnectionDelay");
			 var reconDelayElement = new PropertyField(reconDelayProp);
			 root.Add(reconDelayElement);

			 root.Add(ConnectionButtons());

			 root.Add(new Label("Connection Settings"));
			 var serverURLProp = serializedObject.FindProperty("connectionURL");
			 var serverURLElement= new PropertyField(serverURLProp);
			 root.Add(serverURLElement);
			 
			 if (opCol.recentConnectionURLs.Count > 0)
			 {
				 var recents = opCol.recentConnectionURLs.Select(StringUtility.ConvertSlashToUnicodeSlash).ToList();
				 var urlPresets = new DropdownField("Recent Servers", recents, 0, (s) =>
				 {
					 s = StringUtility.ConvertUnicodeSlashToSlash(s);
					 opCol.connectionURL = s;
					 return s;
				 });
				 root.Add(urlPresets);
			 }

			 var opsLabel = new Label("Operations");
			 opsLabel.style.paddingTop = new StyleLength(20);
			 root.Add(opsLabel);
			 root.Add(OperationButtons());

			 var opsProp = serializedObject.FindProperty("_operations");
			 var opsElement = new PropertyField(opsProp);
			 opsElement.SetEnabled(false);
			 root.Add(opsElement);
			 return root;
		}

		private VisualElement ConnectionButtons()
		{
			var connectionButtons = new VisualElement();
			connectionButtons.style.flexDirection = FlexDirection.Row;
			var reconnectButton = new Button();
			var stopButton = new Button();

			reconnectButton.text = "Connect";
			stopButton.text = "Disconnect";

			reconnectButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Disconnected &&
			                           EditorApplication.isPlaying);
			reconnectButton.clicked += () =>
			{
				if (opCol.ConnectionStatus != ConnectionStatus.Connected)
				{
					opCol.InitAndConnect();
					reconnectButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Disconnected &&
					                           EditorApplication.isPlaying);
					stopButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Connected
					                      || opCol.ConnectionStatus == ConnectionStatus.AttemptingToConnect
					                      && EditorApplication.isPlaying);
					Repaint();
				}
			};
			connectionButtons.Add(reconnectButton);
			stopButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Connected
			                      || opCol.ConnectionStatus == ConnectionStatus.AttemptingToConnect
			                      && EditorApplication.isPlaying);
			stopButton.clicked += () =>
			{
				if (opCol.ConnectionStatus == ConnectionStatus.Connected)
				{
					opCol.Stop();
					reconnectButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Disconnected &&
					                           EditorApplication.isPlaying);
					stopButton.SetEnabled(opCol.ConnectionStatus == ConnectionStatus.Connected
					                      || opCol.ConnectionStatus == ConnectionStatus.AttemptingToConnect
					                      && EditorApplication.isPlaying);
					Repaint();
				}
			};
			connectionButtons.Add(stopButton);
			return connectionButtons;
		}

		private VisualElement OperationButtons()
		{
			var opButtons = new VisualElement();
			opButtons.style.flexDirection = FlexDirection.Row;
			var clearButton = new Button();
			clearButton.text = "Clear";
			clearButton.tooltip = "Send Clear command to server";
			clearButton.clicked += () =>
			{
				if (opCol!)
				{
					opCol.Clear();
				}
			};

			var refreshButton = new Button();
			refreshButton.text = "Refresh";
			clearButton.tooltip = "Force all listeners (e.g. the volume) to act as if the volume needs to be visually updated. This is local-only. Usually this will redraw the mesh.";
			refreshButton.clicked += () =>
			{
				if (opCol.ConnectionStatus != ConnectionStatus.Connected)
				{
					if (opCol!)
					{
						opCol.ForceRefresh.Invoke();
					}
				}
			};
			
			opButtons.Add(clearButton);
			opButtons.Add(refreshButton);
			return opButtons;
		}
	}
}