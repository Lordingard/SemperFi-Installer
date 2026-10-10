using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
class MusicPatch {
 const string Previous="624E19E1CC5BADF36423D9455CAA13E33FCF92695A0D06775C2B6D088D3F773E";
 static void NoLinks(string path){for(string p=Path.GetFullPath(path);!String.IsNullOrEmpty(p);p=Path.GetDirectoryName(p))if((File.Exists(p)||Directory.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new IOException("Les liens et jonctions ne sont pas acceptes.");}
 static string Apply(string folder,bool restore){folder=Path.GetFullPath(folder).TrimEnd('\\');NoLinks(folder);string file=Path.Combine(folder,"SF.EXE"),backup=Path.Combine(folder,"compat-backup","SF.before-music-v07.exe"),manifest=Path.Combine(folder,"installed-files.tsv");NoLinks(file);NoLinks(backup);NoLinks(manifest);
 foreach(var p in Process.GetProcessesByName("SF")){try{if(String.Equals(p.MainModule.FileName,file,StringComparison.OrdinalIgnoreCase))throw new IOException("Fermez le jeu avant d'appliquer le correctif.");}catch(System.ComponentModel.Win32Exception){throw new IOException("Fermez les jeux Semper Fi ouverts avant de continuer.");}}
 byte[] input=File.ReadAllBytes(file);string hash=BinaryPatch.Hash(input);if(hash!=Previous&&hash!=BinaryPatch.Final)throw new IOException("Ce correctif est reserve aux installations autonomes version 0.6.");
 if(hash==(restore?Previous:BinaryPatch.Final))return restore?"La version precedente est deja en place.":"Le correctif musical est deja applique.";
 byte[] result;
 if(restore){result=File.ReadAllBytes(backup);if(BinaryPatch.Hash(result)!=Previous)throw new IOException("La copie de la version precedente n'est pas reconnue.");}
 else{result=BinaryPatch.Transform(input,false);Directory.CreateDirectory(Path.GetDirectoryName(backup));if(File.Exists(backup)){if(BinaryPatch.Hash(File.ReadAllBytes(backup))!=Previous)throw new IOException("Une copie precedente differente existe deja.");}else File.WriteAllBytes(backup,input);}
 string[] rows=File.Exists(manifest)?File.ReadAllLines(manifest):null;
 string temp=file+".music-"+Guid.NewGuid().ToString("N");File.WriteAllBytes(temp,result);File.Replace(temp,file,null);
 if(rows!=null){for(int i=0;i<rows.Length;i++){var row=rows[i].Split('\t');if(row.Length==3&&row[0].Equals("SF.EXE",StringComparison.OrdinalIgnoreCase))rows[i]="SF.EXE\t"+result.Length+"\t"+BinaryPatch.Hash(result);}File.WriteAllLines(manifest,rows,Encoding.UTF8);}
 return restore?"Version 0.6 retablie.":"Musiques corrigees. Les sauvegardes et preferences sont conservees.";
 }
 [STAThread]static int Main(string[] args){bool cli=args.Length>0;try{string folder;bool restore=false;if(cli){if(args.Length!=2||(args[0]!="--apply"&&args[0]!="--restore"))throw new IOException("Usage : --apply DOSSIER | --restore DOSSIER");folder=args[1];restore=args[0]=="--restore";}else{Application.EnableVisualStyles();using(var d=new FolderBrowserDialog()){d.Description="Choisir le dossier du jeu autonome version 0.6";d.SelectedPath=InstallSupport.DefaultFolder;if(d.ShowDialog()!=DialogResult.OK)return 0;folder=d.SelectedPath;}}
 if(InstallSupport.Protected(folder)&&!InstallSupport.Administrator){int result=InstallSupport.Elevate(Assembly.GetExecutingAssembly().Location,(restore?"--restore ":"--apply ")+InstallSupport.Quote(folder));if(result!=0)throw new IOException("Correction interrompue. Consultez music-patch-error.txt a cote du correctif.");if(!cli)MessageBox.Show("Operation terminee. Vous pouvez relancer le jeu.","Semper Fi - musiques");return result;}
 string message=Apply(folder,restore);if(!cli)MessageBox.Show(message,"Semper Fi - musiques");return 0;
 }catch(Exception ex){try{File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"music-patch-error.txt"),ex.ToString());}catch{}if(!cli)MessageBox.Show(ex.Message,"Correction interrompue");return 1;}}
}
