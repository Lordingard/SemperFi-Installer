using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Threading.Tasks;
class LocalizationTests {
 static void Check(bool value,string message) { if(!value)throw new Exception(message); }
 [STAThread] static int Main(string[] args) {
  try {
   Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
   foreach(string code in new[]{"fr","en"}) {
    Language.Set(code);
    Check(Language.T("oui","yes")== (code=="fr"?"oui":"yes"),"Translation selection");
    Check(Task.Run(()=>Language.T("oui","yes")).Result==(code=="fr"?"oui":"yes"),"Background translation");
    Check(Task.Run(()=>System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName).Result==code,"Background UI culture");
    string[] parsed=Language.Parse(new[]{"--install","source","target","--lang",code});
    Check(parsed.SequenceEqual(new[]{"--install","source","target"}),"Language argument parsing");
    Check(Language.Arguments==" --lang "+code,"Elevated/worker language argument");
    Language.Save(args[1]);Language.Set(code=="fr"?"en":"fr");Language.Load(args[1]);Check(Language.Code==code,"Remembered uninstall language");
    using(var dialog=Language.CreateDialog())using(var bitmap=new System.Drawing.Bitmap(dialog.Width,dialog.Height)) {
     Check(dialog.Controls.OfType<ComboBox>().Single().SelectedIndex==(code=="fr"?0:1),"Language dialog default");
     dialog.StartPosition=FormStartPosition.Manual;dialog.Location=new System.Drawing.Point(-10000,-10000);dialog.Show();dialog.Update();
     dialog.DrawToBitmap(bitmap,new System.Drawing.Rectangle(0,0,dialog.Width,dialog.Height));bitmap.Save(Path.Combine(args[1],"language-"+code+".png"));
    }
    var assembly=Assembly.LoadFile(Path.GetFullPath(args[0]));
    assembly.GetType("Language").GetMethod("Set").Invoke(null,new object[]{code});
    using(var form=(Form)Activator.CreateInstance(assembly.GetType("Installer"),true))using(var bitmap=new System.Drawing.Bitmap(form.Width,form.Height)) {
     Check(form.Controls.OfType<Button>().Any(b=>b.Text==(code=="fr"?"Installer":"Install")),"Installer button translation");
     Check(form.Controls.OfType<Label>().Any(l=>l.Text.StartsWith(code=="fr"?"Source :":"Source:")),"Installer label translation");
     form.StartPosition=FormStartPosition.Manual;form.Location=new System.Drawing.Point(-10000,-10000);form.Show();form.Update();
     form.DrawToBitmap(bitmap,new System.Drawing.Rectangle(0,0,form.Width,form.Height));bitmap.Save(Path.Combine(args[1],"installer-"+code+".png"));
    }
   }
   foreach(string[] invalid in new[]{new[]{"--lang"},new[]{"--lang","xx"},new[]{"--lang","en","--lang","fr"}}) {
    bool rejected=false;try{Language.Parse(invalid);}catch(ArgumentException){rejected=true;}Check(rejected,"Reject invalid language option");
   }
   Console.WriteLine("French/English forms, background culture, CLI options and remembered language: OK");return 0;
  }catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
 }
}
