﻿using System.IO;
using System.Security.Cryptography;
using System.Text;

public struct SHA1_Encode
{
    public static string Encryption(string ishText, string pass,
                string sol = "doberman", string cryptographicAlgorithm = "SHA1",
                int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
                int keySize = 256)
    {
        if (string.IsNullOrEmpty(ishText))
            return "";

        byte[] initVecB = System.Text.Encoding.ASCII.GetBytes(initVec);
        byte[] solB = Encoding.ASCII.GetBytes(sol);
        byte[] ishTextB = System.Text.Encoding.UTF8.GetBytes(ishText);

        System.Security.Cryptography.PasswordDeriveBytes derivPass = new System.Security.Cryptography.PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
        byte[] keyBytes = derivPass.GetBytes(keySize / 8);
        System.Security.Cryptography.RijndaelManaged symmK = new System.Security.Cryptography.RijndaelManaged();
        symmK.Mode = System.Security.Cryptography.CipherMode.CBC;

        byte[] cipherTextBytes = null;

        using (ICryptoTransform encryptor = symmK.CreateEncryptor(keyBytes, initVecB))
        {
            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
            {
                using (CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memStream, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                {

                    cryptoStream.Write(ishTextB, 0, ishTextB.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memStream.ToArray();
                    memStream.Close();
                    cryptoStream.Close();
                }
            }
        }

        symmK.Clear();
        return System.Convert.ToBase64String(cipherTextBytes);
    }
    public static string Decryption(string ciphText, string pass,
          string sol = "doberman", string cryptographicAlgorithm = "SHA1",
          int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
          int keySize = 256)
    {
        if (string.IsNullOrEmpty(ciphText))
            return "";

        byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
        byte[] solB = Encoding.ASCII.GetBytes(sol);
        byte[] cipherTextBytes = System.Convert.FromBase64String(ciphText);

        PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
        byte[] keyBytes = derivPass.GetBytes(keySize / 8);

        RijndaelManaged symmK = new RijndaelManaged();
        symmK.Mode = CipherMode.CBC;

        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int byteCount = 0;

        using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
        {
            using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                {
                    byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    mSt.Close();
                    cryptoStream.Close();
                }
            }
        }

        symmK.Clear();
        return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
    }
}
