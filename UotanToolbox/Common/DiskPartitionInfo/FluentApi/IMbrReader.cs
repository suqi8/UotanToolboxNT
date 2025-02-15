using System.IO;
using DiskPartitionInfo.Mbr;

namespace DiskPartitionInfo.FluentApi
{
    public interface IMbrReader
    {
#if Windows
        /// <summary>
        /// Reads the MBR from the physical drive given its number.
        /// </summary>
        /// <param name="driveNumber">Drive number, e.g. 0 or 3.</param>
        /// <returns>The Master Boot Record information.</returns>
        MasterBootRecord FromPhysicalDriveNumber(int driveNumber);

        /// <summary>
        /// Reads the MBR record from the physical drive given a volume letter.
        /// MBR will be read from the corresponding physical drive if it exists.
        /// </summary>
        /// <param name="volumeLetter">Volume letter, e.g. C: or F:.</param>
        /// <returns>The Master Boot Record information.</returns>
        MasterBootRecord FromVolumeLetter(string volumeLetter);
#endif

        /// <summary>
        /// Reads the MBR from the given path.
        /// It can be a path to a file or to a physical drive.
        /// </summary>
        /// <param name="path">Path to disk or file to read from,
        /// e.g. C:\MBR.bin or ../disk.img.</param>
        /// <returns>The Master Boot Record information.</returns>
        MasterBootRecord FromPath(string path);

        /// <summary>
        /// Reads the MBR from the given stream.
        /// The stream is not automatically closed after read operation.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>The Master Boot Record information.</returns>
        MasterBootRecord FromStream(Stream stream);
    }
}
