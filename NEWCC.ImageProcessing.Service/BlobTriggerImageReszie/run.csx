#r "System.Drawing"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Drawing;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;
using Microsoft.WindowsAzure.Storage.Blob;

using ImageProcessor;

public static void Run(Stream myBlob, string blobName, Binder binder, TraceWriter log)
{


    // Thumbnail
    resizeImage(myBlob, new Size(160, 0), binder, blobName);

    // Medium
    resizeImage(myBlob, new Size(500, 0), binder, blobName);

    // Full Screen
    resizeImage(myBlob, new Size(1500, 0), binder, blobName);

    
}

private static async void resizeImage(Stream blob, Size size, Binder binder, string blobName)
{
    using (var imageFactory = new ImageFactory())
    {

        var outStream = new MemoryStream();
        imageFactory.Load(blob).Resize(size).Format(new JpegFormat()).Save(outStream);

        var attributes = new Attribute[]
        {
            new BlobAttribute($"resized/"+size.Width+"/{blobName}"),
            new StorageAccountAttribute("newccazurefunctionsmedia")
        };

        using (var writer = await binder.BindAsync<CloudBlobStream>(attributes).ConfigureAwait(false))
        {
            var bytes = outStream.GetBuffer();
            writer.Write(bytes, 0, bytes.Length);
        }
    }
}

