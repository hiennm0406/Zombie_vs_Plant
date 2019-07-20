using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlertPanel : AnimatedPanel {

	static AlertPanel _instance;

	public static AlertPanel instance {
		get {
			return _instance;
		}
	}

	public static void Show(string title, string text, System.Action<bool> onFinish, bool showCancelButton = true)
	{
		if ( _instance == null ) {
			return;
		}

		_instance._Show(title, text,onFinish,showCancelButton);

	}		

	//[SerializeField] Pane buttonGrid;
	[SerializeField] Button okButton;
	[SerializeField] Button cancelButton;

	[SerializeField] Text titleLabel;
	[SerializeField] Text textLabel;

	System.Action<bool> finishCallback = null;

	#region Monobehavior

	void Awake()
	{
		_instance = this;
		this.Hide ();
	}

	// Use this for initialization
	void Start() 
	{

	}

	#endregion

	void _Show(string title, string text, System.Action<bool> onFinish, bool showCancelButton = true)
	{		
		cancelButton.gameObject.SetActive(showCancelButton);

		this.titleLabel.text = title;
		this.textLabel.text = text;
		finishCallback = onFinish;
		this.Show();

	}

	public void Ok()
	{
		if ( finishCallback != null ) {
			finishCallback(true);
		}
		this.Hide();
	}

	public void Cancel()
	{
		if ( finishCallback != null ) {
			finishCallback(false);
		}
		this.Hide();
	}

}
