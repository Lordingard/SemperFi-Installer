using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
static class Integration {
 public static string DesktopOverride;
 public static object Get(object o,string name){return o.GetType().InvokeMember(name,BindingFlags.GetProperty,null,o,null);}
 static void Set(object o,string name,object value){o.GetType().InvokeMember(name,BindingFlags.SetProperty,null,o,new object[]{value});}
 public static object Shortcut(string path){object shell=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));try{return shell.GetType().InvokeMember("CreateShortcut",BindingFlags.InvokeMethod,null,shell,new object[]{path});}finally{Marshal.FinalReleaseComObject(shell);}}
 public static void NoLinks(string path){for(string p=Path.GetFullPath(path);!String.IsNullOrEmpty(p);p=Path.GetDirectoryName(p))if((Directory.Exists(p)||File.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new IOException("Les liens et jonctions ne sont pas acceptes.");}
 public static void CompleteInstallation(string folder) {
  folder=Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar);NoLinks(folder);if(folder==Path.GetPathRoot(folder).TrimEnd(Path.DirectorySeparatorChar))throw new IOException("Choisissez un dossier de jeu, pas une racine de disque.");
  if(BinaryPatch.Hash(File.ReadAllBytes(Path.Combine(folder,"SF.EXE")))!=BinaryPatch.Final)throw new IOException("Cette installation autonome n'est pas reconnue.");
  string state=Path.Combine(folder,".semperfi-install.txt"),uninstaller=Path.Combine(folder,"Desinstaller.exe");NoLinks(state);NoLinks(uninstaller);
  if(File.Exists(uninstaller)||File.Exists(state))throw new IOException("Les informations d'installation existent deja.");
  string id=Guid.NewGuid().ToString("N");
  byte[] program;using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("uninstaller.exe"))using(var buffer=new MemoryStream()){stream.CopyTo(buffer);program=buffer.ToArray();}
  string desktop=DesktopOverride??Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);NoLinks(desktop);Directory.CreateDirectory(desktop);
  string exe=Path.Combine(folder,"SF.EXE"),link=null;
  for(int n=1;n<=100;n++){
   string candidate=Path.Combine(desktop,n==1?"Semper Fi.lnk":"Semper Fi ("+n+").lnk");NoLinks(candidate);
   if(!File.Exists(candidate)){link=candidate;break;}
   object existing=Shortcut(candidate);try{if(String.Equals((string)Get(existing,"TargetPath"),exe,StringComparison.OrdinalIgnoreCase)){link=candidate;break;}}finally{Marshal.FinalReleaseComObject(existing);}
  }
  if(link==null)throw new IOException("Impossible de choisir un nom de raccourci.");
  File.WriteAllBytes(uninstaller,program);
  object shortcut=Shortcut(link);try{Set(shortcut,"TargetPath",exe);Set(shortcut,"WorkingDirectory",folder);Set(shortcut,"IconLocation",exe+",0");Set(shortcut,"Description","Semper Fi - installation autonome");shortcut.GetType().InvokeMember("Save",BindingFlags.InvokeMethod,null,shortcut,null);}finally{Marshal.FinalReleaseComObject(shortcut);}
  InstallSupport.DataPermissions(folder,InstallSupport.Owner);
  File.WriteAllLines(state,new string[]{"SemperFi-0.7",id,folder,link,InstallSupport.Owner},Encoding.UTF8);
  InstallSupport.Register(folder,id,InstallSupport.Owner);
  File.WriteAllText(Path.Combine(folder,"DESINSTALLATION.txt"),"Utiliser les applications installees de Windows ou lancer Desinstaller.exe. Les sauvegardes et preferences sont conservees par defaut dans ce dossier (SCEN, CAMPAIGN, PREFS.DAT). Les fichiers personnels non installes restent conserves. Fermer le jeu avant de le desinstaller.\r\n",Encoding.UTF8);
 }
}
