using System;
using System.IO;

namespace MAL_Reviewer_Core
{
    /// <summary>
    /// The application's configurations.
    /// </summary>
    public class Config
    {
        #region Fields

        /// <summary>
        /// The path to the storage location.
        /// </summary>
        public readonly string StoragePath;

        /// <summary>
        /// The default storage folder's name.
        /// </summary>
        public readonly string StorageFolder = "MAL_Reviewer";

        #endregion

        #region Properties

        /// <summary>
        /// The full path of the storage folder.
        /// </summary>
        public string StorageDirectory => Path.Combine(this.StoragePath, this.StorageFolder);

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor, storing data in the user's application data folder.
        /// </summary>
        public Config() : this(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) { }

        /// <summary>
        /// Constructor with a custom storage location.
        /// </summary>
        /// <param name="storagePath"> The folder that holds the storage folder. </param>
        public Config(string storagePath)
        {
            this.StoragePath = storagePath;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates the storage folder if it doesn't exist.
        /// </summary>
        public void PrepareStorageFolder()
        {
            Directory.CreateDirectory(this.StorageDirectory);
        }

        /// <summary>
        /// Checks if the file where the data resides exists.
        /// </summary>
        /// <param name="file"> The file's name. </param>
        public bool DoesStorageFileExist(string file)
        {
            return File.Exists(GetStorageFilePath(file));
        }

        /// <summary>
        /// Returns the full path of a file in the storage folder.
        /// </summary>
        /// <param name="file"> The file's name. </param>
        public string GetStorageFilePath(string file)
        {
            return Path.Combine(this.StorageDirectory, file);
        }

        #endregion
    }
}
