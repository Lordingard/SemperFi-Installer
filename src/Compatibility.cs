using System;
using System.IO;
using System.Reflection;
using System.Text;
static class Compatibility {
 public const string SfHash=BinaryPatch.Final;
 public const string DdrawHash="F75F0AC48D2782F225C483DC2F1142303A513E8DD8A60793891ADE89F64755EA";
 public const string Config="GdiInterops = all\r\n";
 public static string Expected(string name,string original){return name.Equals("SF.EXE",StringComparison.OrdinalIgnoreCase)?SfHash:original;}
 static void Backup(string file,string backup){if(File.Exists(file)&&!File.Exists(backup))File.Copy(file,backup);}
 public static void Apply(string folder,StreamWriter log) {
  string file=Path.Combine(folder,"SF.EXE"),backupFolder=Path.Combine(folder,"compat-backup");
  byte[] before=File.ReadAllBytes(file),sf=BinaryPatch.Transform(before,false);
  Directory.CreateDirectory(backupFolder);string backup=Path.Combine(backupFolder,"SF.original.exe");
  if(!File.Exists(backup))File.WriteAllBytes(backup,BinaryPatch.Transform(before,true));
  else if(BinaryPatch.Hash(File.ReadAllBytes(backup))!=BinaryPatch.Original)throw new IOException("Sauvegarde originale differente.");
  byte[] dll;
  using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("ddraw.dll"))using(var m=new MemoryStream()){s.CopyTo(m);dll=m.ToArray();}
  if(BinaryPatch.Hash(dll)!=DdrawHash)throw new IOException("Bibliotheque graphique non conforme.");
  string dllPath=Path.Combine(folder,"ddraw.dll"),configPath=Path.Combine(folder,"DDrawCompat-SF.ini");
  Backup(dllPath,Path.Combine(backupFolder,"ddraw.before-release.dll"));Backup(configPath,Path.Combine(backupFolder,"DDrawCompat.before-release.ini"));
  string temp=file+".patch-"+Guid.NewGuid().ToString("N");File.WriteAllBytes(temp,sf);File.Replace(temp,file,null);
  File.WriteAllBytes(dllPath,dll);File.WriteAllText(configPath,Config,Encoding.ASCII);
  log.WriteLine("PATCH SF.EXE "+SfHash+" - campaign path overflow and six tab bitmaps");log.WriteLine("GRAPHICS DDrawCompat v0.7.1 GdiInterops=all "+DdrawHash);
 }
 public static void WriteManifest(string folder,string[] source) {
  File.WriteAllLines(Path.Combine(folder,"source-files.tsv"),source,Encoding.UTF8);
  var rows=new System.Collections.Generic.List<string>();
  foreach(string line in source){string name=line.Split('\t')[0];string file=Path.Combine(folder,name);rows.Add(name+"\t"+new FileInfo(file).Length+"\t"+BinaryPatch.Hash(File.ReadAllBytes(file)));}
  foreach(string name in new string[]{"ddraw.dll","DDrawCompat-SF.ini"}){string file=Path.Combine(folder,name);rows.Add(name+"\t"+new FileInfo(file).Length+"\t"+BinaryPatch.Hash(File.ReadAllBytes(file)));}
  File.WriteAllLines(Path.Combine(folder,"installed-files.tsv"),rows.ToArray(),Encoding.UTF8);
 }
 public static void Verify(string folder) {
  if(BinaryPatch.Hash(File.ReadAllBytes(Path.Combine(folder,"ddraw.dll")))!=DdrawHash)throw new IOException("Bibliotheque graphique modifiee.");
  if(File.ReadAllText(Path.Combine(folder,"DDrawCompat-SF.ini"))!=Config)throw new IOException("Configuration graphique modifiee.");
 }
}
