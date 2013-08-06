using System;
using CoreUtilities;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using CoreUtilities.Links;
using Layout;
using System.ComponentModel;
//using System.Collections.Generic;



namespace ADD_Button
{
	public class NoteDataXML_Button:  NoteDataXML
	{
		#region constants
		
		//	public const string NotUsed = "Modifier";
#endregion
		#region interface
		ToolStripButton ActionButton = null;
#endregion
		
		#region properties




	private List<string> primaryDetails=null;
		/// <summary>
		/// Gets or sets the primary details. // This is actually an Array but only the 'first value' is handled via the menu. The rest has to be edited via the Properties.
		/// </summary>
		/// <value>
		/// The primary details.
		/// </value>
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		public List<string> PrimaryDetails {
			get {
				return primaryDetails;
			}
			set {
				primaryDetails = value;
			}
		}

		private List<string> secondaryDetails= null;
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		public List<string> SecondaryDetails {
			get {
				return secondaryDetails;
			}
			set {
				secondaryDetails = value;
			}
		}
		
#endregion
		
		
		public NoteDataXML_Button () : base()
		{
			CommonConstructorBehavior ();
		}
		public NoteDataXML_Button(int height, int width):base(height, width)
		{
			CommonConstructorBehavior ();

		}
		public NoteDataXML_Button(NoteDataInterface Note) : base(Note)
		{
			//this.Notelink = ((NoteDataXML_Checklist)Note).Notelink;
		}
		protected override void CommonConstructorBehavior ()
		{
			base.CommonConstructorBehavior ();
			Caption = Loc.Instance.GetString ("Button");
						if (primaryDetails == null) {
							primaryDetails = new System.Collections.Generic.List<string> ();
						//	primaryDetails.Add ("beep");
						}
						if (secondaryDetails == null) {
							secondaryDetails = new System.Collections.Generic.List<string>();
						//	secondaryDetails.Add (Constants.BLANK);
						}

		}
		
		/// <summary>
		/// Registers the type.
		/// </summary>
		public override string RegisterType()
		{
			return Loc.Instance.GetString("Button");
		}
		
		
		protected override void CaptionChanged (string cap)
		{
			base.CaptionChanged (cap);
			ActionButton.Text = cap;

		}
		protected override void DoBuildChildren (LayoutPanelBase Layout)
		{
			base.DoBuildChildren (Layout);
		
			if (PrimaryDetails.Count == 0) {
				PrimaryDetails.Add ("beep");
			}
			if (SecondaryDetails.Count == 0) {
				SecondaryDetails.Add (Constants.BLANK);
			}

			
			ParentNotePanel.Height = this.CaptionLabel.Height;
			this.MaximizeButton.Visible = false;
			this.MinimizeButton.Visible = false;
			this.captionLabel.Visible = false;
			 ActionButton = new ToolStripButton();
			ActionButton.Text = this.captionLabel.Text;
			ActionButton.Click+= HandleActionButtonClick;
			CaptionLabel.Items.Insert (0, ActionButton);

			ToolStripMenuItem PrimaryDetailLink = 
				LayoutDetails.BuildMenuPropertyEdit (Loc.Instance.GetString("Primary: {0}"), 
				                                     PrimaryDetails[0],
				                                     Loc.Instance.GetString ("Enter main action (Beep, evenwindows_alpha)."),HandlePrimaryChange );

			ToolStripMenuItem SecondaryDetailLink = 
				LayoutDetails.BuildMenuPropertyEdit (Loc.Instance.GetString("Secondary: {0}"), 
				                                     SecondaryDetails[0],
				                                     Loc.Instance.GetString ("Enter secondary action (Comma delimited list of notes for evenwindows_alpha)."),
				                                     HandleSecondaryChange );

			properties.DropDownItems.Add (new ToolStripSeparator());
			properties.DropDownItems.Add (PrimaryDetailLink);
			properties.DropDownItems.Add (SecondaryDetailLink);
		}
		void HandlePrimaryChange (object sender, KeyEventArgs e)
		{
			string tablecaption = PrimaryDetails[0];
			LayoutDetails.HandleMenuLabelEdit (sender, e, ref tablecaption, SetSaveRequired);
			PrimaryDetails[0] = tablecaption;
		}
		void HandleSecondaryChange (object sender, KeyEventArgs e)
		{
			string tablecaption = SecondaryDetails[0];
			LayoutDetails.HandleMenuLabelEdit (sender, e, ref tablecaption, SetSaveRequired);
			SecondaryDetails[0] = tablecaption;
		}
		void Function_Beep(string secondary)
		{
			Console.Beep ();
		}

