using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
static class BinaryPatch {
 public const string Original="575F85F4FFE843FF7B49CDD7C8EF3EC9993843CA60ADA16D832B7D5279C9AA26";
 public const string Final="E46DBA94E8CECEB313465A1FE4EB824BE1FAAC864CDB6399A4DF8A7097F75EB9";
 public static string Hash(byte[] b){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","");}
 public static byte[] Transform(byte[] input,bool restore) {
  string hash=Hash(input);
  if(hash!=Original&&hash!=Final&&hash!="624E19E1CC5BADF36423D9455CAA13E33FCF92695A0D06775C2B6D088D3F773E"&&hash!="5B560C9EF4EDAC41C9E3B0DD620E42CD91E84B153DAF66E5558BDF69FE0C5685"&&hash!="C66652F1A902BF67CA0D25E10B8A7C4C9E2580F148C42BAC750CB861A60F10F1"&&hash!="A275192E6FB74DC9D990D7D98613C42D9961739330E3E93C9D59C84C262186AA")throw new IOException("Edition de SF.EXE non prise en charge ; aucun fichier modifie.");
  if(hash==(restore?Original:Final))return input;
  using(Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("sf-compat.patch"))using(BinaryReader reader=new BinaryReader(stream)) {
   if(System.Text.Encoding.ASCII.GetString(reader.ReadBytes(8))!="SFPATCH1")throw new IOException("Correctif invalide.");
   int originalLength=reader.ReadInt32(),finalLength=reader.ReadInt32(),count=reader.ReadInt32();
   if(originalLength<1||finalLength<originalLength||finalLength>4000000||count<1||count>1000)throw new IOException("Taille de correctif invalide.");
   byte[] original=new byte[originalLength];Buffer.BlockCopy(input,0,original,0,Math.Min(input.Length,original.Length));
   byte[] result=new byte[finalLength];Buffer.BlockCopy(input,0,result,0,Math.Min(input.Length,result.Length));
   for(int n=0;n<count;n++) {
    int offset=reader.ReadInt32(),oldLength=reader.ReadInt32(),newLength=reader.ReadInt32();
    if(offset<0||oldLength<0||newLength<0||(long)offset+oldLength>originalLength||(long)offset+newLength>finalLength)throw new IOException("Zone de correctif invalide.");
    byte[] oldBytes=reader.ReadBytes(oldLength),newBytes=reader.ReadBytes(newLength);
    if(oldBytes.Length!=oldLength||newBytes.Length!=newLength)throw new IOException("Correctif tronque.");
    Buffer.BlockCopy(oldBytes,0,original,offset,oldLength);Buffer.BlockCopy(newBytes,0,result,offset,newLength);
   }
   if(Hash(original)!=Original)throw new IOException("Reconstitution de l'original echouee.");
   // Unchanged bytes are identical across all accepted revisions.
   if(Hash(result)!=Final)throw new IOException("Verification du correctif echouee.");
   return restore?original:result;
  }
 }
}
