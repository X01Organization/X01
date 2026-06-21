using System;
using System.Buffers;
using System.IO;
using System.Collections.Generic;

namespace X01.App.MediaImporter;
/// <summary>
/// 用于按内容比较文件的实例比较器。
/// 提供单个实例 FileContentComparer.Instance，用于替代静态方法调用。
/// </summary>
public sealed class FileContentComparer 
{
    private readonly byte[] _buffer1 = new byte[10 * 1024 * 1024];
    private readonly byte[] _buffer2 = new byte[10 * 1024 * 1024];


    public bool IsSame(FileInfo fi1, FileInfo fi2)
    {
        if (fi1.FullName == fi2.FullName)
        {
            return true;
        }

        if (fi1.Length != fi2.Length)
        {
            return false;
        }

        try
        {
            using (FileStream s1 = fi1.OpenRead())
            {
                using (FileStream s2 = fi2.OpenRead())
                {
                    lock (_buffer1)
                    {
                        lock (_buffer2)
                        {
                            int bytesRead1, bytesRead2;
                            while (true)
                            {
                                bytesRead1 = s1.Read(_buffer1);
                                bytesRead2 = s2.Read(_buffer2);

                                if (bytesRead1 != bytesRead2)
                                {
                                    return false;
                                }

                                if (bytesRead1 <= 0)
                                {
                                    return true;
                                }

                                if (!MemoryExtensions.SequenceEqual(
                                        _buffer1.AsSpan(0, bytesRead1),
                                        _buffer2.AsSpan(0, bytesRead2)))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error by comparing:");
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}