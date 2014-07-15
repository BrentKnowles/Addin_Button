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

		public override void CopyNote (NoteDataInterface Note)
		{
			base.CopyNote (Note);
			this.PrimaryDetails = ((NoteDataXML_Button)Note).PrimaryDetails;
			this.SecondaryDetails = ((NoteDataXML_Button)Note).SecondaryDetails;

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

		/// <summary>
		/// Function_s the even windows_ across monitors.
		/// 
		/// First code will be on the 1st monitor, the seocond on the next
		/// Third and further elements ignored.
		/// </summary>
		/// <param name='secondarycode'>
		/// Secondarycode.
		/// </param>
		/// <param name='alpha'>
		/// If set to <c>true</c> alpha.
		/// </param>
		void Function_EvenWindows_AcrossMonitors (string secondarycode, bool alpha)
		{
			if (secondarycode != Constants.BLANK) {
				string[] windows = GetNotesToOperateOn (secondarycode);


				if (windows.Length != 4) {
					NewMessage.Show ("The secondary string must be in the format of monitor1offset,monitor2offset,window1ToOpen,Window2ToOpen");
					return;
				}
				int overrideLeftMonitor1 = 0;
				if (Int32.TryParse (windows [0], out overrideLeftMonitor1) == false)
					NewMessage.Show (Loc.Instance.GetString ("Parse failed 1"));

				//NewMessage.Show (overrideLeftMonitor1.ToString ());

				int overrideLeftMonitor2 = 0;
				if (Int32.TryParse (windows [1], out overrideLeftMonitor2) == false)
					NewMessage.Show (Loc.Instance.GetString ("Parse failed 2"));
				//NewMessage.Show (overrideLeftMonitor2.ToString ());

				if (windows != null) {
					// we need to start the counter low because we pass 4 entries in but skip the first two
					int counter = -2;
					LayoutPanelBase MyLayout = Layout;
					GetMasterLayoutToUse (ref MyLayout);
					
					System.Windows.Forms.Screen[] currentScreen = System.Windows.Forms.Screen.AllScreens;
//					int Width = MyLayout.Width;
//					int Height = MyLayout.Height - MyLayout.HeightOfToolbars (); 
					foreach (string notename in windows) {

						if (notename != Constants.BLANK) {
							counter++;
							if (counter > 0) {
								NoteDataInterface note = MyLayout.FindNoteByName (notename);
								if (note != null) {
								
									note = MyLayout.GoToNote (note);
									if (note != null) {

										int NewWidth = 0;
										int NewHeight = 0;
										int newLeft = 0;
										int NewTop = 0;
										int WindowToUse = 0;
										// if we have only one monitor then we use it for both notes (which doesn't make sense but is all we can do0
										if (counter == 1 || currentScreen.Length == 1) {
										//	NewMessage.Show ("set to " + overrideLeftMonitor1);
											newLeft = overrideLeftMonitor1;
										} else {
											newLeft = overrideLeftMonitor2;
											// 2nd or other, will always try to refer to '2nd' screen
											WindowToUse = 1;
										}
//									string message = String.Format ("Device Name: {0}\nBounds: {1}\nType: {2}\nWorking Area: {3}",
//									                                currentScreen[WindowToUse].DeviceName, currentScreen[WindowToUse].Bounds.ToString (),
//									                                currentScreen[WindowToUse].GetType().ToString (),currentScreen[WindowToUse].WorkingArea.ToString ());
//											NewMessage.Show(message);

										int buffer = 40;//currentScreen[WindowToUse].
										//NewMessage.Show (newLeft.ToString ());
										if (0 == newLeft) {
											newLeft = currentScreen [WindowToUse].WorkingArea.Left;//+buffer;
										}
										NewTop = currentScreen [WindowToUse].WorkingArea.Top;//+buffer;
										NewWidth = currentScreen [WindowToUse].WorkingArea.Width - buffer;
										NewHeight = currentScreen [WindowToUse].WorkingArea.Height - (buffer*2);


								
										note.Width = NewWidth;
										note.Height = NewHeight;


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
		}

		void HandleActionButtonClick (object sender, EventArgs e)
		{
			if (ParentNotePanel != null) {
				ParentNotePanel.Cursor = Cursors.WaitCursor;
				for (int i = 0; i < PrimaryDetails.Count; i++) {
					string primarycode = PrimaryDetails [i];

					string secondarycode = Constants.BLANK;
					try {
						secondarycode = SecondaryDetails [i];
					} catch (Exception) {
						// somtimes this will be empty, and we just ignore it
						secondarycode = Constants.BLANK;
					}
					if (primarycode == "beep") {
						Function_Beep (secondarycode);
					} else
						if (primarycode == "evenwindows_alpha") {
						Function_EvenWindows (secondarycode, true);
					} else
							if (primarycode == "popup") {
						Function_Popup (secondarycode);
					} else
					if (primarycode == "notevisible_true") {
						Function_Visible (secondarycode, true);
					} else
						if (primarycode == "evenwindows_across_monitors_alpha") {
						Function_EvenWindows_AcrossMonitors(secondarycode, false);
					} else
					if (primarycode == "notevisible_false") {
						Function_Visible (secondarycode, false);
					} else
					if (primarycode == "hotkey") {
						// how to access this? need to access parent somehow
						// a callback probably
						// anyway to tie into HOTKEY system for this? actually access those tokens directly? Like how plugins do this?
						// idealy I'd want set to Extendview and Arrange my two current windows side by side
						LayoutDetails.Instance.ManualRunHotkeyOperation (secondarycode);
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

