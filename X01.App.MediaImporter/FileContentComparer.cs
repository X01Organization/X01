using System;
using System.Buffers;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace X01.App.MediaImporter;
/// <summary>
/// 用于按内容比较文件的实例比较器。
/// 提供单个实例 FileContentComparer.Instance，用于替代静态方法调用。
/// </summary>
public sealed class FileContentComparer
{
    // Limit concurrent callers to MatchesByContent to 3 threads.
    private static readonly SemaphoreSlim _concurrencyLimiter = new(3);
    private const int BufferSize = 10 * 1024 * 1024; // 10 MB

    public bool MatchesByContent(FileInfo fi1, FileInfo fi2)
    {
        if (fi1.FullName == fi2.FullName)
        {
            return true;
        }

        if (fi1.Length != fi2.Length)
        {
            return false;
        }

        // Limit concurrent callers
        _concurrencyLimiter.Wait();
        try
        {
            var pool = ArrayPool<byte>.Shared;
            byte[] buffer1 = pool.Rent(BufferSize);
            try
            {
                byte[] buffer2 = pool.Rent(BufferSize);
                try
                {
                    return MatchesByContent(fi1, fi2, buffer1, buffer2);
                }
                finally
                {
                    pool.Return(buffer2);
                }
            }
            finally
            {
                pool.Return(buffer1);
            }
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }

    private bool MatchesByContent(FileInfo fi1, FileInfo fi2, byte[] buffer1, byte[] buffer2)
    {
        try
        {
            using (FileStream s1 = fi1.OpenRead())
            {
                using (FileStream s2 = fi2.OpenRead())
                {
                    int bytesRead1, bytesRead2;
                    while (true)
                    {
                        bytesRead1 = s1.Read(buffer1);
                        bytesRead2 = s2.Read(buffer2);

                        if (bytesRead1 != bytesRead2)
                        {
                            return false;
                        }

                        if (bytesRead1 <= 0)
                        {
                            return true;
                        }

                        if (!MemoryExtensions.SequenceEqual(
                                buffer1.AsSpan(0, bytesRead1),
                                buffer2.AsSpan(0, bytesRead2)))
                        {
                            return false;
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