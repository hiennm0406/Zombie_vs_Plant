using UnityEngine;
using UnityEngine.UI;


public class PanelController : PanelStack
{

	protected PanelStack _parentStack;

	public PanelStack parentStack {
		get {
			return _parentStack;
		}
		set {
			_parentStack = value;
		}
	}

	public bool IsShowing {
		get {
			return (gameObject.activeSelf);
		}
	}

	public Vector3 scale{
		get { 
			return this.gameObject.GetComponent<RectTransform>().localScale;
		}
		set{
			this.gameObject.GetComponent<RectTransform>().localScale = value;
		}
	}
	public virtual void Show (System.Action onFinish = null)
	{
		
		if (this.scale == Vector3.one) {
			return;
		}


		this.gameObject.SetActive (true);
		PanelWillShow ();
		this.scale = Vector3.one;
		PanelDidShow ();
		
		if (onFinish != null) {
			onFinish ();
		}
	
	}

	public virtual void Hide (System.Action onFinish = null)
	{
		
		if (this.scale == Vector3.zero) {
			return;
		}
		
		PanelWillHide ();
		this.scale = Vector3.zero;
		PanelDidHide ();
		
		this.gameObject.SetActive (false);
		
		if (onFinish != null) {
			onFinish ();
		}
		
	}

	public virtual void PanelWillPush ()
	{
	}

	protected virtual void PanelWillShow ()
	{
	}

	protected virtual void PanelDidShow ()
	{
	}

	protected virtual void PanelWillHide ()
	{
	}

	protected virtual void PanelDidHide ()
	{
	}

	#region Navigation Helpers

	public void PopSelf ()
	{
		if (this.parentStack != null) {
			this.parentStack.PopPanel (this);
		}
	}

	public void PopSelfToTop ()
	{
		this.parentStack.PopToTop ();
	}

	#endregion

}
