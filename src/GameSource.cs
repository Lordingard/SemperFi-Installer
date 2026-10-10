using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
sealed class GameSource {
 readonly string source; readonly bool iso;
 readonly Dictionary<string,Tuple<long,int>> files=new Dictionary<string,Tuple<long,int>>(StringComparer.OrdinalIgnoreCase);
 readonly HashSet<long> visited=new HashSet<long>();
 static void NoLinks(string path){for(string p=Path.GetFullPath(path);!String.IsNullOrEmpty(p);p=Path.GetDirectoryName(p))if((File.Exists(p)||Directory.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new IOException("Les liens et jonctions ne sont pas acceptes.");}
 public GameSource(string path) {
  source=Path.GetFullPath(path);NoLinks(source);if(Directory.Exists(source))return;
  if(!File.Exists(source)||!source.EndsWith(".iso",StringComparison.OrdinalIgnoreCase))throw new IOException("Choisissez l'ISO du jeu ou le dossier contenant SF.EXE.");iso=true;
  using(var stream=File.OpenRead(source)){bool found=false;for(int sector=16;sector<64;sector++){
   byte[] descriptor=ReadAt(stream,(long)sector*2048,2048);if(Encoding.ASCII.GetString(descriptor,1,5)!="CD001")throw new IOException("Format ISO 9660 non reconnu.");
   if(descriptor[0]==1){int size=descriptor[156];if(size<34)throw new IOException("Racine ISO invalide.");byte[] root=new byte[size];Buffer.BlockCopy(descriptor,156,root,0,size);Scan(stream,root,"",0);found=true;break;}if(descriptor[0]==255)break;
  }if(!found)throw new IOException("Volume ISO 9660 introuvable.");}
 }
 static byte[] ReadAt(FileStream stream,long offset,int length){if(offset<0||length<0||offset>stream.Length-length)throw new IOException("Donnees ISO hors limites.");byte[] b=new byte[length];stream.Position=offset;int n=0;while(n<length){int got=stream.Read(b,n,length-n);if(got==0)throw new EndOfStreamException();n+=got;}return b;}
 static Tuple<long,int> Location(byte[] record){uint length=BitConverter.ToUInt32(record,10);if(length>268435456)throw new IOException("Fichier ISO trop volumineux.");return Tuple.Create((long)BitConverter.ToUInt32(record,2)*2048,(int)length);}
 void Scan(FileStream stream,byte[] record,string prefix,int depth){
  if(depth>20||files.Count>100000)throw new IOException("Arborescence ISO trop complexe.");var location=Location(record);if(location.Item2>16777216||!visited.Add(location.Item1))throw new IOException("Repertoire ISO invalide.");
  byte[] directory=ReadAt(stream,location.Item1,location.Item2);int offset=0;while(offset<directory.Length){int length=directory[offset];if(length==0){offset=(offset/2048+1)*2048;continue;}if(length<34||offset+length>directory.Length)throw new IOException("Entree ISO invalide.");
   byte[] entry=new byte[length];Buffer.BlockCopy(directory,offset,entry,0,length);offset+=length;int nameLength=entry[32];if(nameLength==0||33+nameLength>length)throw new IOException("Nom ISO invalide.");if(nameLength==1&&(entry[33]==0||entry[33]==1))continue;
   string name=Encoding.ASCII.GetString(entry,33,nameLength).Split(';')[0].TrimEnd('.');if(name==""||name=="."||name==".."||name.IndexOfAny(new char[]{'\\','/',':','\0'})>=0)throw new IOException("Chemin ISO invalide.");if((entry[25]&0x80)!=0)throw new IOException("ISO multi-extents non pris en charge.");string full=prefix+name;
   if((entry[25]&2)!=0)Scan(stream,entry,full+"/",depth+1);else{var extent=Location(entry);if(extent.Item1>stream.Length-extent.Item2||files.ContainsKey(full))throw new IOException("Fichier ISO invalide ou duplique.");files.Add(full,extent);}
  }
 }
 public byte[] Read(string relative){if(!iso){string file=Path.Combine(source,relative);NoLinks(file);return File.ReadAllBytes(file);}Tuple<long,int> location;if(!files.TryGetValue(relative.Replace('\\','/'),out location))throw new IOException("Fichier absent de l'ISO : "+relative);using(var stream=File.OpenRead(source))return ReadAt(stream,location.Item1,location.Item2);}
 public string FileHash(string name){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Read(name))).Replace("-","");}
 public void Copy(string name,string destination){File.WriteAllBytes(destination,Read(name));}
}
