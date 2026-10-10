using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
class MusicPatch {
 const string Previous="624E19E1CC5BADF36423D9455CAA13E33FCF92695A0D06775C2B6D088D3F773E";
 static void NoLinks(string path){for(string p=Path.GetFullPath(path);!String.IsNullOrEmpty(p);p=Path.GetDirectoryName(p))if((File.Exists(p)||Directory.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new IOException(Language.T("Les liens et jonctions ne sont pas acceptes.", "Links and junctions are not supported."));}
 static string Apply(string folder,bool restore){folder=Path.GetFullPath(folder).TrimEnd('\\');NoLinks(folder);string file=Path.Combine(folder,"SF.EXE"),backup=Path.Combine(folder,"compat-backup","SF.before-music-v07.exe"),manifest=Path.Combine(folder,"installed-files.tsv");NoLinks(file);NoLinks(backup);NoLinks(manifest);
 foreach(var p in Process.GetProcessesByName("SF")){try{if(String.Equals(p.MainModule.FileName,file,StringComparison.OrdinalIgnoreCase))throw new IOException(Language.T("Fermez le jeu avant d'appliquer le correctif.", "Close the game before applying the patch."));}catch(System.ComponentModel.Win32Exception){throw new IOException(Language.T("Fermez les jeux Semper Fi ouverts avant de continuer.", "Close any running Semper Fi games before continuing."));}}
 byte[] input=File.ReadAllBytes(file);string hash=BinaryPatch.Hash(input);if(hash!=Previous&&hash!=BinaryPatch.Final)throw new IOException(Language.T("Ce correctif est reserve aux installations autonomes version 0.6.", "This patch is for standalone version 0.6 installations."));
 if(hash==(restore?Previous:BinaryPatch.Final))return restore?Language.T("La version precedente est deja en place.", "The previous version is already in place."):Language.T("Le correctif musical est deja applique.", "The music patch is already applied.");
 byte[] result;
 if(restore){result=File.ReadAllBytes(backup);if(BinaryPatch.Hash(result)!=Previous)throw new IOException(Language.T("La copie de la version precedente n'est pas reconnue.", "The backup of the previous version is not recognized."));}
 else{result=BinaryPatch.Transform(input,false);Directory.CreateDirectory(Path.GetDirectoryName(backup));if(File.Exists(backup)){if(BinaryPatch.Hash(File.ReadAllBytes(backup))!=Previous)throw new IOException(Language.T("Une copie precedente differente existe deja.", "A different previous backup already exists."));}else File.WriteAllBytes(backup,input);}
 string[] rows=File.Exists(manifest)?File.ReadAllLines(manifest):null;
 string temp=file+".music-"+Guid.NewGuid().ToString("N");File.WriteAllBytes(temp,result);File.Replace(temp,file,null);
 if(rows!=null){for(int i=0;i<rows.Length;i++){var row=rows[i].Split('\t');if(row.Length==3&&row[0].Equals("SF.EXE",StringComparison.OrdinalIgnoreCase))rows[i]="SF.EXE\t"+result.Length+"\t"+BinaryPatch.Hash(result);}File.WriteAllLines(manifest,rows,Encoding.UTF8);}
 return restore?Language.T("Version 0.6 retablie.", "Version 0.6 restored."):Language.T("Musiques corrigees. Les sauvegardes et preferences sont conservees.", "Music fixed. Saved games and preferences have been retained.");
 }
 [STAThread]static int Main(string[] args){bool explicitLanguage=Array.IndexOf(args,"--lang")>=0;bool cli=args.Length>0;try{args=Language.Parse(args);cli=args.Length>0;string folder;bool restore=false;if(cli){if(args.Length!=2||(args[0]!="--apply"&&args[0]!="--restore"))throw new IOException(Language.T("Usage : --apply DOSSIER | --restore DOSSIER", "Usage: --apply FOLDER | --restore FOLDER"));folder=args[1];restore=args[0]=="--restore";}else{Application.EnableVisualStyles();if(!explicitLanguage&&!Language.Choose())return 0;using(var d=new FolderBrowserDialog()){d.Description=Language.T("Choisir le dossier du jeu autonome version 0.6", "Choose the standalone version 0.6 game folder");d.SelectedPath=InstallSupport.DefaultFolder;if(d.ShowDialog()!=DialogResult.OK)return 0;folder=d.SelectedPath;}}
 if(InstallSupport.Protected(folder)&&!InstallSupport.Administrator){int result=InstallSupport.Elevate(Assembly.GetExecutingAssembly().Location,(restore?"--restore ":"--apply ")+InstallSupport.Quote(folder)+Language.Arguments);if(result!=0)throw new IOException(Language.T("Correction interrompue. Consultez music-patch-error.txt a cote du correctif.", "Patch interrupted. See music-patch-error.txt next to the patch tool."));if(!cli)MessageBox.Show(Language.T("Operation terminee. Vous pouvez relancer le jeu.", "Operation complete. You can restart the game."),Language.T("Semper Fi - musiques", "Semper Fi - music"));return result;}
 string message=Apply(folder,restore);if(!cli)MessageBox.Show(message,Language.T("Semper Fi - musiques", "Semper Fi - music"));return 0;
 }catch(Exception ex){try{File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"music-patch-error.txt"),ex.ToString());}catch{}if(!cli)MessageBox.Show(ex.Message,Language.T("Correction interrompue", "Patch interrupted"));return 1;}}
}
