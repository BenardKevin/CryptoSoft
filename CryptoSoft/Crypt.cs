using System.IO;

namespace CryptoSoft
{
    public class Crypt
    {
        static void Main(string[] args)
        {
                // get arg [0]
                FileInfo file = new FileInfo(args[0]);

                // get arg [1]
                DirectoryInfo targetDirectory = new DirectoryInfo(args[1]);

                // get arg [0]
                //byte[] key = Encoding.ASCII.GetBytes(args[0]);

                // read the file in a stream
                FileStream streamFile = file.OpenRead();

                // create a buffer with the stream
                byte[] buffer = new byte[streamFile.Length];

                // read a block of bytes from the stream
                streamFile.Read(buffer, 0, buffer.Length);

                // encrypt/decrypte the buffer
                byte[] crypted_buffer = EncryptOrDecryptByte(buffer);

                // close the stream
                streamFile.Close();

                // create a new file in the directory target
                FileInfo newFile = new FileInfo(Path.Combine(targetDirectory.FullName, file.Name));

                // open the new file in a stream
                FileStream newStream = newFile.OpenWrite();

                // write in the new file the buffer encrypted/decrypted
                newStream.Write(crypted_buffer, 0, crypted_buffer.Length);

                // close the stream
                newStream.Close();

        }

        static byte[] EncryptOrDecryptByte(byte[] buffer)
        {
            byte[] key = { 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18 };

            // for each character from the file
            for (int index = 0; index < buffer.Length; index++)
            {
                // perform XOR on the character using the key
                buffer[index] = (byte)(buffer[index] ^ key[index % key.Length]);
            }
            return buffer;
        }
    }
}
