using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

class Installer : Form {
    TextBox source = new TextBox(), target = new TextBox();
    Button install = new Button();
    Label status = new Label();
    bool busy;
    static string Base = AppDomain.CurrentDomain.BaseDirectory;
    public Installer() {
        Text = Language.T("Semper Fi - Installation moderne (version autonome 0.8)", "Semper Fi - Modern installer (standalone version 0.8)");
        ClientSize = new Size(680, 290); FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; StartPosition = FormStartPosition.CenterScreen;
        AddLabel(Language.T("Installe les fichiers originaux sans lancer le setup 16 bits.", "Installs the original files without running the 16-bit setup."), 15, 15, 640);
        AddLabel(Language.T("Source : ISO du jeu ou dossier contenant SF.EXE", "Source: game ISO or folder containing SF.EXE"), 15, 55, 640);
        source.SetBounds(15, 80, 455, 25); source.Text = File.Exists(Path.Combine(Base,"SEMPERFI.iso"))?Path.Combine(Base,"SEMPERFI.iso"):Path.Combine(Base, "SEMPERFI"); Controls.Add(source);
        AddSourceBrowse();
        AddLabel(Language.T("Destination : un nouveau dossier (modifiable)", "Destination: a new folder (you can change this)"), 15, 115, 640);
        target.SetBounds(15, 140, 555, 25); target.Text = InstallSupport.DefaultFolder; Controls.Add(target);
        AddBrowse(target, 580, 139);
        install.Text = Language.T("Installer", "Install"); install.SetBounds(15, 185, 140, 32); Controls.Add(install);
        status.SetBounds(15, 232, 645, 45); status.Text = Language.T("Videos et musiques installees localement. Aucun CD requis apres installation.", "Videos and music are installed locally. No CD required after installation."); Controls.Add(status);
        install.Click += async (s,e) => {
            busy = true; foreach(Control c in Controls) c.Enabled = false;
            try {
                string src = source.Text, dst = target.Text;
                await Task.Run(() => RunInstall(src, dst, m => BeginInvoke((Action)(() => status.Text = m))));
                status.Text = Language.T("Installation verifiee. Raccourci cree sur le Bureau.", "Installation verified. Desktop shortcut created.");
                MessageBox.Show(this, Language.T("Fichiers installes et verifies.\nL'ISO ou le CD n'est plus requis pour jouer.\nLancez Semper Fi depuis le Bureau. Pour le retirer, utilisez les applications installees de Windows ou Desinstaller.exe.", "Files installed and verified.\nThe ISO or CD is no longer required to play.\nStart Semper Fi from your desktop. To remove it, use Windows installed apps or Desinstaller.exe."), Language.T("Installation terminee", "Installation complete"));
            } catch(Exception ex) { status.Text = Language.T("Echec : ", "Failed: ") + ex.Message; MessageBox.Show(this, ex.Message, Language.T("Installation interrompue", "Installation interrupted")); }
            finally { busy = false; foreach(Control c in Controls) c.Enabled = true; }
        };
        FormClosing += (s,e) => { if(busy) e.Cancel = true; };
    }
    void AddLabel(string text, int x, int y, int w) { var l=new Label(); l.Text=text; l.SetBounds(x,y,w,22); Controls.Add(l); }
    void AddBrowse(TextBox box, int x, int y) {
        var b=new Button(); b.Text=Language.T("Parcourir", "Browse"); b.SetBounds(x,y,85,27); Controls.Add(b);
        b.Click += (s,e) => { using(var d=new FolderBrowserDialog()) { d.SelectedPath=box.Text; d.Description=Language.T("Choisir le dossier", "Choose a folder"); if(d.ShowDialog()==DialogResult.OK) box.Text=d.SelectedPath; } };
    }
    void AddSourceBrowse() {
        var isoButton=new Button();isoButton.Text="ISO...";isoButton.SetBounds(480,79,85,27);Controls.Add(isoButton);
        isoButton.Click+=(s,e)=>{using(var d=new OpenFileDialog()){d.Filter=Language.T("Image ISO (*.iso)|*.iso", "ISO image (*.iso)|*.iso");d.Title=Language.T("Choisir l'ISO de Semper Fi", "Choose the Semper Fi ISO");if(d.ShowDialog()==DialogResult.OK)source.Text=d.FileName;}};
        var folderButton=new Button();folderButton.Text=Language.T("Dossier...", "Folder...");folderButton.SetBounds(575,79,90,27);Controls.Add(folderButton);
        folderButton.Click+=(s,e)=>{using(var d=new FolderBrowserDialog()){d.Description=Language.T("Dossier contenant SF.EXE", "Folder containing SF.EXE");if(d.ShowDialog()==DialogResult.OK)source.Text=d.SelectedPath;}};
    }
    static string Hash(string file) { using(var sha=SHA256.Create()) using(var stream=File.OpenRead(file)) return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", ""); }
    static string[] Manifest() {
        using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.tsv"))
        using(var r=new StreamReader(s)) return r.ReadToEnd().Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);
    }
    static bool Within(string child, string parent) { return child.StartsWith(parent.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase); }
    static void NoLinks(string path) {
        for(string p=Path.GetFullPath(path); !String.IsNullOrEmpty(p); p=Path.GetDirectoryName(p))
            if((File.Exists(p)||Directory.Exists(p)) && (File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)
                throw new IOException(Language.T("Les liens et jonctions ne sont pas acceptes : ", "Links and junctions are not supported: ")+p);
    }
    static void RunInstall(string src,string dst,Action<string> progress) {
        if(InstallSupport.Protected(dst)&&!InstallSupport.Administrator) {
            progress(Language.T("Autorisation administrateur pour installer dans ce dossier...", "Administrator permission required to install in this folder..."));
            string desktop=Integration.DesktopOverride??Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            int result=InstallSupport.Elevate(Assembly.GetExecutingAssembly().Location,"--install "+InstallSupport.Quote(Path.GetFullPath(src))+" "+InstallSupport.Quote(Path.GetFullPath(dst))+" --desktop "+InstallSupport.Quote(desktop)+" --owner "+InstallSupport.Quote(InstallSupport.Owner)+Language.Arguments);
            if(result!=0)throw new IOException(Language.T("L'installation avec les droits administrateur a echoue. Consultez installer-error.txt a cote de l'installateur.", "Installation with administrator permissions failed. See installer-error.txt next to the installer."));
        } else Install(src,dst,progress);
    }
    static void Install(string src, string dst, Action<string> progress) {
        src=Path.GetFullPath(src); dst=Path.GetFullPath(dst).TrimEnd(Path.DirectorySeparatorChar);
        GameSource input=new GameSource(src);
        NoLinks(src); NoLinks(dst);
        if(dst==Path.GetPathRoot(dst).TrimEnd(Path.DirectorySeparatorChar) || String.Equals(src,dst,StringComparison.OrdinalIgnoreCase) || Within(dst,src) || Within(src,dst))
            throw new IOException(Language.T("Choisissez une destination distincte de la source et d'une racine de disque.", "Choose a destination separate from the source and from a drive root."));
        if(Directory.Exists(dst)||File.Exists(dst)) throw new IOException(Language.T("La destination existe deja. Choisissez un nouveau dossier pour proteger les sauvegardes.", "The destination already exists. Choose a new folder to protect saved games."));
        var rows=Manifest().Select(l=>l.Split('\t')).ToArray();
        progress(Language.T("Verification des fichiers du CD...", "Checking the CD files..."));
        foreach(var row in rows) {
            if(!String.Equals(input.FileHash(row[0]),row[2],StringComparison.OrdinalIgnoreCase))
                throw new IOException(Language.T("Source manquante ou differente de l'edition attendue : ", "Source missing or different from the supported edition: ")+row[0]);
        }
        string stage=dst+".installation-"+Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(stage);
        using(var log=new StreamWriter(Path.Combine(stage,"installation.log"),false,Encoding.UTF8)) {
            log.AutoFlush=true; log.WriteLine("Semper Fi installer autonome 0.8 - "+DateTime.UtcNow.ToString("o")); log.WriteLine("Source: "+src);
            try {
                int i=0;
                foreach(var row in rows) {
                    string file=Path.Combine(stage,row[0]); Directory.CreateDirectory(Path.GetDirectoryName(file));
                    input.Copy(row[0],file); File.SetAttributes(file,FileAttributes.Normal);
                    if(!String.Equals(Hash(file),row[2],StringComparison.OrdinalIgnoreCase)) throw new IOException(Language.T("Verification echouee : ", "Verification failed: ")+row[0]);
                    log.WriteLine("OK\t"+row[0]+"\t"+row[2]); progress(Language.T("Copie et verification : ", "Copying and verifying: ")+(++i)+" / "+rows.Length);
                }
                Compatibility.Apply(stage,log); Compatibility.WriteManifest(stage,Manifest());
                File.WriteAllText(Path.Combine(stage,"Jouer.cmd"),"@echo off\r\ncd /d \"%~dp0\"\r\nstart \"\" /wait \"%~dp0SF.EXE\"\r\n",Encoding.ASCII);
                File.WriteAllText(Path.Combine(stage,"INSTALLATION.txt"),Language.T("Installation locale du jeu original, edition du CD fourni.\r\nLancer Jouer.cmd. Les videos et musiques sont locales : aucun CD requis.\r\nCorrections de campagne, d'icones et d'acces aux medias locaux incluses ; DDrawCompat v0.7.1.\r\nLes sauvegardes et preferences sont dans ce dossier : le conserver avant toute suppression.\r\nDesinstaller.exe permet de retirer le jeu en choisissant de conserver les sauvegardes et preferences. Les anciens pilotes DirectX, le setup et les demos ne sont pas installes.\r\n", "Local installation of the original game from the supplied CD edition.\r\nRun Jouer.cmd or the desktop shortcut. Videos and music are local: no CD required.\r\nCampaign, icon and local media fixes included; DDrawCompat v0.7.1.\r\nSaved games and preferences are in this folder: back them up before deleting it.\r\nDesinstaller.exe removes the game with a choice to retain saved games and preferences. Legacy DirectX drivers, setup and demos are not installed.\r\n"),Encoding.UTF8);
                log.WriteLine("VERIFIED "+rows.Length+" files");
            } catch { log.WriteLine(Language.T("FAILED - dossier temporaire conserve pour diagnostic", "FAILED - temporary folder retained for diagnostics")); throw; }
        }
        Directory.Move(stage,dst);
        try{Integration.CompleteInstallation(dst);}catch(Exception ex){throw new IOException(Language.T("Le jeu est installe dans ", "The game is installed in ")+dst+Language.T(", mais l'integration Windows n'a pas pu etre ajoute. Details : ", ", but Windows integration could not be added. Details: ")+ex.Message,ex);}
    }
    [STAThread] static int Main(string[] args) {
        bool explicitLanguage=Array.IndexOf(args,"--lang")>=0;
        try { args=Language.Parse(args); } catch(Exception ex) { try{File.WriteAllText(Path.Combine(Base,"installer-error.txt"),ex.ToString());}catch{} return 1; }
        if(args.Length>0) {
            try {
                if(args.Length>=3 && args[0]=="--install") {
                    for(int i=3;i<args.Length;i+=2){if(i+1>=args.Length)throw new ArgumentException(Language.T("Option incomplete.", "Incomplete option."));if(args[i]=="--desktop")Integration.DesktopOverride=args[i+1];else if(args[i]=="--owner"&&InstallSupport.Administrator)InstallSupport.Owner=args[i+1];else throw new ArgumentException(Language.T("Option inconnue.", "Unknown option."));}
                    RunInstall(args[1],args[2],m=>Console.WriteLine(m));return 0;
                }
                if(args.Length==2 && args[0]=="--verify") {
                    foreach(var row in Manifest().Select(l=>l.Split('\t'))) if(Hash(Path.Combine(args[1],row[0]))!=Compatibility.Expected(row[0],row[2])) throw new IOException(Language.T("Fichier modifie : ", "Modified file: ")+row[0]); Compatibility.Verify(args[1]);
                    return 0;
                }
                throw new ArgumentException("Usage: --install SOURCE DESTINATION | --verify DESTINATION");
            } catch(Exception ex) { Console.Error.WriteLine(ex.Message); try{File.WriteAllText(Path.Combine(Base,"installer-error.txt"),ex.ToString());}catch{} return 1; }
        }
        Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); if(!explicitLanguage && !Language.Choose()) return 0; Application.Run(new Installer()); return 0;
    }
}
