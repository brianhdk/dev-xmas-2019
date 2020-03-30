using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace XMAS2019.Domain.Services
{
    public class ToyDistributionProblemRepository : IToyDistributionProblemRepository
    {
        private readonly CloudBlobClient _client;

        public ToyDistributionProblemRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("...");

            _client = storageAccount.CreateCloudBlobClient();
            
        }

        public async Task<Uri> Save(Attempt attempt, ToyDistributionProblem problem, CancellationToken token)
        {
            if (attempt == null) throw new ArgumentNullException(nameof(attempt));
            if (problem == null) throw new ArgumentNullException(nameof(problem));

            CloudBlockBlob blob = Container.GetBlockBlobReference(FileName(attempt));

            using var stream = new MemoryStream();
            using var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);

            Serializer.Serialize(xmlWriter, problem);

            stream.Seek(0, SeekOrigin.Begin);

            await blob.UploadFromStreamAsync(stream, token);

            return blob.Uri;
        }

        public async Task<ToyDistributionProblem> Get(Attempt attempt, CancellationToken token)
        {
            if (attempt == null) throw new ArgumentNullException(nameof(attempt));

            CloudBlockBlob blob = Container.GetBlockBlobReference(FileName(attempt));

            if (!await blob.ExistsAsync(token))
                return null;
            
            using Stream blobStream = await blob.OpenReadAsync(token);
            using StreamReader blobReader = new StreamReader(blobStream);

            var xml = XDocument.Parse(await blobReader.ReadToEndAsync());
            using XmlReader xmlReader = xml.CreateReader();

            return (ToyDistributionProblem) Serializer.Deserialize(xmlReader);
        }

        private static string FileName(Attempt attempt)
        {
            return $"{attempt.Id:N}.xml";
        }

        private XmlSerializer Serializer => new XmlSerializer(typeof(ToyDistributionProblem));
        private CloudBlobContainer Container => _client.GetContainerReference("toydistribution");
    }
}