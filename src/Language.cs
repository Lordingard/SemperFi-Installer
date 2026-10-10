using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

// Language is explicit across elevated processes and retained for uninstall.
static class Language {
 public const string FileName = ".semperfi-language.txt";
 static string code = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr" ? "fr" : "en";
 public static string Code { get { return code; } }
 public static string Arguments { get { return " --lang " + code; } }
 public static string T(string french, string english) { return code == "fr" ? french : english; }
 public static void Set(string value) {
  if(value != "fr" && value != "en") throw new ArgumentException("Language must be fr or en / La langue doit etre fr ou en.");
  code = value;
  var culture = CultureInfo.GetCultureInfo(code == "fr" ? "fr-FR" : "en-US");
  Thread.CurrentThread.CurrentUICulture = culture;
  CultureInfo.DefaultThreadCurrentUICulture = culture;
 }
 public static string[] Parse(string[] args) {
  Set(code);
  var remaining = new List<string>(); bool found = false;
  for(int i=0;i<args.Length;i++) {
   if(args[i] != "--lang") { remaining.Add(args[i]); continue; }
   if(found || i+1 >= args.Length) throw new ArgumentException("Usage: --lang fr|en");
   Set(args[++i]); found = true;
  }
  return remaining.ToArray();
 }
 public static void Load(string folder) {
  string path = Path.Combine(folder,FileName);
  if(!File.Exists(path)) return;
  if((File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0) throw new IOException(T("Fichier de langue non valide.","Invalid language file."));
  Set(File.ReadAllText(path).Trim());
 }
 public static void Save(string folder) { File.WriteAllText(Path.Combine(folder,FileName),code,System.Text.Encoding.UTF8); }
 public static Form CreateDialog() {
  var form = new Form(); form.Text = "Semper Fi - Langue / Language";
  form.ClientSize = new Size(350,130); form.FormBorderStyle = FormBorderStyle.FixedDialog;
  form.MaximizeBox = false; form.MinimizeBox = false; form.StartPosition = FormStartPosition.CenterScreen;
  var label = new Label(); label.Text = "Choisir la langue / Choose your language"; label.SetBounds(18,15,315,22); form.Controls.Add(label);
  var choice = new ComboBox(); choice.DropDownStyle = ComboBoxStyle.DropDownList;
  choice.Items.AddRange(new object[]{"Français", "English"}); choice.SelectedIndex = code == "fr" ? 0 : 1;
  choice.SetBounds(18,42,315,25); form.Controls.Add(choice);
  var ok = new Button(); ok.Text = "OK"; ok.SetBounds(115,88,100,28); ok.DialogResult = DialogResult.OK;
  ok.Click += (s,e) => Set(choice.SelectedIndex == 0 ? "fr" : "en"); form.Controls.Add(ok); form.AcceptButton = ok;
  var cancel = new Button(); cancel.Text = "Annuler / Cancel"; cancel.SetBounds(225,88,108,28); cancel.DialogResult = DialogResult.Cancel; form.Controls.Add(cancel); form.CancelButton = cancel;
  return form;
 }
 public static bool Choose() { using(var form = CreateDialog()) return form.ShowDialog() == DialogResult.OK; }
}
