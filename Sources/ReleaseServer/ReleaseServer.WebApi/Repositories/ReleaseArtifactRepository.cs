using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ReleaseServer.WebApi.Mappers;
using ReleaseServer.WebApi.Models;

namespace ReleaseServer.WebApi.Repositories
{
    public class FsReleaseArtifactRepository : IReleaseArtifactRepository
    {
        private readonly string ArtifactRoot;

        public FsReleaseArtifactRepository(string artifactRoot = "./")
        {
            ArtifactRoot = artifactRoot;
        }
        
        public async Task StoreArtifact(ReleaseArtifactModel artifact)
        {
            
           var path = GeneratePath(
                artifact.ProductInformation.ProductIdentifier,
                artifact.ProductInformation.Os,
                artifact.ProductInformation.HwArchitecture,
                artifact.ProductInformation.Version);

            try
            {
                //If the directory already exists, delete the old content in there
                if (Directory.Exists(path))
                {
                    Console.WriteLine("This path already exits! Old content will be deleted!");
                    
                    var dirInfo = new DirectoryInfo(path);
                        
                    foreach (var file in dirInfo.GetFiles())
                    {
                        file.Delete();
                    }
                    
                    Console.Write("Old path successfully deleted!");
                }
                else
                {
                    //Create the directory
                    var dirInfo = Directory.CreateDirectory(path);
                    Console.WriteLine("The directory {0} was successfully created", dirInfo.FullName);
                }
                
                await Task.Run(() => artifact.Payload.ExtractToDirectory(path));
                Console.WriteLine("The Artifact {0} was successfully unpacked & stored");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<ProductInformationModel> GetInfosByProductName(string productName)
        {
            //Get all directories / subdirectories
            //TODO: filtering with File operations
            var directories = Directory.GetDirectories(Path.Combine(ArtifactRoot, productName), "*", SearchOption.AllDirectories);
            var retVal = new List<ProductInformationModel>();
            
            foreach (var directory in directories)
            {
                var productInfo = ProductInformationMapper.PathToProductInfo(directory);
                    
                //Is null, if the directory hasn't a depth of 5
                if (productInfo != null)
                {
                    retVal.Add(productInfo);
                }
            }
            return retVal;
        }

        public string GetReleaseInfo(string product, string os, string architecture, string version)
        {
            try
            {
                var path = GeneratePath(product, os, architecture, version);

                if (Directory.Exists(path))
                {
                    //Get the file information of the artifact (artifact must be a ZIP!)
                    //TODO: Clarify, whether we have only a Zip or unzipped files.
                    var dir = new DirectoryInfo(path);
                    var files = dir.GetFiles();
                    
                    //Assumption: only one Zip-File is stored as artifact
                    using (ZipArchive archive = ZipFile.OpenRead(files.First().FullName))
                    {
                        var changelogEntry = archive.GetEntry("changelog.txt");

                        var changelog = changelogEntry.Open();
                        var reader = new StreamReader(changelog);

                        var payload = reader.ReadToEnd();
                        return payload;
                    }
                }
                Console.WriteLine("The directory {0} does not exist!", path);
                return (new string("Error: Release notes for this artifact not found!"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public byte[] GetSpecificArtifact(string productName, string os, string architecture, string version)
        {
            try
            {
                var path = GeneratePath(productName, os, architecture, version);
                
                if (Directory.Exists(path))
                {
                    //Get the file information of the artifact (artifact must be a ZIP!)
                    //TODO: Clarify, whether we have only a Zip or unzipped files.
                    //TODO: Refactor -> it has to be smarter!
                    var dir = new DirectoryInfo(path);
                    var files = dir.GetFiles();
                    
                    byte[] artifact = File.ReadAllBytes(Path.Combine(path, files.First().FullName));
                    
                    return artifact;
                }
                Console.WriteLine("The directory {0} does not exist!", path);
                throw new FileNotFoundException();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string GeneratePath(string product, string os, string architecture, string version)
        {
            return Path.Combine(ArtifactRoot, product, os, architecture, version);
        }
    }
    
    public interface IReleaseArtifactRepository     
    {
        Task StoreArtifact(ReleaseArtifactModel artifact);
        List<ProductInformationModel> GetInfosByProductName(string productName);
        string GetReleaseInfo(string product, string os, string architecture, string version);
        byte[] GetSpecificArtifact(string productName, string os, string architecture, string version);
    }
}