		static void GetMasterLayoutToUse (ref LayoutPanelBase MyLayout)
		{
			if (MyLayout.GetIsChild == true) {
				LayoutPanelBase MyPossibleLayout = MyLayout.GetAbsoluteParent ();
				if (MyPossibleLayout != null) {
					if (MyPossibleLayout.GetIsSystemLayout == true) {
						// we only override if we have found the system layout? Will this fix
						// the issue of not finding correct width of immediate parent?
						// IF NOT: only revert to Absolute Layout if a note is not found
						MyLayout = MyPossibleLayout;
					}
				}
			}
		}
		string[] GetNotesToOperateOn (string secondary)
		{
			return secondary.Split (new char[1] {','}, StringSplitOptions.RemoveEmptyEntries);
		}


		void Function_EvenWindows (string secondary, bool alpha)
		{
			if (secondary != Constants.BLANK) {
				string[] windows = GetNotesToOperateOn(secondary);
				if (windows != null) {
					int counter = 0;
					LayoutPanelBase MyLayout = Layout;
					GetMasterLayoutToUse (ref MyLayout);

					// initial implementation just assuming 2 notes and will split 50/50
					int Width = MyLayout.Width;
					int Height = MyLayout.Height - MyLayout.HeightOfToolbars (); 
					foreach (string notename in windows) {
						if (notename != Constants.BLANK) {
							counter++;
							NoteDataInterface note = MyLayout.FindNoteByName (notename);
							if (note != null) {
								
								note = MyLayout.GoToNote (note);
								if (note != null) {
									int NewWidth = Width / windows.Length;
									note.Width = NewWidth;
									note.Height = Height;
									
									//note.ParentNotePanel.Top =0;
									int NewTop = 0;
									int newLeft = 0;
									if (1 == counter) {
										newLeft = 0;
										
									} else {
										newLeft = ((counter - 1) * NewWidth);
										//											NewMessage.Show ("Setting " + note.Caption + " " + newLeft.ToString ());
										//											note.ParentNotePanel.Left = newLeft;
										
									}
									if (note is NoteDataXML_SystemOnly) {
										//NewMessage.Show ("System Note");
										note.Maximize (true);
										note.Maximize (false);
									}
									note.Location = new Point (newLeft, NewTop);
									note.UpdateLocation ();
								}
							}
						}
					}
				}
			}
		}

		void Function_Popup(string secondary)
		{
			NewMessage.Show (Loc.Instance.GetString ("Message:"), secondary);
		}

		void Function_Visible(string secondary, bool visible)
		{
			if (secondary != Constants.BLANK) {
				string[] windows = GetNotesToOperateOn(secondary);
				if (windows != null) {
					int counter = 0;
					LayoutPanelBase MyLayout = Layout;
					GetMasterLayoutToUse (ref MyLayout);
					
					foreach (string notename in windows) {
						if (notename != Constants.BLANK) {
							counter++;
							NoteDataInterface note = MyLayout.FindNoteByName (notename);
							if (note != null) {
								
								note = MyLayout.GoToNote (note);
								if (note != null) {
									note.Visible = visible;
									note.UpdateLocation ();
								}
							}
						}
					}
				}
			}
		}
		protected override string GetIcon ()
		{
			return @"%*control_play.png";
		}
		void HandleActionButtonClick (object sender, EventArgs e)
		{
			if (ParentNotePanel != null) {
				ParentNotePanel.Cursor = Cursors.WaitCursor;
				for (int i = 0; i < PrimaryDetails.Count; i++) {
					string primarycode = PrimaryDetails [i];

					string secondarycode = Constants.BLANK;
					try
					{
						secondarycode = SecondaryDetails[i];
					}
					catch (Exception)
					{
						// somtimes this will be empty, and we just ignore it
						secondarycode = Constants.BLANK;
					}
					if (primarycode == "beep") {
						Function_Beep (secondarycode);
					}
					else
						if (primarycode == "evenwindows_alpha") {
							Function_EvenWindows (secondarycode,true);
						}
						else
							if (primarycode == "popup") {
								Function_Popup (secondarycode);
							}
					else
					if (primarycode == "notevisible_true") {
						Function_Visible (secondarycode, true);
					}
					else
					if (primarycode == "notevisible_false") {
						Function_Visible (secondarycode, false);
					}
					if (primarycode == "hotkey") {
						// how to access this? need to access parent somehow
						// a callback probably
						// anyway to tie into HOTKEY system for this? actually access those tokens directly? Like how plugins do this?
						// idealy I'd want set to Extendview and Arrange my two current windows side by side
						LayoutDetails.Instance.ManualRunHotkeyOperation(secondarycode);
					}
				}


				ParentNotePanel.Cursor = Cursors.Default;
			}
		}
		protected override void DoChildAppearance (AppearanceClass app)
		{
			base.DoChildAppearance (app);
			
			
		}
		
		
		
		
		
		
		
		public override void Save ()
		{
			
			
			
			
			base.Save ();
			
			
			
		}
		
	}
}

