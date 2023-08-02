using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ETS_TOOL.DecryptFiles
{
    public class Decrypt
    {
        [DllImport("Assets/SII_Decrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetMemoryFormat")]
        public static extern unsafe int SIIGetMemoryFormat(byte* InputMS, uint InputMSSize);

        [DllImport("Assets/SII_Decrypt.dll", EntryPoint = "DecryptAndDecodeMemory")]
        public static extern unsafe Int32 SIIDecryptAndDecodeMemory(byte* InputMS, uint InputMSSize, byte* OutputMS, uint* OutputMSSize);

        [DllImport("Assets/SII_Decrypt.dll", EntryPoint = "DecodeMemory")]
        public static extern unsafe Int32 SIIDecodeMemory(byte* InputMS, uint InputMSSize, byte* OutputMS, uint* OutputMSSize);

        public unsafe string[] NewDecodeFile(string _saveFilePath)
        {

            byte[] FileDataByte = new byte[10];
            try
            {
                FileDataByte = File.ReadAllBytes(_saveFilePath);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Could not find file in: " + _saveFilePath);
            }

            int MemFileFrm = -1;
            UInt32 buff = (UInt32)FileDataByte.Length;

            fixed (byte* ptr = FileDataByte)
            {
                MemFileFrm = SIIGetMemoryFormat(ptr, buff);
            }


            switch (MemFileFrm)
            {
                case 1:
                    {
                        string BigS = Encoding.UTF8.GetString(FileDataByte);
                        return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    }
                case 2:
                    // "SIIDEC_RESULT_FORMAT_ENCRYPTED";
                    {
                        int result = -1;
                        uint newbuff = 0;
                        uint* newbuffP = &newbuff;

                        fixed (byte* ptr = FileDataByte)
                        {
                            result = SIIDecryptAndDecodeMemory(ptr, buff, null, newbuffP);
                        }

                        if (result == 0)
                        {
                            byte[] newFileData = new byte[(int)newbuff];

                            fixed (byte* ptr = FileDataByte)
                            {
                                fixed (byte* ptr2 = newFileData)
                                    result = SIIDecryptAndDecodeMemory(ptr, buff, ptr2, newbuffP);
                            }

                            string BigS = Encoding.UTF8.GetString(newFileData);
                            return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                        }

                        return null;
                    }
                case 4:
                    // "SIIDEC_RESULT_FORMAT_3NK";
                    {
                        int result = -1;
                        uint newbuff = 0;
                        uint* newbuffP = &newbuff;

                        fixed (byte* ptr = FileDataByte)
                        {
                            result = SIIDecodeMemory(ptr, buff, null, newbuffP);
                        }

                        if (result == 0)
                        {
                            byte[] newFileData = new byte[(int)newbuff];

                            fixed (byte* ptr = FileDataByte)
                            {
                                fixed (byte* ptr2 = newFileData)
                                    result = SIIDecodeMemory(ptr, buff, ptr2, newbuffP);
                            }

                            string BigS = Encoding.UTF8.GetString(newFileData);
                            return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
