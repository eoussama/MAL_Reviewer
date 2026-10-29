#pragma warning disable SYSLIB0011, SYSLIB0050 // BinaryFormatter is only used to produce the files that versions before 0.2.0 wrote.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MAL_Reviewer_Core.Tests
{
    /// <summary>
    /// Writes settings.dat files exactly like versions before 0.2.0 did: a BinaryFormatter payload of
    /// MAL_Reviewer_Core.Settings from the MAL_Reviewer_Core 1.0.0.0 assembly.
    /// </summary>
    internal static class LegacyPayload
    {
        private const string ReplicaPrefix = "Legacy.";
        private const string LegacyAssembly = "MAL_Reviewer_Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public static Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewTemplateModel Template(string name, DateTime created, DateTime modified, params Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewAspectModel[] aspects)
        {
            return new Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewTemplateModel
            {
                TemplateName = name,
                UseIntro = true,
                UserTLDR = false,
                CreationDate = created,
                ModificationDate = modified,
                TemplateAspects = new List<Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewAspectModel>(aspects)
            };
        }

        public static Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewAspectModel Aspect(string name, string review = "", double rating = 0)
        {
            return new Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewAspectModel { AspectName = name, AspectReview = review, AspectRating = rating };
        }

        public static byte[] Write(object root)
        {
            BinaryFormatter formatter = new BinaryFormatter { Binder = new LegacyNamesBinder() };

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, root);
                return stream.ToArray();
            }
        }

        public static byte[] Write(string defaultUser, params Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewTemplateModel[] templates)
        {
            return Write(new Legacy.MAL_Reviewer_Core.Settings
            {
                UserSettings = new Legacy.MAL_Reviewer_Core.controllers.UserController { DefaultUser = defaultUser },
                ReviewTemplatesSettings = new Legacy.MAL_Reviewer_Core.controllers.ReviewTemplatesController
                {
                    ReviewTemplates = new List<Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels.ReviewTemplateModel>(templates)
                }
            });
        }

        /// <summary>
        /// Writes the replica types under the names and assembly of the original ones.
        /// </summary>
        private class LegacyNamesBinder : SerializationBinder
        {
            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                string testAssembly = typeof(LegacyPayload).Assembly.FullName;

                typeName = serializedType.FullName.Replace(ReplicaPrefix, string.Empty).Replace(testAssembly, LegacyAssembly);
                assemblyName = serializedType.Assembly == typeof(LegacyPayload).Assembly ? LegacyAssembly : null;
            }

            public override Type BindToType(string assemblyName, string typeName) => throw new NotSupportedException();
        }
    }
}

// Replicas of the classes as they were serialized before 0.2.0: same names, same auto-properties.
namespace Legacy.MAL_Reviewer_Core
{
    [Serializable]
    public class Settings
    {
        public controllers.UserController UserSettings { get; set; }

        public controllers.ReviewTemplatesController ReviewTemplatesSettings { get; set; }
    }
}

namespace Legacy.MAL_Reviewer_Core.controllers
{
    [Serializable]
    public class UserController
    {
        public string DefaultUser { get; set; } = string.Empty;
    }

    [Serializable]
    public class ReviewTemplatesController
    {
        public static readonly short MaxReviewTemplates = 10;

        public List<models.ReviewTemplateModels.ReviewTemplateModel> ReviewTemplates { get; set; } = new List<models.ReviewTemplateModels.ReviewTemplateModel>();
    }
}

namespace Legacy.MAL_Reviewer_Core.models.ReviewTemplateModels
{
    [Serializable]
    public class ReviewTemplateModel
    {
        public string TemplateName { get; set; }

        public bool UseIntro { get; set; }

        public bool UserTLDR { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public List<ReviewAspectModel> TemplateAspects { get; set; }
    }

    [Serializable]
    public class ReviewAspectModel
    {
        public string AspectName { get; set; }

        public string AspectReview { get; set; }

        public double AspectRating { get; set; }
    }
}

namespace Legacy
{
    /// <summary>
    /// A serializable object that is not a settings file.
    /// </summary>
    [Serializable]
    public class Unrelated
    {
        public string Name { get; set; } = "nope";
    }
}
